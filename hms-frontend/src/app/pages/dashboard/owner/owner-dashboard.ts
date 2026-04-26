import { Component, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';
import { catchError, of } from 'rxjs';
import { AuthService } from '../../../services/auth';
import { NotificationStore, AppNotification } from '../../../services/notification.store';
import { ReservationService } from '../../../services/reservation.service';
import { RoomService } from '../../../services/room.service';
import { GuestServiceApi } from '../../../services/guest.service';
import { StaffService } from '../../../services/staff.service';
import { BillingService } from '../../../services/billing.service';
import { InventoryService } from '../../../services/inventory.service';
import { StaffManagement } from './staff-management';
import { ReservationManagement } from './reservation-management';
import { RoomManagement } from './room-management';
import { BillingManagement } from './billing-management';
import { GuestManagement } from './guest-management';
import { InventoryManagement } from './inventory-management';
import { PricingManagement } from './pricing-management';
import { UserManagement } from './user-management';

@Component({
  selector: 'app-owner-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, StaffManagement, ReservationManagement, RoomManagement, BillingManagement, GuestManagement, InventoryManagement, PricingManagement, UserManagement],
  templateUrl: './owner-dashboard.html',
  styleUrl: './owner-dashboard.css'
})
export class OwnerDashboard implements OnInit, OnDestroy {
  activeSection = 'overview';
  sidebarOpen = false;
  notifPanelOpen = false;
  overviewLoading = true;

  notifications: AppNotification[] = [];
  unreadCount = 0;
  private sub!: Subscription;

  userEmail = '';
  userInitials = '';

  // KPI data
  totalRooms = 0;
  availableRooms = 0;
  totalRevenue = 0;
  confirmedReservations = 0;
  cancelledReservations = 0;
  totalGuests = 0;
  totalStaff = 0;
  lowStockCount = 0;

  // Chart data
  reservationChartBars: { label: string; value: number; pct: number; color: string }[] = [];
  occupancyPct = 0;

  navItems = [
    { id: 'overview',     icon: '⊞',  label: 'Overview' },
    { id: 'staff',        icon: '👥',  label: 'Staff' },
    { id: 'reservations', icon: '🗓️', label: 'Reservations' },
    { id: 'rooms',        icon: '🛏️', label: 'Rooms' },
    { id: 'billing',      icon: '🧾',  label: 'Billing' },
    { id: 'guests',       icon: '🧑',  label: 'Guests' },
    { id: 'inventory',    icon: '📦',  label: 'Inventory' },
    { id: 'pricing',      icon: '💰',  label: 'Pricing' },
    { id: 'users',        icon: '🔑',  label: 'Users' },
  ];

  constructor(
    public auth: AuthService,
    private router: Router,
    public notifStore: NotificationStore,
    private elRef: ElementRef,
    private resSvc: ReservationService,
    private roomSvc: RoomService,
    private guestSvc: GuestServiceApi,
    private staffSvc: StaffService,
    private billSvc: BillingService,
    private invSvc: InventoryService
  ) {}

  ngOnInit() {
    const token = this.auth.getToken();
    if (token) {
      try {
        const p = JSON.parse(atob(token.split('.')[1]));
        this.userEmail = p.email ?? '';
        const parts = this.userEmail.split('@')[0].split('.');
        this.userInitials = parts.map((s: string) => s[0]?.toUpperCase()).join('').slice(0, 2);
      } catch {}
    }
    this.sub = this.notifStore.notifications$.subscribe(n => {
      this.notifications = n;
      this.unreadCount = n.filter(x => !x.read).length;
    });
    this.loadOverviewData();
  }

  loadOverviewData() {
    this.overviewLoading = true;
    forkJoin({
      rooms:        this.roomSvc.getRooms().pipe(catchError(() => of([]))),
      reservations: this.resSvc.getAll().pipe(catchError(() => of([]))),
      guests:       this.guestSvc.getAll().pipe(catchError(() => of([]))),
      staff:        this.staffSvc.getAllStaff().pipe(catchError(() => of([]))),
      bills:        this.billSvc.getAllBills().pipe(catchError(() => of([]))),
      lowStock:     this.invSvc.getLowStock().pipe(catchError(() => of([])))
    }).subscribe(({ rooms, reservations, guests, staff, bills, lowStock }) => {
      // KPIs
      this.totalRooms        = rooms.length;
      this.availableRooms    = rooms.filter((r: any) => r.isAvailable).length;
      this.confirmedReservations = reservations.filter((r: any) => r.status === 'Confirmed').length;
      this.cancelledReservations = reservations.filter((r: any) => r.status === 'Cancelled').length;
      this.totalRevenue      = bills.reduce((s: number, b: any) => s + b.totalAmount, 0);
      this.totalGuests       = guests.length;
      this.totalStaff        = staff.length;
      this.lowStockCount     = lowStock.length;
      this.occupancyPct      = this.totalRooms > 0
        ? Math.round(((this.totalRooms - this.availableRooms) / this.totalRooms) * 100) : 0;

      // Reservation bar chart — group by status
      const statusMap: Record<string, number> = {};
      reservations.forEach((r: any) => { statusMap[r.status] = (statusMap[r.status] || 0) + 1; });
      const colors: Record<string, string> = { Confirmed: '#10b981', Cancelled: '#ef4444', CheckedIn: '#2d7dd2', CheckedOut: '#f59e0b' };
      const max = Math.max(...Object.values(statusMap), 1);
      this.reservationChartBars = Object.entries(statusMap).map(([label, value]) => ({
        label, value, pct: Math.round((value / max) * 100),
        color: colors[label] || '#8b5cf6'
      }));

      this.overviewLoading = false;
    });
  }

  ngOnDestroy() { this.sub?.unsubscribe(); }

  navigate(section: string) { this.activeSection = section; this.sidebarOpen = false; }

  toggleNotifPanel() {
    this.notifPanelOpen = !this.notifPanelOpen;
    if (this.notifPanelOpen) this.notifStore.markAllRead();
  }

  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent) {
    if (!this.elRef.nativeElement.querySelector('.notif-wrapper')?.contains(e.target)) {
      this.notifPanelOpen = false;
    }
  }

  notifIcon(type: string) {
    return ({ success: '✅', info: 'ℹ️', warning: '⚠️', error: '❌' } as any)[type] ?? 'ℹ️';
  }

  timeAgo(date: Date): string {
    const s = Math.floor((Date.now() - new Date(date).getTime()) / 1000);
    if (s < 60) return 'just now';
    if (s < 3600) return `${Math.floor(s / 60)}m ago`;
    if (s < 86400) return `${Math.floor(s / 3600)}h ago`;
    return `${Math.floor(s / 86400)}d ago`;
  }
}
