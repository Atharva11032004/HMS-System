import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface BillLineDto {
  id: number;
  description: string;
  amount: number;
}

export interface BillDto {
  id: number;
  reservationId: number;
  totalAmount: number;
  createdAt: string;
  lines: BillLineDto[];
}

export interface CreateBillRequest {
  reservationId: number;
  lines: { description: string; amount: number }[];
}

export interface CreatePaymentRequest {
  billId: number;
  amount: number;
  cardNumber: string;
}

@Injectable({ providedIn: 'root' })
export class BillingService {
  private billBase    = '/api/billing/bills';
  private paymentBase = '/api/billing/payments';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get h(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  // GET /bills
  getAllBills(): Observable<BillDto[]> {
    return this.http.get<BillDto[]>(this.billBase, { headers: this.h });
  }

  // GET /bills/{id}
  getBill(id: number): Observable<BillDto> {
    return this.http.get<BillDto>(`${this.billBase}/${id}`, { headers: this.h });
  }

  // POST /bills
  createBill(req: CreateBillRequest): Observable<BillDto> {
    return this.http.post<BillDto>(this.billBase, req, { headers: this.h });
  }

  // POST /bills/{id}/lines
  addBillLine(billId: number, line: { description: string; amount: number }): Observable<void> {
    return this.http.post<void>(`${this.billBase}/${billId}/lines`, line, { headers: this.h });
  }

  // POST /payments
  createPayment(req: CreatePaymentRequest): Observable<void> {
    return this.http.post<void>(this.paymentBase, req, { headers: this.h });
  }
}
