import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, LocalizationPayload } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private readonly fallbackTranslations: Record<string, string> = {
    'App.Title': 'Facility Management CEI',
    'Nav.Authorization': 'Authorization',
    'Nav.Upload': 'Upload',
    'Nav.Viewer': 'Viewer',
    'Nav.Logout': 'Log Out',
    'Language.English': 'English',
    'Language.Arabic': 'Arabic',
    'Authorization.Title': 'Authorization Required',
    'Authorization.Description': 'Sign in before uploading IFC models or managing building issues.',
    'Authorization.HeroTitle': 'Maintaining the future',
    'Authorization.Welcome': 'Welcome Back!',
    'Authorization.UserName': 'User Name',
    'Authorization.Password': 'Password',
    'Authorization.Required': 'Enter your username and password.',
    'Authorization.InvalidLogin': 'Username or password is incorrect, please try again.',
    'Authorization.SignIn': 'Sign In',
    'Authorization.SigningIn': 'Signing in...',
    'Authorization.Continue': 'Continue to Upload',
    'Authorization.Note': 'Authentication is handled by the ASP.NET Core backend.',
    'Upload.Title': 'Upload IFC Model',
    'Upload.Description': 'Upload an IFC file to process floors and generate browser-viewable model data.',
    'Upload.FileLabel': 'IFC file',
    'Upload.Button': 'Upload and Process',
    'Upload.Uploading': 'Uploading...',
    'Upload.SelectFile': 'Select an IFC file before uploading.',
    'Upload.OnlyIfc': 'Only IFC files are supported.',
    'Upload.Failed': 'Upload failed. Check the file and try again.',
    'Upload.Success': 'Model uploaded successfully.',
    'Viewer.Title': 'Building Viewer',
    'Viewer.Description': 'Select a model, inspect elements, manage issues, and control severity indicators.',
    'Viewer.Model': 'Model',
    'Viewer.SelectModel': 'Select model',
    'Viewer.LoadingModels': 'Loading models...',
    'Viewer.LoadingViewer': 'Loading viewer...',
    'Viewer.Selected': 'Selected',
    'Viewer.ZoomToSelected': 'Zoom to Selected',
    'Viewer.LoadModelsError': 'Could not load models from the API.',
    'Viewer.LoadFloorsError': 'Could not load floors for the selected model.',
    'Viewer.LoadHighlightsError': 'Could not load viewer highlight metadata.',
    'Viewer.InitError': 'The model viewer could not be initialized.',
    'Viewer.ScriptsUnavailable': 'XBIM viewer scripts are not available.',
    'Viewer.ScriptLoadError': 'Could not load viewer script.',
    'Viewer.ElementDetailsMissing': 'was selected, but details were not found in the API.',
    'Viewer.ModelPrefix': 'Model',
    'Element.Details': 'Element Details',
    'Element.Id': 'ID',
    'Element.Type': 'Type',
    'Element.Floor': 'Floor',
    'Element.Space': 'Space',
    'Element.Materials': 'Materials',
    'Common.NotAvailable': 'N/A',
    'Floors.Title': 'Floors',
    'Floors.Empty': 'No floors loaded.',
    'Indicators.Title': 'Issue Indicators',
    'Indicators.Highlight': 'Highlight issue elements',
    'Indicators.Flash': 'Flash highlighted elements',
    'Severity.Severe': 'Severe',
    'Severity.High': 'High',
    'Severity.Medium': 'Medium',
    'Severity.Low': 'Low',
    'Issues.Title': 'Issues',
    'Issues.SelectElement': 'Select a model element to view or create issues.',
    'Issues.Description': 'Description',
    'Issues.Severity': 'Severity',
    'Issues.ReporterId': 'Reporter ID',
    'Issues.Create': 'Create Issue',
    'Issues.Saving': 'Saving...',
    'Issues.Loading': 'Loading issues...',
    'Issues.Empty': 'No issues for this element.',
    'Issues.Status': 'Status',
    'Issues.AssignedTo': 'Assigned to',
    'Issues.ChooseWorker': 'Choose worker',
    'Issues.Assign': 'Assign',
    'Issues.Created': 'Issue created.',
    'Issues.Assigned': 'Issue assigned.',
    'Issues.CreateError': 'Could not create issue.',
    'Issues.AssignError': 'Could not assign worker.',
    'Issues.LoadError': 'Could not load issues for this element.',
    'Issues.DescriptionRequired': 'Select an element and enter an issue description.',
    'Issues.WorkerRequired': 'Choose a worker before assigning.'
  };

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
            translations: this.fallbackTranslations
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
    this.translationsSubject.next({ ...this.fallbackTranslations, ...(payload.translations ?? {}) });
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
