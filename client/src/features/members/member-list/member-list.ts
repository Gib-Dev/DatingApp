import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MemberService } from '../../../core/services/member-service';
import { LikesService } from '../../../core/services/likes-service';
import { ToastService } from '../../../core/services/toast-service';
import { Member } from '../../../types/member';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})
export class MemberList implements OnInit, OnDestroy {
  members = signal<Member[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  likingMemberId = signal<string | null>(null);

  private memberService = inject(MemberService);
  private likesService = inject(LikesService);
  private toastService = inject(ToastService);
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.loadMembers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadMembers(): void {
    this.loading.set(true);
    this.error.set(null);

    this.memberService.getMembers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (members) => {
          this.members.set(members);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set('Failed to load members. Please try again.');
          this.loading.set(false);
          console.error('Error loading members:', err);
        }
      });
  }

  toggleLike(memberId: string, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    this.likingMemberId.set(memberId);
    this.likesService.toggleLike(memberId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.likingMemberId.set(null);
          this.toastService.success('Like updated!');
        },
        error: (err) => {
          this.likingMemberId.set(null);
          this.toastService.error('Failed to update like');
          console.error('Error toggling like:', err);
        }
      });
  }
}
