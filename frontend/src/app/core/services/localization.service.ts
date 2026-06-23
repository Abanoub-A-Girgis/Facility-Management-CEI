import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, LocalizationPayload } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private readonly translationsSubject = new BehaviorSubject<Record<string, string>>({});
  private readonly cultureSubject = new BehaviorSubject<string>(this.initialCulture());
  private readonly directionSubject = new BehaviorSubject<'ltr' | 'rtl'>('ltr');

  readonly translations$ = this.translationsSubject.asObservable();
  readonly culture$ = this.cultureSubject.asObservable();
  readonly direction$ = this.directionSubject.asObservable();

  constructor(
    private readonly http: HttpClient,
    @Inject(DOCUMENT) private readonly document: Document
  ) {
    this.loadCulture(this.cultureSubject.value).subscribe();
  }

  loadCulture(culture: string): Observable<Record<string, string>> {
    const normalizedCulture = culture?.toLowerCase().startsWith('ar') ? 'ar' : 'en';

    return this.http
      .get<ApiResponse<LocalizationPayload>>(`${environment.apiBaseUrl}/api/localization`, {
        params: { culture: normalizedCulture }
      })
      .pipe(
        map(response => response.data),
        tap(payload => this.applyPayload(payload)),
        map(payload => payload.translations),
        catchError(() => {
          this.applyPayload({
            culture: 'en',
            direction: 'ltr',
            translations: {}
          });
          return of({});
        })
      );
  }

  setCulture(culture: string): void {
    const normalizedCulture = culture?.toLowerCase().startsWith('ar') ? 'ar' : 'en';
    localStorage.setItem('culture', normalizedCulture);
    this.loadCulture(normalizedCulture).subscribe();
  }

  translate(key: string): string {
    return this.translationsSubject.value[key] ?? key;
  }

  private applyPayload(payload: LocalizationPayload): void {
    this.cultureSubject.next(payload.culture);
    this.directionSubject.next(payload.direction);
    this.translationsSubject.next(payload.translations ?? {});
    this.document.documentElement.lang = payload.culture;
    this.document.documentElement.dir = payload.direction;
  }

  private initialCulture(): string {
    const storedCulture = localStorage.getItem('culture');
    if (storedCulture) {
      return storedCulture;
    }

    return navigator.language?.toLowerCase().startsWith('ar') ? 'ar' : 'en';
  }
}
