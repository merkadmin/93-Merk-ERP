import { DOCUMENT, NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../core/api.service';
import { AuthService, AuthUser } from '../../core/auth.service';
import { LanguageService } from '../../shared/services/language.service';
import { ThemeService } from '../../shared/services/theme.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NgClass, TranslatePipe],
  templateUrl: './login.component.html',
  styleUrl: './login.component.less',
})
export class LoginComponent {
  private api       = inject(ApiService);
  private auth      = inject(AuthService);
  private router    = inject(Router);
  private toastr    = inject(ToastrService);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  public langService  = inject(LanguageService);
  public themeService = inject(ThemeService);

  login    = '';
  password = '';
  loading  = signal(false);
  showPwd  = signal(false);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  submit(): void {
    if (!this.login.trim() || !this.password.trim()) {
      this.toastr.warning(this.translate.instant('login.fields_required'));
      return;
    }
    this.loading.set(true);
    this.api.post<AuthUser>('auth/login', { login: this.login, password: this.password }).subscribe({
      next: user => {
        this.auth.login(user);
        this.router.navigate(['/stock/items']);
      },
      error: () => {
        this.loading.set(false);
        this.toastr.error(this.translate.instant('login.invalid_credentials'));
      },
    });
  }
}
