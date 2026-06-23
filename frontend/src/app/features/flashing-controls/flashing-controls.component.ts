import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslatePipe } from '../../core/pipes/translate.pipe';

@Component({
  selector: 'app-flashing-controls',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './flashing-controls.component.html',
  styleUrl: './flashing-controls.component.css'
})
export class FlashingControlsComponent {
  @Input() highlightEnabled = true;
  @Input() flashingEnabled = true;
  @Output() highlightEnabledChange = new EventEmitter<boolean>();
  @Output() flashingEnabledChange = new EventEmitter<boolean>();

  setHighlight(event: Event): void {
    this.highlightEnabledChange.emit((event.target as HTMLInputElement).checked);
  }

  setFlashing(event: Event): void {
    this.flashingEnabledChange.emit((event.target as HTMLInputElement).checked);
  }
}
