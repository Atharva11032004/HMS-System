import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface PricingDto {
  id: number;
  roomTypeId: number;
  roomTypeName: string;
  pricePerNight: number;
  effectiveDate: string;
}

export interface CreatePricingRequest {
  roomTypeId: number;
  roomTypeName: string;
  pricePerNight: number;
  effectiveDate: string;
}

@Injectable({ providedIn: 'root' })
export class PricingServiceApi {
  private base = '/api/pricing/pricings';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get h(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  // GET /pricings
  getAll(): Observable<PricingDto[]> {
    return this.http.get<PricingDto[]>(this.base, { headers: this.h });
  }

  // GET /pricings/{id}
  getById(id: number): Observable<PricingDto> {
    return this.http.get<PricingDto>(`${this.base}/${id}`, { headers: this.h });
  }

  // POST /pricings
  create(req: CreatePricingRequest): Observable<PricingDto> {
    return this.http.post<PricingDto>(this.base, req, { headers: this.h });
  }

  // PUT /pricings/{id}
  update(id: number, req: CreatePricingRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, req, { headers: this.h });
  }

  // DELETE /pricings/{id}
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`, { headers: this.h });
  }

  // GET /pricings/quote?roomTypeId=&checkIn=&checkOut=&guests=
  getQuote(roomTypeId: number, checkIn: string, checkOut: string, guests: number): Observable<number> {
    const params = new HttpParams()
      .set('roomTypeId', roomTypeId)
      .set('checkIn', checkIn)
      .set('checkOut', checkOut)
      .set('guests', guests);
    return this.http.get<number>(`${this.base}/quote`, { headers: this.h, params });
  }
}
