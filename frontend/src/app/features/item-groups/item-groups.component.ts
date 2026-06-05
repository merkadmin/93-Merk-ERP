import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface ItemGroup { itemGroupId: number; name: string; parentItemGroupId: number | null; isGroup: boolean; isActive: boolean; }

@Component({
  selector: 'app-item-groups',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="ki-outline ki-category fs-2 me-2"></i>{{ 'item_groups.title' | translate }}</h5>
      <button class="btn btn-sm btn-primary" (click)="openNew()"><i class="ki-outline ki-plus fs-4"></i> {{ 'common.add' | translate }}</button>
    </div>

    <div *ngIf="showForm" class="card mb-3 border-primary">
      <div class="card-body">
        <form (ngSubmit)="save()">
          <div class="row g-2 align-items-end">
            <div class="col-md-4">
              <label class="form-label">{{ 'common.name' | translate }}</label>
              <input class="form-control form-control-sm" [(ngModel)]="form.name" name="name" required>
            </div>
            <div class="col-md-3">
              <label class="form-label">{{ 'item_groups.parent_group' | translate }}</label>
              <select class="form-select form-select-sm" [(ngModel)]="form.parentItemGroupId" name="parent">
                <option [ngValue]="null">{{ 'common.none' | translate }}</option>
                <option *ngFor="let g of groups()" [ngValue]="g.itemGroupId">{{ g.name }}</option>
              </select>
            </div>
            <div class="col-auto pt-4">
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isGroup" name="isGroup" id="isGroup">
                <label class="form-check-label" for="isGroup">{{ 'common.is_group' | translate }}</label>
              </div>
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isActive" name="isActive" id="isActive">
                <label class="form-check-label" for="isActive">{{ 'common.active' | translate }}</label>
              </div>
            </div>
            <div class="col-auto">
              <button type="submit" class="btn btn-sm btn-success me-1">{{ 'common.save' | translate }}</button>
              <button type="button" class="btn btn-sm btn-secondary" (click)="cancel()">{{ 'common.cancel' | translate }}</button>
            </div>
          </div>
        </form>
      </div>
    </div>

    <table class="table table-sm table-hover table-bordered">
      <thead class="table-dark">
        <tr>
          <th>#</th>
          <th>{{ 'common.name' | translate }}</th>
          <th>{{ 'common.parent' | translate }}</th>
          <th>{{ 'common.is_group' | translate }}</th>
          <th>{{ 'common.active' | translate }}</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let g of groups()">
          <td>{{ g.itemGroupId }}</td>
          <td>{{ g.name }}</td>
          <td>{{ parentName(g.parentItemGroupId) }}</td>
          <td><span class="badge" [class]="g.isGroup ? 'bg-info' : 'bg-secondary'">{{ (g.isGroup ? 'common.yes' : 'common.no') | translate }}</span></td>
          <td><span class="badge" [class]="g.isActive ? 'bg-success' : 'bg-secondary'">{{ (g.isActive ? 'common.yes' : 'common.no') | translate }}</span></td>
          <td>
            <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(g)">{{ 'common.edit' | translate }}</button>
            <button class="btn btn-sm btn-outline-danger" (click)="delete(g.itemGroupId)">{{ 'common.delete' | translate }}</button>
          </td>
        </tr>
      </tbody>
    </table>
  `,
})
export class ItemGroupsComponent implements OnInit {
  api = inject(ApiService);
  translate = inject(TranslateService);
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
    if (confirm(this.translate.instant('item_groups.delete_confirm')))
      this.api.delete(`itemgroups/${id}`).subscribe(() => this.load());
  }
}
