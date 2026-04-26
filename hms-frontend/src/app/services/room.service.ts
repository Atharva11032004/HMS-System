import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface RoomTypeDto {
  id: number;
  name: string;
  description: string;
  maxOccupancy: number;
}

export interface RoomDto {
  id: number;
  roomNumber: string;
  roomTypeName: string;
  isAvailable: boolean;
}

export interface CreateRoomRequest {
  roomNumber: string;
  roomTypeId: number;
  isAvailable: boolean;
}

export interface CreateRoomTypeRequest {
  name: string;
  description: string;
  maxOccupancy: number;
}

export interface BlockRequest {
  roomId: number;
  checkIn: string;
  checkOut: string;
}

@Injectable({ providedIn: 'root' })
export class RoomService {
  private roomBase     = '/api/room/rooms';
  private roomTypeBase = '/api/room/roomtypes';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get h(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  // ── Rooms ──
  getRooms(): Observable<RoomDto[]> {
    return this.http.get<RoomDto[]>(this.roomBase, { headers: this.h });
  }

  getRoom(id: number): Observable<RoomDto> {
    return this.http.get<RoomDto>(`${this.roomBase}/${id}`, { headers: this.h });
  }

  createRoom(req: CreateRoomRequest): Observable<RoomDto> {
    return this.http.post<RoomDto>(this.roomBase, req, { headers: this.h });
  }

  updateRoom(id: number, req: CreateRoomRequest): Observable<void> {
    return this.http.put<void>(`${this.roomBase}/${id}`, req, { headers: this.h });
  }

  deleteRoom(id: number): Observable<void> {
    return this.http.delete<void>(`${this.roomBase}/${id}`, { headers: this.h });
  }

  getAvailableRooms(checkIn: string, checkOut: string, adults: number, children: number): Observable<{ availableRooms: RoomDto[] }> {
    const params = new HttpParams()
      .set('checkIn', checkIn).set('checkOut', checkOut)
      .set('adults', adults).set('children', children);
    return this.http.get<{ availableRooms: RoomDto[] }>(`${this.roomBase}/available`, { headers: this.h, params });
  }

  blockRoom(req: BlockRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.roomBase}/block`, req, { headers: this.h });
  }

  freeRoom(roomId: number): Observable<void> {
    return this.http.post<void>(`${this.roomBase}/free`, roomId, { headers: this.h });
  }

  // ── Room Types ──
  getRoomTypes(): Observable<RoomTypeDto[]> {
    return this.http.get<RoomTypeDto[]>(this.roomTypeBase, { headers: this.h });
  }

  getRoomType(id: number): Observable<RoomTypeDto> {
    return this.http.get<RoomTypeDto>(`${this.roomTypeBase}/${id}`, { headers: this.h });
  }

  createRoomType(req: CreateRoomTypeRequest): Observable<RoomTypeDto> {
    return this.http.post<RoomTypeDto>(this.roomTypeBase, req, { headers: this.h });
  }

  updateRoomType(id: number, req: CreateRoomTypeRequest): Observable<void> {
    return this.http.put<void>(`${this.roomTypeBase}/${id}`, req, { headers: this.h });
  }

  deleteRoomType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.roomTypeBase}/${id}`, { headers: this.h });
  }
}
