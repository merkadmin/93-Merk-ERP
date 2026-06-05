import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { ApiService } from '../../core/api.service';

interface UOM { uomId: number; name: string; mustBeWholeNumber: boolean; }

@Component({
  selector: 'app-uoms',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="bi bi-rulers me-2"></i>Units of Measure</h5>
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
            <div class="col-auto pt-4">
              <div class="form-check">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.mustBeWholeNumber" name="whole" id="whole">
                <label class="form-check-label" for="whole">Whole Number Only</label>
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
      <thead class="table-dark"><tr><th>#</th><th>Name</th><th>Whole Number</th><th></th></tr></thead>
      <tbody>
        <tr *ngFor="let u of uoms()">
          <td>{{ u.uomId }}</td>
          <td>{{ u.name }}</td>
          <td><span class="badge" [class]="u.mustBeWholeNumber ? 'bg-info' : 'bg-secondary'">{{ u.mustBeWholeNumber ? 'Yes' : 'No' }}</span></td>
          <td>
            <button class="btn btn-xs btn-sm btn-outline-primary me-1" (click)="edit(u)">Edit</button>
            <button class="btn btn-xs btn-sm btn-outline-danger" (click)="delete(u.uomId)">Del</button>
          </td>
        </tr>
      </tbody>
    </table>
  `,
})
export class UomsComponent implements OnInit {
  api = inject(ApiService);
  uoms = signal<UOM[]>([]);
  showForm = false;
  editing = false;
  form: Partial<UOM> = {};

  ngOnInit() { this.load(); }
  load() { this.api.get<UOM[]>('uoms').subscribe(d => this.uoms.set(d)); }
  openNew() { this.form = { name: '', mustBeWholeNumber: false }; this.editing = false; this.showForm = true; }
  edit(u: UOM) { this.form = { ...u }; this.editing = true; this.showForm = true; }
  cancel() { this.showForm = false; }
  save() {
    const obs = this.editing
      ? this.api.put(`uoms/${this.form.uomId}`, this.form)
      : this.api.post('uoms', this.form);
    obs.subscribe(() => { this.load(); this.cancel(); });
  }
  delete(id: number) {
    if (confirm('Delete this UOM?')) this.api.delete(`uoms/${id}`).subscribe(() => this.load());
  }
}
