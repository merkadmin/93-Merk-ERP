import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, tap, throwError } from 'rxjs';
import { ServerStatusService } from '../services/server-status.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const status = inject(ServerStatusService);
  return next(req).pipe(
    tap({ next: () => status.setOffline(false) }),
    catchError(err => {
      if (err.status === 0) status.setOffline(true);
      return throwError(() => err);
    })
  );
};
