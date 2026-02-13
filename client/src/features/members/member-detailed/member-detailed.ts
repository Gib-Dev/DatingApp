import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MemberService } from '../../../core/services/member-service';
import { Member } from '../../../types/member';

@Component({
  selector: 'app-member-detailed',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css'
})
export class MemberDetailed implements OnInit, OnDestroy {
  member = signal<Member | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadMember(id);
    } else {
      this.router.navigate(['/members']);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadMember(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.memberService.getMember(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (member) => {
          this.member.set(member);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set('Failed to load member profile. Please try again.');
          this.loading.set(false);
          console.error('Error loading member:', err);
        }
      });
  }

  calculateAge(dateOfBirth: string): number {
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
