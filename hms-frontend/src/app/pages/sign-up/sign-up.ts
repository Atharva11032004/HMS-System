import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.css'
})
export class SignUp {
  email = '';
  password = '';
  role = 'Receptionist';
  error = '';
  success = '';
  loading = false;

  // Public registration only allows Receptionist
  roles = [
    { value: 'Receptionist', label: 'Receptionist', icon: '🛎️' }
  ];

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit(): void {
    this.error = '';
    this.success = '';
    this.loading = true;
    this.auth.register({ email: this.email, password: this.password, role: this.role }).subscribe({
      next: () => {
        this.success = 'Account created! Redirecting to sign in…';
        setTimeout(() => this.router.navigate(['/sign-in']), 1500);
      },
      error: (err) => {
        this.error = err?.error?.errors?.join(', ') || 'Registration failed. Please try again.';
        this.loading = false;
      }
    });
  }
}
