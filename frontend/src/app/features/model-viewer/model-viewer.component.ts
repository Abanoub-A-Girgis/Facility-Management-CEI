import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { BuildingElement, BuildingModel, Floor, ViewerHighlight } from '../../core/models/api.models';
import { ElementsService } from '../../core/services/elements.service';
import { FloorsService } from '../../core/services/floors.service';
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

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly modelsService: ModelsService,
    private readonly floorsService: FloorsService,
    private readonly elementsService: ElementsService,
    private readonly zone: NgZone
  ) {}

  ngOnInit(): void {
    const routeModelId = Number(this.route.snapshot.paramMap.get('modelId'));
    this.selectedModelId = routeModelId || undefined;
    this.loadModels();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    if (this.selectedModelId) {
      this.loadModelData(this.selectedModelId);
    }
  }

  ngOnDestroy(): void {
    this.stopFlashing();
  }

  loadModels(): void {
    this.loading = true;
    this.modelsService.getModels().subscribe({
      next: models => {
        this.models = models;
        this.loading = false;
        if (!this.selectedModelId && models.length) {
          this.selectedModelId = models[0].id;
          this.router.navigate(['/models', this.selectedModelId, 'viewer']);
        }
        if (this.selectedModelId && this.viewReady) {
          this.loadModelData(this.selectedModelId);
        }
      },
      error: () => {
        this.loading = false;
        this.error = 'Could not load models from the API.';
      }
    });
  }

  onModelChange(): void {
    if (!this.selectedModelId) {
      return;
    }
    this.router.navigate(['/models', this.selectedModelId, 'viewer']);
    this.loadModelData(this.selectedModelId);
  }

  loadModelData(modelId: number): void {
    this.error = '';
    this.selectedElement = undefined;
    this.selectedElementId = undefined;
    this.viewerLoading = true;
    this.stopFlashing();

    this.floorsService.getByModel(modelId).subscribe({
      next: floors => {
        this.floors = floors;
        this.visibleFloorIds = new Set(floors.map(floor => floor.id));
        this.modelsService.getViewerHighlights(modelId).subscribe({
          next: highlights => {
            this.highlights = this.normalizeHighlights(highlights);
            this.initializeViewer();
          },
          error: () => {
            this.error = 'Could not load viewer highlight metadata.';
            this.initializeViewer();
          }
        });
      },
      error: () => {
        this.viewerLoading = false;
        this.error = 'Could not load floors for the selected model.';
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

  private initializeViewer(): void {
    if (!this.viewReady || !this.floors.length) {
      this.viewerLoading = false;
      return;
    }

    this.loadViewerScripts()
      .then(() => {
        const canvas = document.getElementById('viewer-canvas');
        if (!canvas || !window.xViewer) {
          throw new Error('XBIM viewer scripts are not available.');
        }

        this.stopFlashing();
        canvas.replaceWith(canvas.cloneNode(false));

        this.viewer = new window.xViewer('viewer-canvas');
        if (window.xNavigationCube) {
          const navCube = new window.xNavigationCube();
          navCube.ratio = 0.1;
          this.viewer.addPlugin(navCube);
        }

        this.viewer.defineStyle(0, [255, 0, 0, 255]);
        this.viewer.defineStyle(1, [255, 160, 16, 255]);
        this.viewer.defineStyle(2, [255, 224, 32, 255]);
        this.viewer.defineStyle(3, [0, 255, 0, 255]);
        this.viewer.on('pick', (args: { id: number }) => this.zone.run(() => this.onElementPicked(args.id)));
        this.viewer.start();

        this.loadFloorModels(0);
      })
      .catch(error => {
        this.viewerLoading = false;
        this.error = error.message ?? 'The model viewer could not be initialized.';
      });
  }

  private loadFloorModels(index: number): void {
    if (!this.viewer || index >= this.floors.length) {
      this.viewerLoading = false;
      this.applyHighlighting();
      return;
    }

    const floor = this.floors[index];
    this.viewer.load(this.assetUrl(floor.wexBimPath));
    this.viewer.on('loaded', () => {
      this.viewer.off('loaded');
      this.loadFloorModels(index + 1);
    });
  }

  private onElementPicked(elementId: number): void {
    this.selectedElementId = elementId;
    if (this.viewer) {
      this.viewer.setCameraTarget(elementId);
      this.viewer.setState(window.xState?.HIGHLIGHTED, [elementId]);
    }

    this.elementsService.getElement(elementId).subscribe({
      next: element => {
        this.selectedElement = element;
        this.error = '';
      },
      error: () => {
        this.selectedElement = undefined;
        this.error = `Element ${elementId} was selected, but details were not found in the API.`;
      }
    });
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
      }, 750);
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
      script.onerror = () => reject(new Error(`Could not load ${src}`));
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
      rooms: highlights.rooms ?? []
    };
  }
}
