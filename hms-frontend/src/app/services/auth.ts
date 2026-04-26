import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; role: string; }
export interface AuthResponse { token: string; refreshToken: string; expiration: string; }
export interface UserDto { id: string; email: string; role: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly base = '/api/identity/auth';

  constructor(private http: HttpClient, private router: Router) {}

  private get authHeaders(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.getToken()}` });
  }

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/login`, payload).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  register(payload: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/register`, payload);
  }

  // Used by Owner/Manager to create users — sends JWT
  createUser(payload: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/register`, payload, { headers: this.authHeaders });
  }

  getUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.base}/users`, { headers: this.authHeaders });
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/users/${id}`, { headers: this.authHeaders });
  }

  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/sign-in']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // .NET identity uses this long claim URI for roles
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        ?? payload['role']
        ?? null;
    } catch { return null; }
  }

  redirectToDashboard(): void {
    const role = this.getRole()?.toLowerCase();
    if (role === 'owner')        this.router.navigate(['/dashboard/owner']);
    else if (role === 'manager') this.router.navigate(['/dashboard/manager']);
    else                         this.router.navigate(['/dashboard/receptionist']);
  }
}
