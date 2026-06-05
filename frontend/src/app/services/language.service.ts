import { Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export type Lang = 'en' | 'ar';

const STORAGE_KEY = 'app-lang';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private _lang = signal<Lang>(this._loadStored());

  readonly lang  = this._lang.asReadonly();
  readonly isRtl = () => this._lang() === 'ar';

  constructor(private translate: TranslateService) {
    this.translate.addLangs(['en', 'ar']);
    this.translate.setDefaultLang('en');
    this._apply(this._lang());
  }

  setLang(lang: Lang): void {
    this._lang.set(lang);
    localStorage.setItem(STORAGE_KEY, lang);
    this._apply(lang);
  }

  toggle(): void {
    this.setLang(this._lang() === 'en' ? 'ar' : 'en');
  }

  private _apply(lang: Lang): void {
    this.translate.use(lang);
    document.documentElement.setAttribute('lang', lang);
    document.documentElement.setAttribute('dir', lang === 'ar' ? 'rtl' : 'ltr');
  }

  private _loadStored(): Lang {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'ar' ? 'ar' : 'en';
  }
}
