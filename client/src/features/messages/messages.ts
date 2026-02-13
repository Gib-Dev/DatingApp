import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MessageService } from '../../core/services/message-service';
import { AccountService } from '../../core/services/account-service';
import { Message } from '../../types/message';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './messages.html',
  styleUrl: './messages.css'
})
export class Messages implements OnInit, OnDestroy {
  messages = signal<Message[]>([]);
  loading = signal(false);
  container = signal<'inbox' | 'outbox' | 'unread'>('inbox');

  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.loadMessages();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectContainer(container: 'inbox' | 'outbox' | 'unread'): void {
    this.container.set(container);
    this.loadMessages();
  }

  private loadMessages(): void {
    this.loading.set(true);
    this.messageService.getMessages(this.container())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (messages) => {
          this.messages.set(messages);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Error loading messages:', err);
          this.loading.set(false);
        }
      });
  }

  openThread(message: Message): void {
    const currentUserId = this.accountService.currentUser()?.id;
    const otherUserId = message.senderId === currentUserId ? message.recipientId : message.senderId;
    this.router.navigate(['/messages', otherUserId]);
  }

  getTimeAgo(date: string): string {
    const now = new Date();
    const messageDate = new Date(date);
    const seconds = Math.floor((now.getTime() - messageDate.getTime()) / 1000);

    if (seconds < 60) return 'Just now';
    if (seconds < 3600) return `${Math.floor(seconds / 60)}m ago`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)}h ago`;
    if (seconds < 604800) return `${Math.floor(seconds / 86400)}d ago`;
    return messageDate.toLocaleDateString();
  }
}
