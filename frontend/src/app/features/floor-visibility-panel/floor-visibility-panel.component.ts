import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Floor } from '../../core/models/api.models';
import { TranslatePipe } from '../../core/pipes/translate.pipe';

@Component({
  selector: 'app-floor-visibility-panel',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './floor-visibility-panel.component.html',
  styleUrl: './floor-visibility-panel.component.css'
})
export class FloorVisibilityPanelComponent {
  @Input() floors: Floor[] = [];
  @Input() visibleFloorIds = new Set<number>();
  @Output() floorVisibilityChanged = new EventEmitter<{ floor: Floor; visible: boolean }>();

  isVisible(floor: Floor): boolean {
    return this.visibleFloorIds.has(floor.id);
  }

  onToggle(floor: Floor, event: Event): void {
    const input = event.target as HTMLInputElement;
    this.floorVisibilityChanged.emit({ floor, visible: input.checked });
  }
}
