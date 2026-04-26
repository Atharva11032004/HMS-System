import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, forkJoin, map } from 'rxjs';
import { AuthService } from './auth';

export interface ReservationDto {
  id: number;
  guestId: number;
  guestName: string;
  roomId: number;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  totalAmount: number;
  status: string;
}

export interface CreateReservationRequest {
  guestId: number;
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
}

export interface AvailableRoomDto {
  id: number;
  roomNumber: string;
  roomTypeName: string;
  price: number;
}

export interface GuestOption {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

export interface RoomOption {
  id: number;
  roomNumber: string;
  roomTypeName: string;
  isAvailable: boolean;
}

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private resBase     = '/api/reservation/reservations';
  private guestBase   = '/api/guest/guests';
  private roomBase    = '/api/room/rooms';
  private pricingBase = '/api/pricing/pricings';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get headers(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  getAll(guestId?: number, from?: string, to?: string): Observable<ReservationDto[]> {
    let params = new HttpParams();
    if (guestId) params = params.set('guestId', guestId);
    if (from)    params = params.set('from', from);
    if (to)      params = params.set('to', to);
    return this.http.get<ReservationDto[]>(this.resBase, { headers: this.headers, params });
  }

  create(req: CreateReservationRequest): Observable<ReservationDto> {
    return this.http.post<ReservationDto>(this.resBase, req, { headers: this.headers });
  }

  cancel(id: number): Observable<void> {
    return this.http.post<void>(`${this.resBase}/${id}/cancel`, {}, { headers: this.headers });
  }

  // Call RoomService /rooms/available directly (source of truth)
  // then merge price from PricingService /pricings by roomTypeName
  getAvailability(checkIn: string, checkOut: string, adults: number, children: number): Observable<{ availableRooms: AvailableRoomDto[] }> {
    const params = new HttpParams()
      .set('checkIn', checkIn)
      .set('checkOut', checkOut)
      .set('adults', adults)
      .set('children', children);

    const rooms$ = this.http.get<any>(`${this.roomBase}/available`, { headers: this.headers, params });
    const pricings$ = this.http.get<any[]>(this.pricingBase, { headers: this.headers });

    return forkJoin({ rooms: rooms$, pricings: pricings$ }).pipe(
      map(({ rooms, pricings }) => {
        // RoomService returns { AvailableRooms: [...] } — PascalCase from .NET
        const roomList: any[] = rooms?.AvailableRooms ?? rooms?.availableRooms ?? [];

        // Build price map: roomTypeName (lowercase) → pricePerNight
        const priceMap = new Map<string, number>();
        (pricings ?? []).forEach((p: any) => {
          const key = (p.roomTypeName ?? p.RoomTypeName ?? '').toLowerCase();
          if (key) priceMap.set(key, p.pricePerNight ?? p.PricePerNight ?? 0);
        });

        const availableRooms: AvailableRoomDto[] = roomList.map((r: any) => ({
          id: r.id ?? r.Id,
          roomNumber: r.roomNumber ?? r.RoomNumber,
          roomTypeName: r.roomTypeName ?? r.RoomTypeName,
          price: priceMap.get((r.roomTypeName ?? r.RoomTypeName ?? '').toLowerCase()) ?? 0
        }));

        return { availableRooms };
      })
    );
  }

  getGuests(): Observable<GuestOption[]> {
    return this.http.get<GuestOption[]>(this.guestBase, { headers: this.headers });
  }

  getRooms(): Observable<RoomOption[]> {
    return this.http.get<RoomOption[]>(this.roomBase, { headers: this.headers });
  }
}
