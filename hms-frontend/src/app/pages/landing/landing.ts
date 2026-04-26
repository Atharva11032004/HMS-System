import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './landing.html',
  styleUrl: './landing.css'
})
export class Landing implements OnInit, OnDestroy {
  testimonials = [
    { initials: 'MR', name: 'Maria Rodriguez', role: 'GM, Grand Palms Resort', text: '"LuxeStay HMS cut our check-in time by 60%. Our front desk team can\'t imagine going back."' },
    { initials: 'TN', name: 'Thomas Nguyen', role: 'Owner, Boutique 88', text: '"The billing module alone saved us 10 hours a week. The microservices architecture means zero downtime."' },
    { initials: 'PK', name: 'Priya Kapoor', role: 'Operations Director, The Ivory', text: '"Real-time room tracking and guest notifications have dramatically improved our guest satisfaction scores."' },
    { initials: 'AL', name: 'Ahmed Larbi', role: 'Director, Sahara Grand', text: '"Onboarding took less than a day. The support team is exceptional and the platform is incredibly intuitive."' },
    { initials: 'SC', name: 'Sophie Chen', role: 'CEO, Urban Nest Hotels', text: '"Revenue jumped 22% in the first quarter after switching. The analytics dashboard is a game changer."' },
  ];

  activeIndex = 0;
  isAnimating = false;
  private timer: any;

  constructor(public auth: AuthService) {}

  ngOnInit() { this.startAuto(); }
  ngOnDestroy() { clearInterval(this.timer); }

  startAuto() { this.timer = setInterval(() => this.goTo((this.activeIndex + 1) % this.testimonials.length), 4000); }

  goTo(i: number) {
    if (this.isAnimating || i === this.activeIndex) return;
    this.isAnimating = true;
    this.activeIndex = i;
    setTimeout(() => this.isAnimating = false, 500);
  }

  prev() { clearInterval(this.timer); this.goTo((this.activeIndex - 1 + this.testimonials.length) % this.testimonials.length); this.startAuto(); }
  next() { clearInterval(this.timer); this.goTo((this.activeIndex + 1) % this.testimonials.length); this.startAuto(); }

  visibleTestimonials() {
    const len = this.testimonials.length;
    return [0, 1, 2].map(offset => this.testimonials[(this.activeIndex + offset) % len]);
  }
}
