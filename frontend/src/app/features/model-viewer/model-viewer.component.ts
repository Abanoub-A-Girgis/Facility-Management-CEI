import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BuildingElement, BuildingModel, Floor, ViewerHighlight } from '../../core/models/api.models';
import { TranslatePipe } from '../../core/pipes/translate.pipe';
import { ElementsService } from '../../core/services/elements.service';
import { FloorsService } from '../../core/services/floors.service';
import { LocalizationService } from '../../core/services/localization.service';
import { ModelsService } from '../../core/services/models.service';
import { FlashingControlsComponent } from '../flashing-controls/flashing-controls.component';
import { FloorVisibilityPanelComponent } from '../floor-visibility-panel/floor-visibility-panel.component';
import { IssuePanelComponent } from '../issue-panel/issue-panel.component';

declare global {
  interface Window {
    xViewer?: any;
    xNavigationCube?: any;
    xState?: any;
  }
}

@Component({
  selector: 'app-model-viewer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    FloorVisibilityPanelComponent,
    IssuePanelComponent,
    FlashingControlsComponent
  ],
  templateUrl: './model-viewer.component.html',
  styleUrl: './model-viewer.component.css'
})
export class ModelViewerComponent implements OnInit, AfterViewInit, OnDestroy {
  models: BuildingModel[] = [];
  selectedModelId?: number;
  floors: Floor[] = [];
  visibleFloorIds = new Set<number>();
  selectedElement?: BuildingElement;
  selectedElementId?: number;
  highlights: ViewerHighlight = { severe: [], high: [], medium: [], low: [], rooms: [] };
  loading = false;
  viewerLoading = false;
  error = '';
  highlightEnabled = true;
  flashingEnabled = true;

  private viewer: any;
  private viewReady = false;
  private flashingTimer?: number;
  private flashOn = true;
  private scriptsLoaded?: Promise<void>;
  private readonly destroy$ = new Subject<void>();
  private modelRequestVersion = 0;
  private elementRequestVersion = 0;
  private activeModelId?: number;
  private destroyed = false;
  private pickHandler?: (args: { id: number | null }) => void;
  private loadedHandler?: () => void;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly modelsService: ModelsService,
    private readonly floorsService: FloorsService,
    private readonly elementsService: ElementsService,
    private readonly localization: LocalizationService,
    private readonly zone: NgZone
  ) {}

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const routeModelId = Number(params.get('modelId')) || undefined;
      this.selectedModelId = routeModelId;
      this.tryLoadSelectedModel();
    });
    this.loadModels();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.tryLoadSelectedModel();
  }

  ngOnDestroy(): void {
    this.destroyed = true;
    this.modelRequestVersion++;
    this.destroy$.next();
    this.destroy$.complete();
    this.teardownViewer();
  }

  loadModels(): void {
    this.loading = true;
    this.modelsService.getModels().pipe(takeUntil(this.destroy$)).subscribe({
      next: models => {
        this.models = models;
        this.loading = false;
        if (!this.selectedModelId && models.length) {
          this.selectedModelId = models[0].id;
          this.router.navigate(['/models', this.selectedModelId, 'viewer']);
          return;
        }
        this.tryLoadSelectedModel();
      },
      error: () => {
        this.loading = false;
        this.error = this.localization.translate('Viewer.LoadModelsError');
      }
    });
  }

  onModelChange(): void {
    if (!this.selectedModelId) {
      return;
    }
    this.router.navigate(['/models', this.selectedModelId, 'viewer']);
  }

  loadModelData(modelId: number): void {
    if (this.destroyed || this.activeModelId === modelId) {
      return;
    }

    const requestVersion = ++this.modelRequestVersion;
    this.activeModelId = modelId;
    this.error = '';
    this.selectedElement = undefined;
    this.selectedElementId = undefined;
    this.viewerLoading = true;
    this.teardownViewer();

    this.floorsService.getByModel(modelId).pipe(takeUntil(this.destroy$)).subscribe({
      next: floors => {
        if (!this.isCurrentRequest(requestVersion)) {
          return;
        }
        this.floors = floors;
        this.visibleFloorIds = new Set(floors.map(floor => floor.id));
        this.modelsService.getViewerHighlights(modelId).pipe(takeUntil(this.destroy$)).subscribe({
          next: highlights => {
            if (!this.isCurrentRequest(requestVersion)) {
              return;
            }
            this.highlights = this.normalizeHighlights(highlights);
            this.initializeViewer(requestVersion);
          },
          error: () => {
            if (!this.isCurrentRequest(requestVersion)) {
              return;
            }
            this.error = this.localization.translate('Viewer.LoadHighlightsError');
            this.initializeViewer(requestVersion);
          }
        });
      },
      error: () => {
        if (!this.isCurrentRequest(requestVersion)) {
          return;
        }
        this.activeModelId = undefined;
        this.viewerLoading = false;
        this.error = this.localization.translate('Viewer.LoadFloorsError');
      }
    });
  }

  onFloorVisibilityChanged(change: { floor: Floor; visible: boolean }): void {
    if (change.visible) {
      this.visibleFloorIds.add(change.floor.id);
    } else {
      this.visibleFloorIds.delete(change.floor.id);
    }
    this.applyFloorVisibility(change.floor);
  }

  onHighlightEnabledChange(enabled: boolean): void {
    this.highlightEnabled = enabled;
    this.applyHighlighting();
  }

  onFlashingEnabledChange(enabled: boolean): void {
    this.flashingEnabled = enabled;
    this.applyHighlighting();
  }

  zoomToSelected(): void {
    if (this.viewer && this.selectedElementId) {
      this.viewer.zoomTo(this.selectedElementId);
    }
  }

  private initializeViewer(requestVersion: number): void {
    if (!this.viewReady || !this.floors.length) {
      this.viewerLoading = false;
      return;
    }

    this.loadViewerScripts()
      .then(() => {
        if (!this.isCurrentRequest(requestVersion)) {
          return;
        }
        const canvas = document.getElementById('viewer-canvas');
        if (!canvas || !window.xViewer) {
          throw new Error(this.localization.translate('Viewer.ScriptsUnavailable'));
        }

        this.teardownViewer();
        canvas.replaceWith(canvas.cloneNode(false));

        this.viewer = new window.xViewer('viewer-canvas');
        if (window.xNavigationCube) {
          const navCube = new window.xNavigationCube();
          navCube.ratio = 0.1;
          this.viewer.addPlugin(navCube);
        }

        this.viewer.defineStyle(0, this.severityColor('Severe', [255, 0, 0, 255]));
        this.viewer.defineStyle(1, this.severityColor('High', [255, 160, 16, 255]));
        this.viewer.defineStyle(2, this.severityColor('Medium', [255, 224, 32, 255]));
        this.viewer.defineStyle(3, this.severityColor('Low', [0, 255, 0, 255]));
        this.pickHandler = (args: { id: number | null }) => this.zone.run(() => this.onElementPicked(args.id));
        this.viewer.on('pick', this.pickHandler);
        this.viewer.start();

        this.loadFloorModels(0, requestVersion);
      })
      .catch(error => {
        if (!this.isCurrentRequest(requestVersion)) {
          return;
        }
        this.activeModelId = undefined;
        this.viewerLoading = false;
        this.error = error.message ?? this.localization.translate('Viewer.InitError');
      });
  }

  private loadFloorModels(index: number, requestVersion: number): void {
    if (!this.viewer || !this.isCurrentRequest(requestVersion)) {
      return;
    }

    if (index >= this.floors.length) {
      this.viewerLoading = false;
      this.applyHighlighting();
      return;
    }

    const floor = this.floors[index];
    this.loadedHandler = () => {
      if (this.viewer && this.loadedHandler) {
        this.viewer.off('loaded', this.loadedHandler);
      }
      this.loadedHandler = undefined;
      this.loadFloorModels(index + 1, requestVersion);
    };
    this.viewer.on('loaded', this.loadedHandler);
    this.viewer.load(this.assetUrl(floor.wexBimPath));
  }

  private onElementPicked(elementId: number | null): void {
    if (elementId == null || !Number.isFinite(elementId) || elementId <= 0) {
      this.clearVisualSelection();
      this.elementRequestVersion++;
      this.selectedElementId = undefined;
      this.selectedElement = undefined;
      this.error = '';
      return;
    }

    if (this.selectedElementId !== elementId) {
      this.clearVisualSelection();
    }

    const requestVersion = ++this.elementRequestVersion;
    this.selectedElementId = elementId;
    if (this.viewer) {
      this.viewer.setCameraTarget(elementId);
      this.viewer.setState(window.xState?.HIGHLIGHTED, [elementId]);
    }

    if (!this.selectedModelId) {
      return;
    }

    this.elementsService.getViewerElement(this.selectedModelId, elementId).pipe(takeUntil(this.destroy$)).subscribe({
      next: element => {
        if (requestVersion !== this.elementRequestVersion || this.selectedElementId !== elementId) {
          return;
        }
        this.selectedElement = element;
        this.error = '';
      },
      error: () => {
        if (requestVersion !== this.elementRequestVersion || this.selectedElementId !== elementId) {
          return;
        }
        this.selectedElement = undefined;
        this.error = `${this.localization.translate('Element.Id')} ${elementId} ${this.localization.translate('Viewer.ElementDetailsMissing')}`;
      }
    });
  }

  private clearVisualSelection(): void {
    if (!this.viewer || !window.xState || !this.selectedElementId) {
      return;
    }

    let restoredState = window.xState.UNDEFINED;
    if (this.selectedElement && !this.visibleFloorIds.has(this.selectedElement.floorId)) {
      restoredState = window.xState.HIDDEN;
    } else if (this.highlights.rooms.includes(this.selectedElementId)) {
      restoredState = window.xState.HIGHLIGHTED;
    }

    this.viewer.setState(restoredState, [this.selectedElementId]);
  }

  private applyFloorVisibility(floor: Floor): void {
    if (!this.viewer || !window.xState) {
      return;
    }

    const modelIndex = this.floors.findIndex(item => item.id === floor.id);
    if (modelIndex < 0) {
      return;
    }

    const state = this.viewer.getModelState(modelIndex);
    const ids = state.map((entry: [number]) => entry[0]);
    this.viewer.setState(this.visibleFloorIds.has(floor.id) ? window.xState.UNDEFINED : window.xState.HIDDEN, ids);
  }

  private applyHighlighting(): void {
    if (!this.viewer) {
      return;
    }

    this.stopFlashing();
    this.viewer.resetStyles();

    if (!this.highlightEnabled) {
      return;
    }

    if (this.flashingEnabled) {
      this.flashingTimer = window.setInterval(() => {
        this.flashOn = !this.flashOn;
        this.paintHighlights(this.flashOn);
      }, this.flashInterval());
    } else {
      this.paintHighlights(true);
    }
  }

  private paintHighlights(show: boolean): void {
    if (!this.viewer) {
      return;
    }

    this.viewer.resetStyles();
    if (!show) {
      return;
    }

    this.viewer.setStyle(0, this.highlights.severe);
    this.viewer.setStyle(1, this.highlights.high);
    this.viewer.setStyle(2, this.highlights.medium);
    this.viewer.setStyle(3, this.highlights.low);
    if (window.xState && this.highlights.rooms.length) {
      this.viewer.setState(window.xState.HIGHLIGHTED, this.highlights.rooms);
    }
  }

  private stopFlashing(): void {
    if (this.flashingTimer) {
      window.clearInterval(this.flashingTimer);
      this.flashingTimer = undefined;
    }
    this.flashOn = true;
  }

  private tryLoadSelectedModel(): void {
    if (this.viewReady && this.models.length && this.selectedModelId) {
      this.loadModelData(this.selectedModelId);
    }
  }

  private isCurrentRequest(requestVersion: number): boolean {
    return !this.destroyed && requestVersion === this.modelRequestVersion;
  }

  private teardownViewer(): void {
    this.stopFlashing();
    if (!this.viewer) {
      return;
    }

    if (this.loadedHandler) {
      this.viewer.off('loaded', this.loadedHandler);
      this.loadedHandler = undefined;
    }
    if (this.pickHandler) {
      this.viewer.off('pick', this.pickHandler);
      this.pickHandler = undefined;
    }

    if (typeof this.viewer.dispose === 'function') {
      this.viewer.dispose();
    } else {
      this.viewer.stop?.();
    }
    this.viewer = undefined;
  }

  private loadViewerScripts(): Promise<void> {
    if (!this.scriptsLoaded) {
      const scripts = [
        '/Scripts/XBIM/xbim-viewer.debug.bundle.js',
        '/Scripts/XBIM/NavigationCube/xbim-navigation-cube-shaders.debug.js',
        '/Scripts/XBIM/NavigationCube/xbim-navigation-cube-textures.debug.js',
        '/Scripts/XBIM/NavigationCube/xbim-navigation-cube.debug.js'
      ];
      this.scriptsLoaded = scripts.reduce(
        (promise, script) => promise.then(() => this.loadScript(this.assetUrl(script))),
        Promise.resolve()
      );
    }
    return this.scriptsLoaded;
  }

  private loadScript(src: string): Promise<void> {
    return new Promise((resolve, reject) => {
      if (document.querySelector(`script[src="${src}"]`)) {
        resolve();
        return;
      }

      const script = document.createElement('script');
      script.src = src;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error(`${this.localization.translate('Viewer.ScriptLoadError')} ${src}`));
      document.body.appendChild(script);
    });
  }

  private assetUrl(path: string): string {
    if (!path) {
      return path;
    }
    if (path.startsWith('http')) {
      return path;
    }
    return `${environment.apiBaseUrl}${path.startsWith('/') ? path : `/${path}`}`;
  }

  private normalizeHighlights(highlights: ViewerHighlight): ViewerHighlight {
    return {
      severe: highlights.severe ?? [],
      high: highlights.high ?? [],
      medium: highlights.medium ?? [],
      low: highlights.low ?? [],
      rooms: highlights.rooms ?? [],
      severityColors: highlights.severityColors ?? {},
      flashIntervalMs: highlights.flashIntervalMs
    };
  }

  private severityColor(severity: string, fallback: number[]): number[] {
    const configured = this.highlights.severityColors?.[severity];
    if (!configured?.length) {
      return fallback;
    }

    const normalized = [...fallback];
    configured.slice(0, 4).forEach((value, index) => {
      normalized[index] = Math.max(0, Math.min(255, Number(value)));
    });
    if (configured.length === 3) {
      normalized[3] = 255;
    }

    return normalized;
  }

  private flashInterval(): number {
    return Math.max(250, Math.min(5000, this.highlights.flashIntervalMs ?? 750));
  }
}
