import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from './auth';

export interface StaffDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  departmentId: number;
  departmentName: string;
  role: string;
  hireDate: string;
  isActive: boolean;
}

export interface DepartmentDto {
  id: number;
  name: string;
  description?: string;
  staffCount: number;
}

export interface CreateStaffRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  departmentId: number;
  role: string;
}

export interface UpdateStaffRequest extends CreateStaffRequest {
  isActive: boolean;
}

export interface CreateDepartmentRequest {
  name: string;
  description?: string;
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class StaffService {
  private staffBase = '/api/staff';
  private deptBase  = '/api/departments';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private headers(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  getAllStaff(): Observable<StaffDto[]> {
    return this.http.get<ApiResponse<StaffDto[]>>(this.staffBase, { headers: this.headers() })
      .pipe(map(r => r.data));
  }

  getStaff(id: number): Observable<StaffDto> {
    return this.http.get<ApiResponse<StaffDto>>(`${this.staffBase}/${id}`, { headers: this.headers() })
      .pipe(map(r => r.data));
  }

  createStaff(req: CreateStaffRequest): Observable<StaffDto> {
    return this.http.post<ApiResponse<StaffDto>>(this.staffBase, req, { headers: this.headers() })
      .pipe(map(r => r.data));
  }

  updateStaff(id: number, req: UpdateStaffRequest): Observable<StaffDto> {
    return this.http.put<ApiResponse<StaffDto>>(`${this.staffBase}/${id}`, req, { headers: this.headers() })
      .pipe(map(r => r.data));
  }

  deleteStaff(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<boolean>>(`${this.staffBase}/${id}`, { headers: this.headers() })
      .pipe(map(r => r.success));
  }

  getAllDepartments(): Observable<DepartmentDto[]> {
    return this.http.get<ApiResponse<DepartmentDto[]>>(this.deptBase, { headers: this.headers() })
      .pipe(map(r => r.data));
  }

  createDepartment(req: CreateDepartmentRequest): Observable<DepartmentDto> {
    return this.http.post<ApiResponse<DepartmentDto>>(this.deptBase, req, { headers: this.headers() })
      .pipe(map(r => r.data));
  }
}
