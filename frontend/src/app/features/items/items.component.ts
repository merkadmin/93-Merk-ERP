import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { ApiService } from '../../core/api.service';

interface Item { itemId: number; itemCode: string; itemName: string; itemGroupId: number; itemTypeId: number; defaultUOMId: number; description?: string; hasBatch: boolean; hasSerial: boolean; isActive: boolean; itemGroup?: { name: string }; itemType?: { name: string }; defaultUOM?: { name: string }; }
interface ItemGroup { itemGroupId: number; name: string; }
interface ItemType  { itemTypeId: number; name: string; }
interface UOM       { uomId: number; name: string; }

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="bi bi-box-seam me-2"></i>Items</h5>
      <button class="btn btn-sm btn-primary" (click)="openNew()"><i class="bi bi-plus"></i> Add Item</button>
    </div>

    <div *ngIf="showForm" class="card mb-3 border-primary">
      <div class="card-body">
        <form (ngSubmit)="save()">
          <div class="row g-2">
            <div class="col-md-2">
              <label class="form-label">Code <span class="text-danger">*</span></label>
              <input class="form-control form-control-sm" [(ngModel)]="form.itemCode" name="itemCode" required>
            </div>
            <div class="col-md-3">
              <label class="form-label">Name <span class="text-danger">*</span></label>
              <input class="form-control form-control-sm" [(ngModel)]="form.itemName" name="itemName" required>
            </div>
            <div class="col-md-2">
              <label class="form-label">Group</label>
              <select class="form-select form-select-sm" [(ngModel)]="form.itemGroupId" name="groupId">
                <option [ngValue]="0">— Select —</option>
                <option *ngFor="let g of groups()" [ngValue]="g.itemGroupId">{{ g.name }}</option>
              </select>
            </div>
            <div class="col-md-2">
              <label class="form-label">Type</label>
              <select class="form-select form-select-sm" [(ngModel)]="form.itemTypeId" name="typeId">
                <option *ngFor="let t of types()" [ngValue]="t.itemTypeId">{{ t.name }}</option>
              </select>
            </div>
            <div class="col-md-2">
              <label class="form-label">Default UOM</label>
              <select class="form-select form-select-sm" [(ngModel)]="form.defaultUOMId" name="uomId">
                <option [ngValue]="0">— Select —</option>
                <option *ngFor="let u of uoms()" [ngValue]="u.uomId">{{ u.name }}</option>
              </select>
            </div>
            <div class="col-md-5">
              <label class="form-label">Description</label>
              <input class="form-control form-control-sm" [(ngModel)]="form.description" name="description">
            </div>
            <div class="col-auto pt-4">
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.hasBatch" name="hasBatch" id="hasBatch">
                <label class="form-check-label" for="hasBatch">Batch</label>
              </div>
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.hasSerial" name="hasSerial" id="hasSerial">
                <label class="form-check-label" for="hasSerial">Serial</label>
              </div>
              <div class="form-check form-check-inline">
                <input class="form-check-input" type="checkbox" [(ngModel)]="form.isActive" name="isActive" id="isActive">
                <label class="form-check-label" for="isActive">Active</label>
              </div>
            </div>
          </div>
          <div class="mt-3">
            <button type="submit" class="btn btn-sm btn-success me-2">Save</button>
            <button type="button" class="btn btn-sm btn-secondary" (click)="cancel()">Cancel</button>
          </div>
        </form>
      </div>
    </div>

    <table class="table table-sm table-hover table-bordered">
      <thead class="table-dark">
        <tr><th>Code</th><th>Name</th><th>Group</th><th>Type</th><th>UOM</th><th>Batch</th><th>Serial</th><th>Active</th><th></th></tr>
      </thead>
      <tbody>
        <tr *ngFor="let item of items()">
          <td class="fw-semibold">{{ item.itemCode }}</td>
          <td>{{ item.itemName }}</td>
          <td>{{ item.itemGroup?.name }}</td>
          <td>{{ item.itemType?.name }}</td>
          <td>{{ item.defaultUOM?.name }}</td>
          <td><span class="badge" [class]="item.hasBatch ? 'bg-info' : 'bg-light text-dark'">{{ item.hasBatch ? 'Yes' : 'No' }}</span></td>
          <td><span class="badge" [class]="item.hasSerial ? 'bg-info' : 'bg-light text-dark'">{{ item.hasSerial ? 'Yes' : 'No' }}</span></td>
          <td><span class="badge" [class]="item.isActive ? 'bg-success' : 'bg-secondary'">{{ item.isActive ? 'Yes' : 'No' }}</span></td>
          <td>
            <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(item)">Edit</button>
            <button class="btn btn-sm btn-outline-danger" (click)="delete(item.itemId)">Del</button>
          </td>
        </tr>
      </tbody>
    </table>
  `,
})
export class ItemsComponent implements OnInit {
  api    = inject(ApiService);
  items  = signal<Item[]>([]);
  groups = signal<ItemGroup[]>([]);
  types  = signal<ItemType[]>([]);
  uoms   = signal<UOM[]>([]);
  showForm = false;
  editing  = false;
  form: Partial<Item> = {};

  ngOnInit() {
    this.load();
    this.api.get<ItemGroup[]>('itemgroups').subscribe(d => this.groups.set(d));
    this.api.get<ItemType[]>('itemtypes').subscribe(d => this.types.set(d));
    this.api.get<UOM[]>('uoms').subscribe(d => this.uoms.set(d));
  }

  load() { this.api.get<Item[]>('items').subscribe(d => this.items.set(d)); }

  openNew() {
    this.form = { itemCode: '', itemName: '', itemGroupId: 0, itemTypeId: 1, defaultUOMId: 0, hasBatch: false, hasSerial: false, isActive: true };
    this.editing = false; this.showForm = true;
  }

  edit(item: Item) {
    this.form = { itemId: item.itemId, itemCode: item.itemCode, itemName: item.itemName, itemGroupId: item.itemGroupId, itemTypeId: item.itemTypeId, defaultUOMId: item.defaultUOMId, description: item.description, hasBatch: item.hasBatch, hasSerial: item.hasSerial, isActive: item.isActive };
    this.editing = true; this.showForm = true;
  }

  cancel() { this.showForm = false; }

  save() {
    const obs = this.editing
      ? this.api.put(`items/${this.form.itemId}`, this.form)
      : this.api.post('items', this.form);
    obs.subscribe(() => { this.load(); this.cancel(); });
  }

  delete(id: number) {
    if (confirm('Delete this item?')) this.api.delete(`items/${id}`).subscribe(() => this.load());
  }
}
