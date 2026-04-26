import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { AuthService } from './auth';

export interface InventoryItemDto {
  id: number;
  name: string;
  description?: string;
  category: string;
  supplier: string;
  quantityInStock: number;
  minimumStockLevel: number;
  unitPrice: number;
  lastRestocked: string;
  isActive: boolean;
  isLowStock: boolean;
}

export interface CreateInventoryItemRequest {
  name: string;
  description?: string;
  category: string;
  supplier: string;
  quantityInStock: number;
  minimumStockLevel: number;
  unitPrice: number;
}

export interface UpdateInventoryItemRequest {
  name: string;
  description?: string;
  category: string;
  supplier: string;
  minimumStockLevel: number;
  unitPrice: number;
  isActive: boolean;
}

export interface AdjustStockRequest {
  quantityAdjustment: number;
  reason?: string;
}

interface ApiResponse<T> { success: boolean; message: string; data: T; }

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private base = '/api/inventory';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get h(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  private unwrap<T>(obs: Observable<ApiResponse<T>>): Observable<T> {
    return obs.pipe(map(r => r.data));
  }

  // GET /api/inventory
  getAll(): Observable<InventoryItemDto[]> {
    return this.unwrap(this.http.get<ApiResponse<InventoryItemDto[]>>(this.base, { headers: this.h }));
  }

  // GET /api/inventory/{id}
  getById(id: number): Observable<InventoryItemDto> {
    return this.unwrap(this.http.get<ApiResponse<InventoryItemDto>>(`${this.base}/${id}`, { headers: this.h }));
  }

  // POST /api/inventory
  create(req: CreateInventoryItemRequest): Observable<InventoryItemDto> {
    return this.unwrap(this.http.post<ApiResponse<InventoryItemDto>>(this.base, req, { headers: this.h }));
  }

  // PUT /api/inventory/{id}
  update(id: number, req: UpdateInventoryItemRequest): Observable<InventoryItemDto> {
    return this.unwrap(this.http.put<ApiResponse<InventoryItemDto>>(`${this.base}/${id}`, req, { headers: this.h }));
  }

  // DELETE /api/inventory/{id}
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`, { headers: this.h });
  }

  // POST /api/inventory/{id}/adjust-stock
  adjustStock(id: number, req: AdjustStockRequest): Observable<InventoryItemDto> {
    return this.unwrap(this.http.post<ApiResponse<InventoryItemDto>>(`${this.base}/${id}/adjust-stock`, req, { headers: this.h }));
  }

  // GET /api/inventory/low-stock
  getLowStock(): Observable<InventoryItemDto[]> {
    return this.unwrap(this.http.get<ApiResponse<InventoryItemDto[]>>(`${this.base}/low-stock`, { headers: this.h }));
  }
}
