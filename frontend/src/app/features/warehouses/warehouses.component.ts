import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface Warehouse { warehouseId: number; name: string; parentWarehouseId: number | null; isGroup: boolean; isActive: boolean; }

@Component({
  selector: 'app-warehouses',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  templateUrl: './warehouses.component.html',
  styleUrl: './warehouses.component.less',
})
export class WarehousesComponent implements OnInit {
  api = inject(ApiService);
  translate = inject(TranslateService);
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
    if (confirm(this.translate.instant('warehouses.delete_confirm')))
      this.api.delete(`warehouses/${id}`).subscribe(() => this.load());
  }
}


