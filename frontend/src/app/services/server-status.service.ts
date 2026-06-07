import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServerStatusService {
  readonly isOffline = signal(false);

  setOffline(offline: boolean) { this.isOffline.set(offline); }
}
