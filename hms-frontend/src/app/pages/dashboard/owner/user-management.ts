import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService, UserDto } from '../../../services/auth';
import { RoleCountPipe } from '../../../pipes/role-count.pipe';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule, RoleCountPipe],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css'
})
export class UserManagement implements OnInit {
  // Owner can create Manager + Receptionist, Manager can only create Receptionist
  @Input() allowedRoles: string[] = ['Manager', 'Receptionist'];

  users: UserDto[] = [];
  loading = false;

  showCreateModal = false;
  form = { email: '', password: '', role: '' };

  showDeleteConfirm = false;
  deletingId: string | null = null;
  deletingEmail = '';

  successMsg = '';
  errorMsg = '';

  constructor(private auth: AuthService) {}

  ngOnInit() {
    this.form.role = this.allowedRoles[0];
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.auth.getUsers().subscribe({
      next: u => {
        // Filter to only show users with roles this panel manages
        this.users = u.filter(x => this.allowedRoles.includes(x.role));
        this.loading = false;
      },
      error: (e: any) => {
        this.showError(e?.error?.message || 'Failed to load users');
        this.loading = false;
      }
    });
  }

  openCreate() {
    this.form = { email: '', password: '', role: this.allowedRoles[0] };
    this.showCreateModal = true;
  }

  createUser() {
    if (!this.form.email || !this.form.password || !this.form.role) {
      this.showError('All fields are required'); return;
    }
    this.auth.createUser(this.form).subscribe({
      next: () => {
        this.showSuccess(`${this.form.role} account created for ${this.form.email}`);
        this.showCreateModal = false;
        this.loadUsers();
      },
      error: (e: any) => {
        const msg = e?.error?.errors?.join(', ') || e?.error?.title || 'Failed to create user';
        this.showError(msg);
      }
    });
  }

  confirmDelete(user: UserDto) {
    this.deletingId = user.id;
    this.deletingEmail = user.email;
    this.showDeleteConfirm = true;
  }

  deleteUser() {
    if (!this.deletingId) return;
    this.auth.deleteUser(this.deletingId).subscribe({
      next: () => {
        this.showSuccess('User deleted');
        this.showDeleteConfirm = false;
        this.loadUsers();
      },
      error: (e: any) => this.showError(e?.error?.message || 'Failed to delete user')
    });
  }

  roleColor(role: string): string {
    return { Owner: '#f59e0b', Manager: '#10b981', Receptionist: '#8b5cf6' }[role] || '#64748b';
  }

  showSuccess(msg: string) { this.successMsg = msg; this.errorMsg = ''; setTimeout(() => this.successMsg = '', 3000); }
  showError(msg: string)   { this.errorMsg = msg; setTimeout(() => this.errorMsg = '', 6000); }
}
