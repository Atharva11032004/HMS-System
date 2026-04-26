import { Component, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin, Subscription } from 'rxjs';
import { catchError, of } from 'rxjs';
import { AuthService } from '../../../services/auth';
import { NotificationStore, AppNotification } from '../../../services/notification.store';
import { ReservationService } from '../../../services/reservation.service';
import { RoomService } from '../../../services/room.service';
import { GuestServiceApi } from '../../../services/guest.service';
import { BillingService } from '../../../services/billing.service';
import { ReservationManagement } from '../owner/reservation-management';
import { GuestManagement } from '../owner/guest-management';
import { BillingManagement } from '../owner/billing-management';
import { RoomManagement } from '../owner/room-management';

@Component({
  selector: 'app-receptionist-dashboard',
  standalone: true,
  imports: [CommonModule, ReservationManagement, GuestManagement, BillingManagement, RoomManagement],
  templateUrl: './receptionist-dashboard.html',
  styleUrl: './receptionist-dashboard.css'
})
export class ReceptionistDashboard implements OnInit, OnDestroy {
  activeSection = 'overview';
  sidebarOpen = false;
  notifPanelOpen = false;
  overviewLoading = true;

  notifications: AppNotification[] = [];
  unreadCount = 0;
  private sub!: Subscription;

  userEmail = '';
  userInitials = '';

  totalRooms = 0;
  availableRooms = 0;
  confirmedReservations = 0;
  totalGuests = 0;
  totalRevenue = 0;
  occupancyPct = 0;
  reservationChartBars: { label: string; value: number; pct: number; color: string }[] = [];

  navItems = [
    { id: 'overview',     icon: '⊞',  label: 'Overview' },
    { id: 'reservations', icon: '🗓️', label: 'Reservations' },
    { id: 'guests',       icon: '🧑',  label: 'Guests' },
    { id: 'rooms',        icon: '🛏️', label: 'Rooms' },
    { id: 'billing',      icon: '🧾',  label: 'Billing' },
  ];

  constructor(
    public auth: AuthService,
    public notifStore: NotificationStore,
    private elRef: ElementRef,
    private resSvc: ReservationService,
    private roomSvc: RoomService,
    private guestSvc: GuestServiceApi,
    private billSvc: BillingService
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
      bills:        this.billSvc.getAllBills().pipe(catchError(() => of([])))
    }).subscribe(({ rooms, reservations, guests, bills }) => {
      this.totalRooms            = rooms.length;
      this.availableRooms        = rooms.filter((r: any) => r.isAvailable).length;
      this.confirmedReservations = reservations.filter((r: any) => r.status === 'Confirmed').length;
      this.totalGuests           = guests.length;
      this.totalRevenue          = bills.reduce((s: number, b: any) => s + b.totalAmount, 0);
      this.occupancyPct          = this.totalRooms > 0
        ? Math.round(((this.totalRooms - this.availableRooms) / this.totalRooms) * 100) : 0;

      const statusMap: Record<string, number> = {};
      reservations.forEach((r: any) => { statusMap[r.status] = (statusMap[r.status] || 0) + 1; });
      const colors: Record<string, string> = { Confirmed: '#10b981', Cancelled: '#ef4444', CheckedIn: '#8b5cf6', CheckedOut: '#f59e0b' };
      const max = Math.max(...Object.values(statusMap), 1);
      this.reservationChartBars = Object.entries(statusMap).map(([label, value]) => ({
        label, value, pct: Math.round((value / max) * 100), color: colors[label] || '#8b5cf6'
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
