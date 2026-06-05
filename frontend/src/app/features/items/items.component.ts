import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface Item { itemId: number; itemCode: string; itemName: string; itemGroupId: number; itemTypeId: number; defaultUOMId: number; description?: string; hasBatch: boolean; hasSerial: boolean; isActive: boolean; itemGroup?: { name: string }; itemType?: { name: string }; defaultUOM?: { name: string }; }
interface ItemGroup { itemGroupId: number; name: string; }
interface ItemType  { itemTypeId: number; name: string; }
interface UOM       { uomId: number; name: string; }

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  templateUrl: './items.component.html',
})
export class ItemsComponent implements OnInit {
  api    = inject(ApiService);
  translate = inject(TranslateService);
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
    if (confirm(this.translate.instant('items.delete_confirm')))
      this.api.delete(`items/${id}`).subscribe(() => this.load());
  }
}

