import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GuestServiceApi, GuestDto, CreateGuestRequest } from '../../../services/guest.service';

@Component({
  selector: 'app-guest-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './guest-management.html',
  styleUrl: './guest-management.css'
})
export class GuestManagement implements OnInit {

  guests: GuestDto[] = [];
  filtered: GuestDto[] = [];
  loading = false;

  // Search
  searchName  = '';
  searchEmail = '';
  searchPhone = '';
  searching   = false;

  // Create / Edit modal
  showModal   = false;
  editingGuest: GuestDto | null = null;
  form: CreateGuestRequest = { firstName: '', lastName: '', email: '', phone: '' };

  // Delete confirm
  showDeleteConfirm = false;
  deletingId: number | null = null;

  // View detail
  showDetailModal = false;
  detailGuest: GuestDto | null = null;

  successMsg = '';
  errorMsg   = '';

  constructor(private svc: GuestServiceApi) {}

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: g => { this.guests = g; this.filtered = g; this.loading = false; },
      error: (e: any) => { this.showError(e?.error?.title || e?.message || 'Failed to load guests'); this.loading = false; }
    });
  }

  // ── Search ──
  onSearch() {
    const hasQuery = this.searchName || this.searchEmail || this.searchPhone;
    if (!hasQuery) { this.filtered = this.guests; return; }
    this.searching = true;
    this.svc.search(
      this.searchEmail || undefined,
      this.searchPhone || undefined,
      this.searchName  || undefined
    ).subscribe({
      next: g => { this.filtered = g; this.searching = false; },
      error: (e: any) => { this.showError(e?.error?.title || 'Search failed'); this.searching = false; }
    });
  }

  clearSearch() {
    this.searchName = ''; this.searchEmail = ''; this.searchPhone = '';
    this.filtered = this.guests;
  }

  // ── Create / Edit ──
  openAdd() {
    this.editingGuest = null;
    this.form = { firstName: '', lastName: '', email: '', phone: '' };
    this.showModal = true;
  }

  openEdit(g: GuestDto) {
    this.editingGuest = g;
    this.form = { firstName: g.firstName, lastName: g.lastName, email: g.email, phone: g.phone };
    this.showModal = true;
  }

  save() {
    if (!this.form.firstName || !this.form.lastName || !this.form.email) {
      this.showError('First name, last name and email are required'); return;
    }
    if (this.editingGuest) {
      this.svc.update(this.editingGuest.id, this.form).subscribe({
        next: () => { this.showSuccess('Guest updated'); this.showModal = false; this.loadAll(); },
        error: (e: any) => this.showError(e?.error?.title || e?.message || 'Failed to update guest')
      });
    } else {
      this.svc.create(this.form).subscribe({
        next: () => { this.showSuccess('Guest created'); this.showModal = false; this.loadAll(); },
        error: (e: any) => this.showError(e?.error?.title || e?.message || 'Failed to create guest')
      });
    }
  }

  // ── Delete ──
  confirmDelete(id: number) { this.deletingId = id; this.showDeleteConfirm = true; }

  deleteGuest() {
    if (this.deletingId == null) return;
    this.svc.delete(this.deletingId).subscribe({
      next: () => { this.showSuccess('Guest deleted'); this.showDeleteConfirm = false; this.loadAll(); },
      error: (e: any) => this.showError(e?.error?.title || 'Failed to delete guest')
    });
  }

  // ── View Detail ──
  openDetail(g: GuestDto) { this.detailGuest = g; this.showDetailModal = true; }

  // ── Stats ──
  get totalGuests() { return this.guests.length; }

  initials(g: GuestDto) { return `${g.firstName[0] ?? ''}${g.lastName[0] ?? ''}`.toUpperCase(); }
  avatarColor(id: number) { return `hsl(${(id * 53) % 360}, 55%, 55%)`; }

  showSuccess(msg: string) { this.successMsg = msg; this.errorMsg = ''; setTimeout(() => this.successMsg = '', 3000); }
  showError(msg: string)   { this.errorMsg = msg; setTimeout(() => this.errorMsg = '', 6000); }
}
