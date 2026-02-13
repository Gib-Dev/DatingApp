import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { LikesService } from '../../core/services/likes-service';
import { Member } from '../../types/member';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists implements OnInit, OnDestroy {
  members = signal<Member[]>([]);
  loading = signal(false);
  selectedTab = signal<'liked' | 'likedBy' | 'mutual'>('liked');

  private likesService = inject(LikesService);
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.loadLikes();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectTab(tab: 'liked' | 'likedBy' | 'mutual'): void {
    this.selectedTab.set(tab);
    this.loadLikes();
  }

  private loadLikes(): void {
    this.loading.set(true);
    this.likesService.getLikes(this.selectedTab())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (members) => {
          this.members.set(members);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Error loading likes:', err);
          this.loading.set(false);
        }
      });
  }
}
