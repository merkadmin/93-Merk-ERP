import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { ApiService } from '../../core/api.service';

interface Warehouse { warehouseId: number; name: string; parentWarehouseId: number | null; isGroup: boolean; isActive: boolean; }

@Component({
  selector: 'app-warehouses',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="bi bi-building me-2"></i>Warehouses</h5>
      <button class="btn btn-sm btn-primary" (click)="openNew()"><i class="bi bi-plus"></i> Add</button>
    </div>

    <div *ngIf="showForm" class="card mb-3 border-primary">
      <div class="card-body">
        <form (ngSubmit)="save()">
          <div class="row g-2 align-items-end">
            <div class="col-md-4">
              <label class="form-label">Name</label>
              <input class="form-control form-control-sm" [(ngModel)]="form.name" name="name" required>
            </div>
            <div class="col-md-3">
              <label class="form-label">Parent Warehouse</label>
              <select class="form-select form-select-sm" [(ngModel)]="form.parentWarehouseId" name="parent">
                <option [ngValue]="null">— None —</option>
                <option *ngFor="let w of warehouses()" [ngValue]="w.warehouseId">{{ w.name }}</option>
              </select>
            </div>
            <div class="col-auto pt-4">
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isGroup" name="isGroup" id="wIsGroup">
                <label class="form-check-label" for="wIsGroup">Is Group</label>
              </div>
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isActive" name="isActive" id="wIsActive">
                <label class="form-check-label" for="wIsActive">Active</label>
              </div>
            </div>
            <div class="col-auto">
              <button type="submit" class="btn btn-sm btn-success me-1">Save</button>
              <button type="button" class="btn btn-sm btn-secondary" (click)="cancel()">Cancel</button>
            </div>
          </div>
        </form>
      </div>
    </div>

    <table class="table table-sm table-hover table-bordered">
      <thead class="table-dark"><tr><th>#</th><th>Name</th><th>Parent</th><th>Is Group</th><th>Active</th><th></th></tr></thead>
      <tbody>
        <tr *ngFor="let w of warehouses()">
          <td>{{ w.warehouseId }}</td>
          <td>{{ w.name }}</td>
          <td>{{ parentName(w.parentWarehouseId) }}</td>
          <td><span class="badge" [class]="w.isGroup ? 'bg-info' : 'bg-secondary'">{{ w.isGroup ? 'Yes' : 'No' }}</span></td>
          <td><span class="badge" [class]="w.isActive ? 'bg-success' : 'bg-secondary'">{{ w.isActive ? 'Yes' : 'No' }}</span></td>
          <td>
            <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(w)">Edit</button>
            <button class="btn btn-sm btn-outline-danger" (click)="delete(w.warehouseId)">Del</button>
          </td>
        </tr>
      </tbody>
    </table>
  `,
})
export class WarehousesComponent implements OnInit {
  api = inject(ApiService);
  warehouses = signal<Warehouse[]>([]);
  showForm = false;
  editing = false;
  form: Partial<Warehouse> = {};

  ngOnInit() { this.load(); }
  load() { this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d)); }
  openNew() { this.form = { name: '', parentWarehouseId: null, isGroup: false, isActive: true }; this.editing = false; this.showForm = true; }
  edit(w: Warehouse) { this.form = { ...w }; this.editing = true; this.showForm = true; }
  cancel() { this.showForm = false; }
  parentName(id: number | null) { return id ? (this.warehouses().find(w => w.warehouseId === id)?.name ?? '') : '—'; }
  save() {
    const obs = this.editing
      ? this.api.put(`warehouses/${this.form.warehouseId}`, this.form)
      : this.api.post('warehouses', this.form);
    obs.subscribe(() => { this.load(); this.cancel(); });
  }
  delete(id: number) {
    if (confirm('Delete this warehouse?')) this.api.delete(`warehouses/${id}`).subscribe(() => this.load());
  }
}
