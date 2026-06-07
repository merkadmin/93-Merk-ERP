import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { loadingInterceptor } from './core/loading.interceptor';
import { errorInterceptor } from './core/error.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideToastr } from 'ngx-toastr';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([loadingInterceptor, errorInterceptor])),
    provideAnimationsAsync(),
    provideTranslateService({
      loader: provideTranslateHttpLoader({ suffix: '.json' }),
      fallbackLang: 'en'
    }),
    provideToastr({
      timeOut: 4000,
      positionClass: 'toast-top-right',
      progressBar: true,
      closeButton: true,
      preventDuplicates: true,
    })
  ]
};
