import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';
import { CustomSelectInputComponent, SelectOption } from '../../../shared/components/custom-controls/custom-select-input/custom-select-input.component';

interface ItemGroup { itemGroupId: number; name_EN: string; name_AR?: string; }
interface ItemType  { itemTypeId: number;  name: string; }
interface ItemUOM   { id: number; name_EN: string; name_AR?: string; }

interface Item {
  itemId: number;
  itemCode: string;
  itemName: string;
  itemGroupId: number;
  itemTypeId: number;
  defaultUOMId: number;
  description?: string;
  hasBatch: boolean;
  hasSerial: boolean;
  isActive: boolean;
  isFavorite: boolean;
}

interface SavedRow {
  itemCode: string;
  itemName: string;
  groupName: string;
  typeName: string;
  uomName: string;
}

@Component({
  selector: 'app-items-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './items-operation.component.html',
  styleUrl: './items-operation.component.less',
})
export class ItemsOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit    = signal(false);
  saving    = signal(false);
  savingNew = signal(false);
  savedRows = signal<SavedRow[]>([]);

  groups = signal<ItemGroup[]>([]);
  types  = signal<ItemType[]>([]);
  uoms   = signal<ItemUOM[]>([]);

  form: Partial<Item> = this.blank();

  ngOnInit() {
    this.api.get<ItemGroup[]>('itemgroups').subscribe(d => this.groups.set(d));
    this.api.get<ItemType[]>('itemtypes').subscribe(d => this.types.set(d));
    this.api.get<ItemUOM[]>('uoms').subscribe(d => this.uoms.set(d));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<Item>(`items/${id}`).subscribe(item => this.form = { ...item });
    }
  }

  uomLabel(uom: ItemUOM): string {
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR || '');
  }

  groupLabel(g: ItemGroup): string {
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR || '');
  }

  get groupOptions(): SelectOption[] {
    return this.groups().map(g => ({ value: g.itemGroupId, label: this.groupLabel(g) }));
  }

  get typeOptions(): SelectOption[] {
    return this.types().map(t => ({ value: t.itemTypeId, label: t.name }));
  }

  get uomOptions(): SelectOption[] {
    return this.uoms().map(u => ({ value: u.id, label: this.uomLabel(u) }));
  }

  private blank(): Partial<Item> {
    return { itemId: 0, itemCode: '', itemName: '', itemGroupId: 0, itemTypeId: 1, defaultUOMId: 0, description: '', hasBatch: false, hasSerial: false, isActive: true, isFavorite: false };
  }

  private validate(): boolean {
    const missing: string[] = [];

    if (!this.form.itemCode?.trim())
      missing.push(this.translate.instant('items.code'));

    if (!this.form.itemName?.trim())
      missing.push(this.translate.instant('items.name'));

    if (!this.form.itemGroupId)
      missing.push(this.translate.instant('items.group'));

    if (!this.form.itemTypeId)
      missing.push(this.translate.instant('items.type'));

    if (!this.form.defaultUOMId)
      missing.push(this.translate.instant('items.default_uom'));

    if (missing.length) {
      this.toastr.error(
        missing.join('<br>'),
        this.translate.instant('common.validation_error'),
        { enableHtml: true }
      );
      return false;
    }
    return true;
  }

  private submit(andNew: boolean) {
    if (!this.validate()) return;

    andNew ? this.savingNew.set(true) : this.saving.set(true);

    const req = this.isEdit()
      ? this.api.put<Item>(`items/${this.form.itemId}`, this.form)
      : this.api.post<Item>('items', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          const group = this.groups().find(g => g.itemGroupId === this.form.itemGroupId);
          const type  = this.types().find(t => t.itemTypeId  === this.form.itemTypeId);
          const uom   = this.uoms().find(u => u.id           === this.form.defaultUOMId);
          this.savedRows.update(rows => [...rows, {
            itemCode:  this.form.itemCode  ?? '',
            itemName:  this.form.itemName  ?? '',
            groupName: group ? this.groupLabel(group) : '—',
            typeName:  type?.name          ?? '—',
            uomName:   uom ? this.uomLabel(uom) : '—',
          }]);
          this.form = this.blank();
          this.isEdit.set(false);
          this.savingNew.set(false);
        } else {
          this.back();
        }
      },
      error: () => {
        this.saving.set(false);
        this.savingNew.set(false);
      },
    });
  }

  save()       { this.submit(false); }
  saveAndNew() { this.submit(true);  }

  resetForm() { this.form = this.blank(); }

  back() { this.router.navigate(['/stock/items']); }
}
