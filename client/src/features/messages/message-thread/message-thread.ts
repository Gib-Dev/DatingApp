import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { MessageService } from '../../../core/services/message-service';
import { AccountService } from '../../../core/services/account-service';
import { ToastService } from '../../../core/services/toast-service';
import { Message } from '../../../types/message';

@Component({
  selector: 'app-message-thread',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './message-thread.html',
  styleUrl: './message-thread.css'
})
export class MessageThread implements OnInit, OnDestroy {
  messages = signal<Message[]>([]);
  loading = signal(false);
  sending = signal(false);
  recipientId = signal<string>('');
  recipientName = signal<string>('');

  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
  private toastService = inject(ToastService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  messageForm = this.fb.group({
    content: ['', [Validators.required, Validators.maxLength(500)]]
  });

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('userId');
    if (userId) {
      this.recipientId.set(userId);
      this.loadMessages();
    } else {
      this.router.navigate(['/messages']);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadMessages(): void {
    this.loading.set(true);
    this.messageService.getMessageThread(this.recipientId())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (messages) => {
          this.messages.set(messages);
          this.loading.set(false);
          if (messages.length > 0) {
            const currentUserId = this.accountService.currentUser()?.id;
            const firstMessage = messages[0];
            this.recipientName.set(
              firstMessage.senderId === currentUserId ? firstMessage.recipientName : firstMessage.senderName
            );
          }
          // Scroll to bottom after messages load
          setTimeout(() => this.scrollToBottom(), 100);
        },
        error: (err) => {
          this.toastService.error('Failed to load messages');
          console.error('Error loading messages:', err);
          this.loading.set(false);
        }
      });
  }

  onEnterKey(event: Event): void {
    event.preventDefault();
    const keyboardEvent = event as KeyboardEvent;
    if (!keyboardEvent.shiftKey) {
      this.sendMessage();
    }
  }

  sendMessage(): void {
    if (this.messageForm.invalid) {
      this.messageForm.markAllAsTouched();
      return;
    }

    const content = this.messageForm.value.content?.trim();
    if (!content) return;

    this.sending.set(true);
    this.messageService.sendMessage({
      recipientId: this.recipientId(),
      content
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (message) => {
          this.messages.update(current => [...current, message]);
          this.messageForm.reset();
          this.sending.set(false);
          setTimeout(() => this.scrollToBottom(), 100);
        },
        error: (err) => {
          this.toastService.error('Failed to send message');
          console.error('Error sending message:', err);
          this.sending.set(false);
        }
      });
  }

  isMyMessage(message: Message): boolean {
    return message.senderId === this.accountService.currentUser()?.id;
  }

  private scrollToBottom(): void {
    const chatContainer = document.getElementById('chatContainer');
    if (chatContainer) {
      chatContainer.scrollTop = chatContainer.scrollHeight;
    }
  }

  getTimeAgo(date: string): string {
    const now = new Date();
    const messageDate = new Date(date);
    const seconds = Math.floor((now.getTime() - messageDate.getTime()) / 1000);

    if (seconds < 60) return 'Just now';
    if (seconds < 3600) return `${Math.floor(seconds / 60)}m ago`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)}h ago`;
    return messageDate.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }
}
