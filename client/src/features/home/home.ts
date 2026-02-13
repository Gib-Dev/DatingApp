import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Register } from '../account/register/register';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  protected registerMode = signal(false);

  private accountService = inject(AccountService);
  private router = inject(Router);

  ngOnInit(): void {
    // Redirect to members if already logged in
    if (this.accountService.currentUser()) {
      this.router.navigateByUrl('/members');
    }
  }

  showRegister(value: boolean) {
    this.registerMode.set(value);
  }
}
