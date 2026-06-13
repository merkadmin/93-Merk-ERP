import { computed, inject, Injectable } from '@angular/core';
import { AuthService, AuthUser } from '../../core/auth.service';

@Injectable({ providedIn: 'root' })
export class LoginService {
  private auth = inject(AuthService);

  readonly user = this.auth.user;
  readonly isLoggedIn = this.auth.isLoggedIn;
  readonly currentUserId = computed(() => this.auth.user()?.id ?? null);
  readonly currentUserName = computed(() => {
    const u = this.auth.user();
    return u ? (u.name_EN || u.login) : null;
  });
}
