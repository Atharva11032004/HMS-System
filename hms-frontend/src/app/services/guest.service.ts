import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface GuestDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

export interface CreateGuestRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

@Injectable({ providedIn: 'root' })
export class GuestServiceApi {
  private base = '/api/guest/guests';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get h(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  // GET /guests
  getAll(): Observable<GuestDto[]> {
    return this.http.get<GuestDto[]>(this.base, { headers: this.h });
  }

  // GET /guests/{id}
  getById(id: number): Observable<GuestDto> {
    return this.http.get<GuestDto>(`${this.base}/${id}`, { headers: this.h });
  }

  // POST /guests
  create(req: CreateGuestRequest): Observable<GuestDto> {
    return this.http.post<GuestDto>(this.base, req, { headers: this.h });
  }

  // PUT /guests/{id}
  update(id: number, req: CreateGuestRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, req, { headers: this.h });
  }

  // DELETE /guests/{id}
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`, { headers: this.h });
  }

  // GET /guests/search?email=&phone=&name=
  search(email?: string, phone?: string, name?: string): Observable<GuestDto[]> {
    let params = new HttpParams();
    if (email) params = params.set('email', email);
    if (phone) params = params.set('phone', phone);
    if (name)  params = params.set('name', name);
    return this.http.get<GuestDto[]>(`${this.base}/search`, { headers: this.h, params });
  }
}
