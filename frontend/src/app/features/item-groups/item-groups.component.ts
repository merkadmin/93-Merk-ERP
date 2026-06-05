import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { ApiService } from '../../core/api.service';

interface ItemGroup { itemGroupId: number; name: string; parentItemGroupId: number | null; isGroup: boolean; isActive: boolean; }

@Component({
  selector: 'app-item-groups',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="bi bi-diagram-3 me-2"></i>Item Groups</h5>
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
              <label class="form-label">Parent Group</label>
              <select class="form-select form-select-sm" [(ngModel)]="form.parentItemGroupId" name="parent">
                <option [ngValue]="null">— None —</option>
                <option *ngFor="let g of groups()" [ngValue]="g.itemGroupId">{{ g.name }}</option>
              </select>
            </div>
            <div class="col-auto pt-4">
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isGroup" name="isGroup" id="isGroup">
                <label class="form-check-label" for="isGroup">Is Group</label>
              </div>
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isActive" name="isActive" id="isActive">
                <label class="form-check-label" for="isActive">Active</label>
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
        <tr *ngFor="let g of groups()">
          <td>{{ g.itemGroupId }}</td>
          <td>{{ g.name }}</td>
          <td>{{ parentName(g.parentItemGroupId) }}</td>
          <td><span class="badge" [class]="g.isGroup ? 'bg-info' : 'bg-secondary'">{{ g.isGroup ? 'Yes' : 'No' }}</span></td>
          <td><span class="badge" [class]="g.isActive ? 'bg-success' : 'bg-secondary'">{{ g.isActive ? 'Yes' : 'No' }}</span></td>
          <td>
            <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(g)">Edit</button>
            <button class="btn btn-sm btn-outline-danger" (click)="delete(g.itemGroupId)">Del</button>
          </td>
        </tr>
      </tbody>
    </table>
  `,
})
export class ItemGroupsComponent implements OnInit {
  api = inject(ApiService);
  groups = signal<ItemGroup[]>([]);
  showForm = false;
  editing = false;
  form: Partial<ItemGroup> = {};

  ngOnInit() { this.load(); }
  load() { this.api.get<ItemGroup[]>('itemgroups').subscribe(d => this.groups.set(d)); }
  openNew() { this.form = { name: '', parentItemGroupId: null, isGroup: false, isActive: true }; this.editing = false; this.showForm = true; }
  edit(g: ItemGroup) { this.form = { ...g }; this.editing = true; this.showForm = true; }
  cancel() { this.showForm = false; }
  parentName(id: number | null) { return id ? (this.groups().find(g => g.itemGroupId === id)?.name ?? '') : '—'; }
  save() {
    const obs = this.editing
      ? this.api.put(`itemgroups/${this.form.itemGroupId}`, this.form)
      : this.api.post('itemgroups', this.form);
    obs.subscribe(() => { this.load(); this.cancel(); });
  }
  delete(id: number) {
    if (confirm('Delete this group?')) this.api.delete(`itemgroups/${id}`).subscribe(() => this.load());
  }
}
