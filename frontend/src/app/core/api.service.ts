import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = 'http://localhost:5202/api';

  get<T>(path: string)                  { return this.http.get<T>(`${this.base}/${path}`); }
  post<T>(path: string, body: unknown)  { return this.http.post<T>(`${this.base}/${path}`, body); }
  put<T>(path: string, body: unknown)   { return this.http.put<T>(`${this.base}/${path}`, body); }
  delete(path: string)                  { return this.http.delete(`${this.base}/${path}`); }
}
