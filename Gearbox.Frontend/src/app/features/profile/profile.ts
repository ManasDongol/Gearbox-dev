import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../../core/services/auth/auth';
import { UserService } from '../../core/services/user/user.service';
import { UserProfile } from '../../core/models/user-profile.model';
import { Navmenu } from '../../shared/components/navmenu/navmenu';
import { Topbar } from '../../shared/components/topbar/topbar';
import { ToastService } from '../../shared/components/toast/toast.service';
import { Spinner } from '../../shared/components/spinner/spinner';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, Navmenu, Topbar],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile implements OnInit {
  private auth = inject(Auth);
  private userService = inject(UserService);

  profile: UserProfile | null = null;
  isLoading = true;
  isSaving = false;
  saveMessage = '';
  errorMessage = '';

  ngOnInit() {
    this.loadProfile();
  }

  get displayName(): string {
    const fullName = `${this.profile?.firstName ?? ''} ${this.profile?.lastName ?? ''}`.trim();
    return fullName || this.profile?.username || 'Gearbox User';
  }

  get initials(): string {
    return this.displayName
      .split(/[.@\s_-]+/)
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0].toUpperCase())
      .join('');
  }

  get roles(): string {
    return this.auth.user?.roles?.join(', ') || this.profile?.role || 'User';
  }

  loadProfile() {
    const userId = this.auth.user?.userId;
    if (!userId) {
      this.isLoading = false;
      this.errorMessage = 'Unable to load profile for this session.';
      return;
    }

    this.isLoading = true;
    this.userService.getById(userId).subscribe({
      next: (profile) => {
        this.profile = {
          ...profile,
          firstName: profile.firstName ?? '',
          lastName: profile.lastName ?? '',
          address: profile.address ?? '',
          phoneNumber: profile.phoneNumber ?? '',
        };
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Profile details could not be loaded.';
        this.isLoading = false;
      },
    });
  }

  saveProfile() {
    const userId = this.auth.user?.userId;
    if (!userId || !this.profile) return;

    this.isSaving = true;
    this.saveMessage = '';
    this.errorMessage = '';

    const payload: UserProfile = {
      ...this.profile,
      username: this.profile.username,
      email: this.profile.email,
      firstName: this.profile.firstName.trim(),
      lastName: this.profile.lastName.trim(),
      address: this.profile.address.trim(),
      phoneNumber: this.profile.phoneNumber.trim(),
    };

    this.userService.update(userId, payload).subscribe({
      next: () => {
        this.profile = payload;
        this.isSaving = false;
        this.saveMessage = 'Profile updated successfully.';
      },
      error: () => {
        this.isSaving = false;
        this.errorMessage = 'Profile could not be saved. Please try again.';
      },
    });
  }
}
