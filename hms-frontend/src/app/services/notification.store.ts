import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from './auth';

export interface AppNotification {
  id: number;
  title: string;
  message: string;
  type: 'success' | 'info' | 'warning' | 'error';
  time: Date;
  read: boolean;
}

@Injectable({ providedIn: 'root' })
export class NotificationStore {
  private base = '/api/notification/api/notifications';
  private _notifications = new BehaviorSubject<AppNotification[]>([]);
  notifications$ = this._notifications.asObservable();
  private nextId = 1;

  constructor(private http: HttpClient, private auth: AuthService) {
    // seed with some initial system notifications
    this.add('System Ready', 'HMS dashboard loaded successfully.', 'success');
    this.add('Low Stock Alert', 'Some inventory items are below minimum stock level.', 'warning');
    this.add('New Reservation', 'A new reservation has been created via the system.', 'info');
  }

  private get h(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  get all(): AppNotification[] { return this._notifications.getValue(); }
  get unreadCount(): number { return this.all.filter(n => !n.read).length; }

  add(title: string, message: string, type: AppNotification['type'] = 'info') {
    const current = this._notifications.getValue();
    this._notifications.next([
      { id: this.nextId++, title, message, type, time: new Date(), read: false },
      ...current
    ]);
  }

  markAllRead() {
    this._notifications.next(this.all.map(n => ({ ...n, read: true })));
  }

  markRead(id: number) {
    this._notifications.next(this.all.map(n => n.id === id ? { ...n, read: true } : n));
  }

  clear() { this._notifications.next([]); }

  // Send via NotificationService backend
  sendNotification(recipientEmail: string, subject: string, message: string) {
    return this.http.post(`${this.base}/notify`, {
      recipientEmail, subject, message, priority: 'Normal'
    }, { headers: this.h });
  }

  sendTestEmail(recipientEmail: string) {
    return this.http.post(`${this.base}/test-email?recipientEmail=${encodeURIComponent(recipientEmail)}&subject=Test`, {}, { headers: this.h });
  }
}
