import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';

const API_URL = 'http://localhost:8080/api/auth';

export interface LoginResponse {
  message: string;
  token: string;
}

export interface RegisterPayload {
  name: string;
  firstName: string;
  email: string;
  password: string;
}

export interface User {
  name?: string;
  firstName?: string;
  email: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private authInitializedSubject = new BehaviorSubject<boolean>(false);

  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  public currentUser$ = this.currentUserSubject.asObservable();
  public authInitialized$ = this.authInitializedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.checkTokenOnInit();
  }

  private checkTokenOnInit(): void {
    const token = this.getToken();
    this.isAuthenticatedSubject.next(!!token);
    this.authInitializedSubject.next(true);

    if (token) {
      setTimeout(() => {
        this.refreshUserInfo().subscribe();
      }, 0);
    }
  }

  refreshUserInfo(): Observable<User | null> {
    if (!this.getToken()) {
      this.currentUserSubject.next(null);
      return of(null);
    }

    return this.getUserInfo().pipe(
      tap((user) => {
        this.currentUserSubject.next(user);
      }),
      catchError((error) => {
        console.error('Error fetching user info:', error);
        if (error.status === 401) {
          this.logout();
        }
        return of(null);
      })
    );
  }

  registerUser(payload: RegisterPayload): Observable<any> {
    return this.http.post(`${API_URL}/register`, payload).pipe(
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.isAuthenticatedSubject.next(true);
          this.refreshUserInfo().subscribe();
        }
      })
    );
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_URL}/login`, { email, password })
      .pipe(
        tap((response) => {
          localStorage.setItem('token', response.token);
          this.isAuthenticatedSubject.next(true);
          this.refreshUserInfo().subscribe();
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.isAuthenticatedSubject.next(false);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getUserInfo(): Observable<User> {
    return this.http.get<User>(`${API_URL}/me`);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isInitialized(): boolean {
    return this.authInitializedSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }
}
