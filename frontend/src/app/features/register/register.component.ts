import { DOCUMENT, NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../core/api.service';
import { AuthService, AuthUser } from '../../core/auth.service';
import { LanguageService } from '../../shared/services/language.service';
import { ThemeService } from '../../shared/services/theme.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, NgClass, RouterLink, TranslatePipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.less',
})
export class RegisterComponent {
  private api       = inject(ApiService);
  private auth      = inject(AuthService);
  private router    = inject(Router);
  private toastr    = inject(ToastrService);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  public langService  = inject(LanguageService);
  public themeService = inject(ThemeService);

  name_EN  = '';
  name_AR  = '';
  login    = '';
  email    = '';
  password = '';
  confirm  = '';

  loading  = signal(false);
  showPwd  = signal(false);
  showConf = signal(false);

  submit(): void {
    if (!this.name_EN.trim() || !this.login.trim() || !this.password.trim()) {
      this.toastr.warning(this.translate.instant('register.fields_required'));
      return;
    }
    if (this.password !== this.confirm) {
      this.toastr.warning(this.translate.instant('register.password_mismatch'));
      return;
    }
    this.loading.set(true);
    this.api.post<AuthUser>('auth/register', {
      name_EN:  this.name_EN,
      name_AR:  this.name_AR  || null,
      login:    this.login,
      password: this.password,
      email:    this.email    || null,
    }).subscribe({
      next: user => {
        this.auth.login(user);
        this.router.navigate(['/stock/items']);
      },
      error: err => {
        this.loading.set(false);
        const msg = err.status === 409
          ? this.translate.instant('register.login_taken')
          : this.translate.instant('register.error');
        this.toastr.error(msg);
      },
    });
  }
}
