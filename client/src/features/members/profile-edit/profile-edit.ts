import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MemberService } from '../../../core/services/member-service';
import { AccountService } from '../../../core/services/account-service';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './profile-edit.html',
  styleUrl: './profile-edit.css'
})
export class ProfileEdit implements OnInit, OnDestroy {
  loading = signal(false);
  saving = signal(false);

  private fb = inject(FormBuilder);
  private memberService = inject(MemberService);
  private accountService = inject(AccountService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  form = this.fb.group({
    description: ['', [Validators.maxLength(1000)]],
    city: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    country: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]]
  });

  ngOnInit(): void {
    this.loadCurrentMember();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCurrentMember(): void {
    const user = this.accountService.currentUser();
    if (!user) {
      this.router.navigate(['/']);
      return;
    }

    this.loading.set(true);
    this.memberService.getMember(user.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (member) => {
          this.form.patchValue({
            description: member.description || '',
            city: member.city,
            country: member.country
          });
          this.loading.set(false);
        },
        error: (err) => {
          this.toastService.error('Failed to load profile');
          this.loading.set(false);
          console.error('Error loading member:', err);
        }
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const updateData = this.form.value as { description?: string; city: string; country: string };

    this.memberService.updateMember(updateData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toastService.success('Profile updated successfully!');
          this.saving.set(false);
          this.form.markAsPristine();
        },
        error: (err) => {
          this.toastService.error('Failed to update profile');
          this.saving.set(false);
          console.error('Error updating member:', err);
        }
      });
  }

  onCancel(): void {
    if (this.form.dirty) {
      if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
        this.router.navigate(['/members']);
      }
    } else {
      this.router.navigate(['/members']);
    }
  }
}
