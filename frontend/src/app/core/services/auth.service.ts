import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AuthUser, LoginRequest } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/auth`;
  private readonly currentUserSubject = new BehaviorSubject<AuthUser | null>(null);
  private loaded = false;

  readonly currentUser$ = this.currentUserSubject.asObservable();

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest): Observable<AuthUser> {
    return this.http.post<ApiResponse<AuthUser>>(`${this.baseUrl}/login`, request).pipe(
      map(response => response.data),
      tap(user => {
        this.loaded = true;
        this.currentUserSubject.next(user);
      })
    );
  }

  logout(): Observable<void> {
    return this.http.post<ApiResponse<object>>(`${this.baseUrl}/logout`, {}).pipe(
      tap(() => {
        this.loaded = true;
        this.currentUserSubject.next(null);
      }),
      map(() => undefined)
    );
  }

  loadCurrentUser(): Observable<AuthUser | null> {
    if (this.loaded) {
      return of(this.currentUserSubject.value);
    }

    return this.http.get<ApiResponse<AuthUser>>(`${this.baseUrl}/me`).pipe(
      map(response => response.data),
      tap(user => {
        this.loaded = true;
        this.currentUserSubject.next(user);
      }),
      catchError(() => {
        this.loaded = true;
        this.currentUserSubject.next(null);
        return of(null);
      })
    );
  }
}
