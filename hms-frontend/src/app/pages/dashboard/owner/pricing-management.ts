import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PricingServiceApi, PricingDto, CreatePricingRequest } from '../../../services/pricing.service';

type Tab = 'pricings' | 'quote';

@Component({
  selector: 'app-pricing-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pricing-management.html',
  styleUrl: './pricing-management.css'
})
export class PricingManagement implements OnInit {
  activeTab: Tab = 'pricings';

  pricings: PricingDto[] = [];
  // Use room types extracted from existing pricings (PricingService's own RoomTypes)
  roomTypes: { id: number; name: string }[] = [];
  loading = false;

  // Create / Edit modal
  showModal = false;
  editingPricing: PricingDto | null = null;
  form: CreatePricingRequest = { roomTypeId: 0, roomTypeName: '', pricePerNight: 0, effectiveDate: '' };

  // Delete confirm
  showDeleteConfirm = false;
  deletingId: number | null = null;

  // Quote calculator
  quoteForm = { roomTypeId: 0, checkIn: '', checkOut: '', guests: 1 };
  quoteResult: number | null = null;
  quoteLoading = false;
  quoteNights = 0;

  successMsg = '';
  errorMsg   = '';

  constructor(private svc: PricingServiceApi) {}

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: p => {
        this.pricings = p;
        // Build room types from existing pricings — these are PricingService's own RoomType IDs
        const seen = new Set<number>();
        this.roomTypes = p
          .filter(x => { if (seen.has(x.roomTypeId)) return false; seen.add(x.roomTypeId); return true; })
          .map(x => ({ id: x.roomTypeId, name: x.roomTypeName }));
        this.loading = false;
      },
      error: (e: any) => { this.showError(e?.error?.title || e?.message || 'Failed to load pricings'); this.loading = false; }
    });
  }

  // ── Create / Edit ──
  openAdd() {
    this.editingPricing = null;
    this.form = {
      roomTypeId: this.roomTypes[0]?.id ?? 0,
      roomTypeName: this.roomTypes[0]?.name ?? '',
      pricePerNight: 0,
      effectiveDate: new Date().toISOString().split('T')[0]
    };
    this.showModal = true;
  }

  openEdit(p: PricingDto) {
    this.editingPricing = p;
    this.form = {
      roomTypeId: p.roomTypeId,
      roomTypeName: p.roomTypeName,
      pricePerNight: p.pricePerNight,
      effectiveDate: p.effectiveDate.split('T')[0]
    };
    this.showModal = true;
  }

  onRoomTypeChange() {
    const rt = this.roomTypes.find(t => t.id === +this.form.roomTypeId);
    if (rt) this.form.roomTypeName = rt.name;
  }

  save() {
    if (!this.form.roomTypeId || !this.form.pricePerNight || !this.form.effectiveDate) {
      this.showError('Room type, price and effective date are required'); return;
    }
    const req: CreatePricingRequest = {
      ...this.form,
      roomTypeId: +this.form.roomTypeId,
      pricePerNight: +this.form.pricePerNight,
      effectiveDate: new Date(this.form.effectiveDate).toISOString()
    };
    if (this.editingPricing) {
      this.svc.update(this.editingPricing.id, req).subscribe({
        next: () => { this.showSuccess('Pricing updated'); this.showModal = false; this.loadAll(); },
        error: (e: any) => this.showError(e?.error?.title || 'Failed to update pricing')
      });
    } else {
      this.svc.create(req).subscribe({
        next: () => { this.showSuccess('Pricing created'); this.showModal = false; this.loadAll(); },
        error: (e: any) => this.showError(e?.error?.title || 'Failed to create pricing')
      });
    }
  }

  // ── Delete ──
  confirmDelete(id: number) { this.deletingId = id; this.showDeleteConfirm = true; }

  deletePricing() {
    if (this.deletingId == null) return;
    this.svc.delete(this.deletingId).subscribe({
      next: () => { this.showSuccess('Pricing deleted'); this.showDeleteConfirm = false; this.loadAll(); },
      error: (e: any) => this.showError(e?.error?.title || 'Failed to delete pricing')
    });
  }

  // ── Quote ──
  calcNights() {
    if (!this.quoteForm.checkIn || !this.quoteForm.checkOut) { this.quoteNights = 0; return; }
    this.quoteNights = Math.max(0, Math.round(
      (new Date(this.quoteForm.checkOut).getTime() - new Date(this.quoteForm.checkIn).getTime()) / 86400000
    ));
  }

  getQuote() {
    if (!this.quoteForm.roomTypeId || !this.quoteForm.checkIn || !this.quoteForm.checkOut) {
      this.showError('Room type, check-in and check-out are required'); return;
    }
    this.quoteLoading = true;
    this.quoteResult = null;
    this.svc.getQuote(
      +this.quoteForm.roomTypeId,
      new Date(this.quoteForm.checkIn).toISOString(),
      new Date(this.quoteForm.checkOut).toISOString(),
      this.quoteForm.guests
    ).subscribe({
      next: q => { this.quoteResult = q; this.quoteLoading = false; },
      error: (e: any) => { this.showError(e?.error?.title || 'Failed to get quote'); this.quoteLoading = false; }
    });
  }

  // ── Stats ──
  get avgPrice(): number {
    if (!this.pricings.length) return 0;
    return this.pricings.reduce((s, p) => s + p.pricePerNight, 0) / this.pricings.length;
  }

  get maxPrice(): number { return this.pricings.length ? Math.max(...this.pricings.map(p => p.pricePerNight)) : 0; }
  get minPrice(): number { return this.pricings.length ? Math.min(...this.pricings.map(p => p.pricePerNight)) : 0; }

  showSuccess(msg: string) { this.successMsg = msg; this.errorMsg = ''; setTimeout(() => this.successMsg = '', 3000); }
  showError(msg: string)   { this.errorMsg = msg; setTimeout(() => this.errorMsg = '', 6000); }
}
