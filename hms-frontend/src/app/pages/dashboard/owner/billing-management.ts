import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BillingService, BillDto, CreateBillRequest, CreatePaymentRequest } from '../../../services/billing.service';
import { ReservationService, ReservationDto } from '../../../services/reservation.service';

@Component({
  selector: 'app-billing-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './billing-management.html',
  styleUrl: './billing-management.css'
})
export class BillingManagement implements OnInit {

  // ── Bills ──
  bills: BillDto[] = [];
  loading = false;

  // ── Reservations for dropdown ──
  reservations: ReservationDto[] = [];
  reservationsLoading = false;
  reservationSearch = '';
  filteredReservations: ReservationDto[] = [];
  selectedReservation: ReservationDto | null = null;
  showReservationDropdown = false;

  // ── Create Bill Modal ──
  showCreateBillModal = false;
  billLines: { description: string; amount: number }[] = [{ description: '', amount: 0 }];

  // ── Add Line Modal ──
  showAddLineModal = false;
  addingToBillId: number | null = null;
  lineForm = { description: '', amount: 0 };

  // ── Bill Detail Modal ──
  showDetailModal = false;
  detailBill: BillDto | null = null;

  // ── Payment Modal ──
  showPaymentModal = false;
  paymentForm: CreatePaymentRequest = { billId: 0, amount: 0, cardNumber: '' };

  successMsg = '';
  errorMsg = '';

  constructor(private svc: BillingService, private resSvc: ReservationService) {}

  ngOnInit() { this.loadBills(); }

  // ── Load Bills ──
  loadBills() {
    this.loading = true;
    this.svc.getAllBills().subscribe({
      next: b => { this.bills = b; this.loading = false; },
      error: (e: any) => {
        this.showError(e?.error?.title || e?.message || 'Failed to load bills');
        this.loading = false;
      }
    });
  }

  // ── Load Reservations for dropdown ──
  loadReservations() {
    this.reservationsLoading = true;
    this.resSvc.getAll().subscribe({
      next: r => {
        this.reservations = r;
        this.filteredReservations = r;
        this.reservationsLoading = false;
      },
      error: () => { this.reservationsLoading = false; }
    });
  }

  onReservationSearch() {
    const q = this.reservationSearch.toLowerCase();
    this.filteredReservations = q
      ? this.reservations.filter(r =>
          `${r.guestName} #${r.id} ${r.roomNumber}`.toLowerCase().includes(q))
      : this.reservations;
    this.showReservationDropdown = true;
    this.selectedReservation = null;
  }

  selectReservation(r: ReservationDto) {
    this.selectedReservation = r;
    this.reservationSearch = `#${r.id} — ${r.guestName} (Room ${r.roomNumber})`;
    this.showReservationDropdown = false;
  }

  // ── Create Bill ──
  openCreateBill() {
    this.selectedReservation = null;
    this.reservationSearch = '';
    this.billLines = [{ description: '', amount: 0 }];
    this.showCreateBillModal = true;
    this.loadReservations();
  }

  addLine() { this.billLines.push({ description: '', amount: 0 }); }

  removeLine(i: number) {
    if (this.billLines.length > 1) this.billLines.splice(i, 1);
  }

  get billFormTotal(): number {
    return this.billLines.reduce((s, l) => s + (+l.amount || 0), 0);
  }

  saveBill() {
    if (!this.selectedReservation) { this.showError('Please select a reservation'); return; }
    if (this.billLines.some(l => !l.description || !l.amount)) {
      this.showError('All line items need a description and amount'); return;
    }
    const req: CreateBillRequest = {
      reservationId: this.selectedReservation.id,
      lines: this.billLines.map(l => ({ description: l.description, amount: +l.amount }))
    };
    this.svc.createBill(req).subscribe({
      next: () => {
        this.showSuccess('Bill created successfully');
        this.showCreateBillModal = false;
        this.loadBills();
      },
      error: (e: any) => {
        const msg = e?.error?.errors
          ? Object.values(e.error.errors).flat().join(', ')
          : e?.error?.title || e?.error?.Error || e?.message || 'Failed to create bill';
        this.showError(msg);
      }
    });
  }

  // ── Add Line ──
  openAddLine(bill: BillDto) {
    this.addingToBillId = bill.id;
    this.lineForm = { description: '', amount: 0 };
    this.showAddLineModal = true;
  }

  saveAddLine() {
    if (!this.lineForm.description || !this.lineForm.amount) {
      this.showError('Description and amount required'); return;
    }
    this.svc.addBillLine(this.addingToBillId!, { description: this.lineForm.description, amount: +this.lineForm.amount }).subscribe({
      next: () => { this.showSuccess('Line added'); this.showAddLineModal = false; this.loadBills(); },
      error: (e: any) => this.showError(e?.error?.title || e?.message || 'Failed to add line')
    });
  }

  // ── View Detail ──
  openDetail(bill: BillDto) { this.detailBill = bill; this.showDetailModal = true; }

  // ── Payment ──
  openPayment(bill: BillDto) {
    this.paymentForm = { billId: bill.id, amount: bill.totalAmount, cardNumber: '' };
    this.showPaymentModal = true;
  }

  savePayment() {
    if (!this.paymentForm.cardNumber || !this.paymentForm.amount) {
      this.showError('Card number and amount required'); return;
    }
    this.svc.createPayment(this.paymentForm).subscribe({
      next: () => { this.showSuccess('Payment recorded successfully'); this.showPaymentModal = false; },
      error: (e: any) => this.showError(e?.error?.title || e?.message || 'Failed to record payment')
    });
  }

  // ── Stats ──
  get totalRevenue(): number { return this.bills.reduce((s, b) => s + b.totalAmount, 0); }
  get totalLines(): number   { return this.bills.reduce((s, b) => s + b.lines.length, 0); }

  // ── Helpers ──
  getReservationLabel(reservationId: number): string {
    const r = this.reservations.find(x => x.id === reservationId);
    return r ? `${r.guestName} (Room ${r.roomNumber})` : `Res #${reservationId}`;
  }

  showSuccess(msg: string) { this.successMsg = msg; this.errorMsg = ''; setTimeout(() => this.successMsg = '', 3000); }
  showError(msg: string)   { this.errorMsg = msg; setTimeout(() => this.errorMsg = '', 6000); }
}
