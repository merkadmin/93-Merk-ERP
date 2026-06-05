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
  templateUrl: './item-groups.component.html',
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

