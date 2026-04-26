import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ReservationService, ReservationDto, AvailableRoomDto, GuestOption, RoomOption
} from '../../../services/reservation.service';

@Component({
  selector: 'app-reservation-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reservation-management.html',
  styleUrl: './reservation-management.css'
})
export class ReservationManagement implements OnInit {
  activeTab: 'reservations' | 'availability' = 'reservations';
  reservations: ReservationDto[] = [];
  filtered: ReservationDto[] = [];
  availableRooms: AvailableRoomDto[] = [];

  loading = false;
  availLoading = false;
  successMsg = '';
  error = '';

  searchQuery = '';
  filterStatus = '';
  filterFrom = '';
  filterTo = '';

  showCreateModal = false;
  showCancelConfirm = false;
  cancellingId: number | null = null;
  availSearchDone = false;

  availForm = { checkIn: '', checkOut: '', adults: 1, children: 0 };

  // Create form
  createForm = { checkInDate: '', checkOutDate: '' };
  selectedGuest: GuestOption | null = null;
  selectedRoom: RoomOption | AvailableRoomDto | null = null;

  // Guest dropdown
  guestSearch = '';
  guestResults: GuestOption[] = [];
  allGuests: GuestOption[] = [];
  showGuestDropdown = false;
  guestsLoading = false;

  // Room dropdown
  roomSearch = '';
  roomResults: RoomOption[] = [];
  allRooms: RoomOption[] = [];
  showRoomDropdown = false;
  roomsLoading = false;

  constructor(private svc: ReservationService) {}

  ngOnInit() { this.loadReservations(); }

  loadReservations() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: r => { this.reservations = r; this.applyFilters(); this.loading = false; },
      error: () => { this.error = 'Failed to load reservations'; this.loading = false; }
    });
  }

  applyFilters() {
    const q = this.searchQuery.toLowerCase();
    this.filtered = this.reservations.filter(r => {
      const matchSearch = !q || `${r.guestName} ${r.roomNumber} ${r.id}`.toLowerCase().includes(q);
      const matchStatus = !this.filterStatus || r.status === this.filterStatus;
      const matchFrom = !this.filterFrom || new Date(r.checkInDate) >= new Date(this.filterFrom);
      const matchTo = !this.filterTo || new Date(r.checkOutDate) <= new Date(this.filterTo);
      return matchSearch && matchStatus && matchFrom && matchTo;
    });
  }

  // ── Availability ──
  searchAvailability() {
    if (!this.availForm.checkIn || !this.availForm.checkOut) return;
    this.availLoading = true;
    this.availSearchDone = false;
    const checkIn = new Date(this.availForm.checkIn).toISOString();
    const checkOut = new Date(this.availForm.checkOut).toISOString();
    this.svc.getAvailability(checkIn, checkOut, this.availForm.adults, this.availForm.children).subscribe({
      next: res => {
        this.availableRooms = res.availableRooms ?? (res as any).AvailableRooms ?? [];
        this.availLoading = false;
        this.availSearchDone = true;
      },
      error: () => { this.error = 'Failed to fetch availability'; this.availLoading = false; }
    });
  }

  openCreateFromRoom(room: AvailableRoomDto) {
    this.resetCreateForm();
    this.selectedRoom = room;
    this.roomSearch = `Room ${room.roomNumber} — ${room.roomTypeName}`;
    this.createForm.checkInDate = this.availForm.checkIn;
    this.createForm.checkOutDate = this.availForm.checkOut;
    this.showCreateModal = true;
    this.loadGuestsForModal();
  }

  openCreateModal() {
    this.resetCreateForm();
    this.showCreateModal = true;
    this.loadGuestsForModal();
    this.loadRoomsForModal();
  }

  loadGuestsForModal() {
    this.guestsLoading = true;
    this.svc.getGuests().subscribe({
      next: g => { this.allGuests = g; this.guestResults = g; this.guestsLoading = false; },
      error: () => { this.guestsLoading = false; }
    });
  }

  loadRoomsForModal() {
    // Only load available rooms based on selected dates
    if (this.createForm.checkInDate && this.createForm.checkOutDate) {
      this.roomsLoading = true;
      const checkIn = new Date(this.createForm.checkInDate).toISOString();
      const checkOut = new Date(this.createForm.checkOutDate).toISOString();
      this.svc.getAvailability(checkIn, checkOut, 1, 0).subscribe({
        next: res => {
          const available = res.availableRooms ?? [];
          this.allRooms = available.map(r => ({ id: r.id, roomNumber: r.roomNumber, roomTypeName: r.roomTypeName, isAvailable: true }));
          this.roomResults = this.allRooms;
          this.roomsLoading = false;
        },
        error: () => {
          // fallback to all rooms
          this.svc.getRooms().subscribe({
            next: r => { this.allRooms = r; this.roomResults = r; this.roomsLoading = false; },
            error: () => { this.roomsLoading = false; }
          });
        }
      });
    } else {
      this.roomsLoading = true;
      this.svc.getRooms().subscribe({
        next: r => { this.allRooms = r; this.roomResults = r; this.roomsLoading = false; },
        error: () => { this.roomsLoading = false; }
      });
    }
  }

  // ── Guest dropdown ──
  onGuestSearch() {
    const q = this.guestSearch.toLowerCase();
    this.guestResults = q
      ? this.allGuests.filter(g => `${g.firstName} ${g.lastName} ${g.email}`.toLowerCase().includes(q))
      : this.allGuests;
    this.showGuestDropdown = true;
    if (this.selectedGuest) this.selectedGuest = null;
  }

  selectGuest(g: GuestOption) {
    this.selectedGuest = g;
    this.guestSearch = `${g.firstName} ${g.lastName}`;
    this.showGuestDropdown = false;
  }

  onModalDateChange() {
    // Reset room selection and reload available rooms for new dates
    this.selectedRoom = null;
    this.roomSearch = '';
    this.roomResults = [];
    this.loadRoomsForModal();
  }

  // ── Room dropdown ──
  onRoomSearch() {
    const q = this.roomSearch.toLowerCase();
    this.roomResults = q
      ? this.allRooms.filter(r => `${r.roomNumber} ${r.roomTypeName}`.toLowerCase().includes(q))
      : this.allRooms;
    this.showRoomDropdown = true;
    if (this.selectedRoom) this.selectedRoom = null;
  }

  selectRoom(r: RoomOption) {
    this.selectedRoom = r;
    this.roomSearch = `Room ${r.roomNumber} — ${r.roomTypeName}`;
    this.showRoomDropdown = false;
  }

  saveReservation() {
    if (!this.selectedGuest || !this.selectedRoom || !this.createForm.checkInDate || !this.createForm.checkOutDate) {
      this.error = 'Please fill all required fields';
      return;
    }
    this.svc.create({
      guestId: this.selectedGuest.id,
      roomId: this.selectedRoom.id,
      checkInDate: new Date(this.createForm.checkInDate).toISOString(),
      checkOutDate: new Date(this.createForm.checkOutDate).toISOString()
    }).subscribe({
      next: () => { this.showSuccess('Reservation created successfully'); this.showCreateModal = false; this.loadReservations(); },
      error: (e) => this.error = e?.error?.Error || e?.error?.title || 'Failed to create reservation'
    });
  }

  confirmCancel(id: number) { this.cancellingId = id; this.showCancelConfirm = true; }

  cancelReservation() {
    if (this.cancellingId == null) return;
    this.svc.cancel(this.cancellingId).subscribe({
      next: () => { this.showSuccess('Reservation cancelled'); this.showCancelConfirm = false; this.cancellingId = null; this.loadReservations(); },
      error: () => this.error = 'Failed to cancel reservation'
    });
  }

  resetCreateForm() {
    this.createForm = { checkInDate: '', checkOutDate: '' };
    this.selectedGuest = null;
    this.selectedRoom = null;
    this.guestSearch = '';
    this.roomSearch = '';
    this.guestResults = [];
    this.roomResults = [];
    this.showGuestDropdown = false;
    this.showRoomDropdown = false;
  }

  closeCreateModal() { this.showCreateModal = false; this.resetCreateForm(); }

  showSuccess(msg: string) { this.successMsg = msg; setTimeout(() => this.successMsg = '', 3000); }

  calcNights(checkIn: string, checkOut: string): number {
    if (!checkIn || !checkOut) return 0;
    return Math.max(0, Math.round((new Date(checkOut).getTime() - new Date(checkIn).getTime()) / 86400000));
  }

  get nights() { return this.calcNights(this.availForm.checkIn, this.availForm.checkOut); }
  get modalNights() { return this.calcNights(this.createForm.checkInDate, this.createForm.checkOutDate); }
  get confirmedCount() { return this.reservations.filter(r => r.status === 'Confirmed').length; }
  get cancelledCount() { return this.reservations.filter(r => r.status === 'Cancelled').length; }
  get totalRevenue() { return this.reservations.filter(r => r.status !== 'Cancelled').reduce((s, r) => s + r.totalAmount, 0); }
}
