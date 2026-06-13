import { Injectable, computed, signal } from '@angular/core';

export interface AuthUser {
  id: number;
  name_EN: string;
  name_AR: string | null;
  login: string;
  email: string | null;
  userTypeId: number;
  userTypeName_EN: string | null;
  userTypeName_AR: string | null;
}

const STORAGE_KEY = 'merk_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private _user = signal<AuthUser | null>(this.loadFromStorage());

  readonly user      = this._user.asReadonly();
  readonly isLoggedIn = computed(() => this._user() !== null);

  login(user: AuthUser): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    this._user.set(user);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this._user.set(null);
  }

  private loadFromStorage(): AuthUser | null {
    try {
      const json = localStorage.getItem(STORAGE_KEY);
      return json ? JSON.parse(json) : null;
    } catch {
      return null;
    }
  }
}
