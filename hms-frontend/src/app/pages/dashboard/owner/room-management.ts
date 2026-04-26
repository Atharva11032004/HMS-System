import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoomService, RoomDto, RoomTypeDto, CreateRoomRequest, CreateRoomTypeRequest, BlockRequest } from '../../../services/room.service';

type Tab = 'rooms' | 'roomtypes' | 'availability';

@Component({
  selector: 'app-room-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './room-management.html',
  styleUrl: './room-management.css'
})
export class RoomManagement implements OnInit {
  activeTab: Tab = 'rooms';

  // ── Rooms ──
  rooms: RoomDto[] = [];
  roomsLoading = false;
  showRoomModal = false;
  editingRoom: RoomDto | null = null;
  roomForm: CreateRoomRequest = { roomNumber: '', roomTypeId: 0, isAvailable: true };
  showDeleteRoomConfirm = false;
  deletingRoomId: number | null = null;

  // ── Room Types ──
  roomTypes: RoomTypeDto[] = [];
  roomTypesLoading = false;
  showTypeModal = false;
  editingType: RoomTypeDto | null = null;
  typeForm: CreateRoomTypeRequest = { name: '', description: '', maxOccupancy: 1 };
  showDeleteTypeConfirm = false;
  deletingTypeId: number | null = null;

  // ── Availability ──
  availForm = { checkIn: '', checkOut: '', adults: 1, children: 0 };
  availRooms: RoomDto[] = [];
  availLoading = false;
  availSearchDone = false;

  // ── Block ──
  showBlockModal = false;
  blockingRoom: RoomDto | null = null;
  blockForm = { checkIn: '', checkOut: '' };

  successMsg = '';
  errorMsg = '';

  constructor(private svc: RoomService) {}

  ngOnInit() {
    this.loadRooms();
    this.loadRoomTypes();
  }

  // ── Rooms ──
  loadRooms() {
    this.roomsLoading = true;
    this.svc.getRooms().subscribe({
      next: r => { this.rooms = r; this.roomsLoading = false; },
      error: () => { this.showError('Failed to load rooms'); this.roomsLoading = false; }
    });
  }

  openAddRoom() {
    this.editingRoom = null;
    this.roomForm = { roomNumber: '', roomTypeId: this.roomTypes[0]?.id ?? 0, isAvailable: true };
    this.showRoomModal = true;
  }

  openEditRoom(r: RoomDto) {
    this.editingRoom = r;
    const typeId = this.roomTypes.find(t => t.name === r.roomTypeName)?.id ?? 0;
    this.roomForm = { roomNumber: r.roomNumber, roomTypeId: typeId, isAvailable: r.isAvailable };
    this.showRoomModal = true;
  }

  saveRoom() {
    if (!this.roomForm.roomNumber || !this.roomForm.roomTypeId) {
      this.showError('Room number and type are required'); return;
    }
    const onSuccess = () => {
      this.showSuccess(this.editingRoom ? 'Room updated' : 'Room created');
      this.showRoomModal = false;
      this.loadRooms();
    };
    const onError = (e: any) => this.showError(e?.error?.Error || e?.error?.title || 'Failed to save room');

    if (this.editingRoom) {
      this.svc.updateRoom(this.editingRoom.id, this.roomForm).subscribe({ next: onSuccess, error: onError });
    } else {
      this.svc.createRoom(this.roomForm).subscribe({ next: onSuccess, error: onError });
    }
  }

  confirmDeleteRoom(id: number) { this.deletingRoomId = id; this.showDeleteRoomConfirm = true; }

  deleteRoom() {
    if (this.deletingRoomId == null) return;
    this.svc.deleteRoom(this.deletingRoomId).subscribe({
      next: () => { this.showSuccess('Room deleted'); this.showDeleteRoomConfirm = false; this.loadRooms(); },
      error: () => this.showError('Failed to delete room')
    });
  }

  openBlockModal(r: RoomDto) {
    this.blockingRoom = r;
    this.blockForm = { checkIn: '', checkOut: '' };
    this.showBlockModal = true;
  }

  blockRoom() {
    if (!this.blockingRoom || !this.blockForm.checkIn || !this.blockForm.checkOut) return;
    const req: BlockRequest = {
      roomId: this.blockingRoom.id,
      checkIn: new Date(this.blockForm.checkIn).toISOString(),
      checkOut: new Date(this.blockForm.checkOut).toISOString()
    };
    this.svc.blockRoom(req).subscribe({
      next: () => { this.showSuccess('Room blocked'); this.showBlockModal = false; this.loadRooms(); },
      error: e => this.showError(e?.error?.Error || 'Failed to block room')
    });
  }

  freeRoom(r: RoomDto) {
    this.svc.freeRoom(r.id).subscribe({
      next: () => { this.showSuccess(`Room ${r.roomNumber} freed`); this.loadRooms(); },
      error: () => this.showError('Failed to free room')
    });
  }

  // ── Room Types ──
  loadRoomTypes() {
    this.roomTypesLoading = true;
    this.svc.getRoomTypes().subscribe({
      next: t => { this.roomTypes = t; this.roomTypesLoading = false; },
      error: () => { this.showError('Failed to load room types'); this.roomTypesLoading = false; }
    });
  }

  openAddType() {
    this.editingType = null;
    this.typeForm = { name: '', description: '', maxOccupancy: 1 };
    this.showTypeModal = true;
  }

  openEditType(t: RoomTypeDto) {
    this.editingType = t;
    this.typeForm = { name: t.name, description: t.description, maxOccupancy: t.maxOccupancy };
    this.showTypeModal = true;
  }

  saveType() {
    if (!this.typeForm.name) { this.showError('Name is required'); return; }
    const onSuccess = () => {
      this.showSuccess(this.editingType ? 'Room type updated' : 'Room type created');
      this.showTypeModal = false;
      this.loadRoomTypes();
    };
    const onError = (e: any) => this.showError(e?.error?.Error || 'Failed to save room type');

    if (this.editingType) {
      this.svc.updateRoomType(this.editingType.id, this.typeForm).subscribe({ next: onSuccess, error: onError });
    } else {
      this.svc.createRoomType(this.typeForm).subscribe({ next: onSuccess, error: onError });
    }
  }

  confirmDeleteType(id: number) { this.deletingTypeId = id; this.showDeleteTypeConfirm = true; }

  deleteType() {
    if (this.deletingTypeId == null) return;
    this.svc.deleteRoomType(this.deletingTypeId).subscribe({
      next: () => { this.showSuccess('Room type deleted'); this.showDeleteTypeConfirm = false; this.loadRoomTypes(); },
      error: () => this.showError('Failed to delete room type')
    });
  }

  // ── Availability ──
  searchAvailability() {
    if (!this.availForm.checkIn || !this.availForm.checkOut) return;
    this.availLoading = true;
    this.availSearchDone = false;
    this.svc.getAvailableRooms(
      new Date(this.availForm.checkIn).toISOString(),
      new Date(this.availForm.checkOut).toISOString(),
      this.availForm.adults,
      this.availForm.children
    ).subscribe({
      next: res => {
        this.availRooms = res?.availableRooms ?? (res as any)?.AvailableRooms ?? [];
        this.availLoading = false;
        this.availSearchDone = true;
      },
      error: () => { this.showError('Failed to fetch availability'); this.availLoading = false; }
    });
  }

  get availableCount() { return this.rooms.filter(r => r.isAvailable).length; }
  get occupiedCount()  { return this.rooms.filter(r => !r.isAvailable).length; }

  showSuccess(msg: string) { this.successMsg = msg; this.errorMsg = ''; setTimeout(() => this.successMsg = '', 3000); }
  showError(msg: string)   { this.errorMsg = msg; setTimeout(() => this.errorMsg = '', 5000); }
}
