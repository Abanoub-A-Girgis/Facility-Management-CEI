import { ChangeDetectorRef, OnDestroy, Pipe, PipeTransform } from '@angular/core';
import { Subscription } from 'rxjs';
import { LocalizationService } from '../services/localization.service';

@Pipe({
  name: 't',
  standalone: true,
  pure: false
})
export class TranslatePipe implements PipeTransform, OnDestroy {
  private translations: Record<string, string> = {};
  private readonly subscription: Subscription;

  constructor(
    localization: LocalizationService,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {
    this.subscription = localization.translations$.subscribe(translations => {
      this.translations = translations;
      this.changeDetectorRef.markForCheck();
    });
  }

  transform(key: string): string {
    return this.translations[key] ?? key;
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
