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

interface ItemGroup   { itemGroupId: number; name_EN: string; name_AR?: string; }
interface ItemType    { itemTypeId: number;  name: string; }
interface ItemUOM     { id: number; name_EN: string; name_AR?: string; }
interface BarcodeType { barcodeTypeId: number; name: string; }

interface ItemBarcode {
  id: number;
  itemId: number;
  barcodeTypeId: number;
  uomId: number;
  barcode: string;
  barcodeType?: BarcodeType;
  uom?: ItemUOM;
}

interface Item {
  id: number;
  internalCode: string;
  name_EN: string;
  name_AR?: string;
  itemGroupId: number;
  itemTypeId: number;
  defaultUOMId: number;
  defaultPurchaseUOMId?: number;
  acceptSelling: boolean;
  defaultSellingUOMId?: number;
  description?: string;
  openingStock?: number;
  expirationDate?: string;
  minOrderQuantity?: number;
  safetyStock?: number;
  isActive: boolean;
  isFavorite: boolean;
}

interface SavedRow {
  internalCode: string;
  name_EN: string;
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

  isEdit        = signal(false);
  saving        = signal(false);
  savingNew     = signal(false);
  addingBarcode = signal(false);
  savedRows     = signal<SavedRow[]>([]);
  activeTab     = signal<'general' | 'inventory' | 'barcodes'>('general');

  groups       = signal<ItemGroup[]>([]);
  types        = signal<ItemType[]>([]);
  uoms         = signal<ItemUOM[]>([]);
  barcodeTypes = signal<BarcodeType[]>([]);
  barcodes     = signal<ItemBarcode[]>([]);

  form: Partial<Item> = this.blank();
  newBarcode = this.blankBarcode();

  ngOnInit() {
    this.api.get<ItemGroup[]>('itemgroups').subscribe(d => this.groups.set(d));
    this.api.get<ItemType[]>('itemtypes').subscribe(d => this.types.set(d));
    this.api.get<ItemUOM[]>('uoms').subscribe(d => this.uoms.set(d));
    this.api.get<BarcodeType[]>('barcodetypes').subscribe(d => this.barcodeTypes.set(d));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<Item>(`items/${id}`).subscribe(item => this.form = { ...item });
      this.loadBarcodes(id);
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('items/nextcode').subscribe(r => {
      this.form.internalCode = r.code;
    });
  }

  private loadBarcodes(itemId: number) {
    this.api.get<ItemBarcode[]>(`items/${itemId}/barcodes`).subscribe(d => this.barcodes.set(d));
  }

  private blankBarcode() {
    return { barcodeTypeId: 0, uomId: 0, barcode: '' };
  }

  addBarcode() {
    if (!this.newBarcode.barcodeTypeId || !this.newBarcode.uomId || !this.newBarcode.barcode.trim()) {
      this.toastr.warning(this.translate.instant('items.barcode_fill_all'));
      return;
    }
    this.addingBarcode.set(true);
    this.api.post<ItemBarcode>(`items/${this.form.id}/barcodes`, this.newBarcode).subscribe({
      next: () => {
        this.loadBarcodes(this.form.id!);
        this.newBarcode = this.blankBarcode();
        this.addingBarcode.set(false);
      },
      error: () => this.addingBarcode.set(false),
    });
  }

  removeBarcode(id: number) {
    this.api.delete(`items/${this.form.id}/barcodes/${id}`).subscribe(() =>
      this.barcodes.update(list => list.filter(b => b.id !== id))
    );
  }

  uomLabel(uom: ItemUOM): string {
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR || '');
  }

  groupLabel(g: ItemGroup): string {
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR || '');
  }

  barcodeTypeName(id: number): string {
    return this.barcodeTypes().find(b => b.barcodeTypeId === id)?.name ?? '—';
  }

  uomName(id: number): string {
    const u = this.uoms().find(u => u.id === id);
    return u ? this.uomLabel(u) : '—';
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

  get barcodeTypeOptions(): SelectOption[] {
    return this.barcodeTypes().map(b => ({ value: b.barcodeTypeId, label: b.name }));
  }

  private blank(): Partial<Item> {
    return {
      id: 0, internalCode: '', name_EN: '', name_AR: '',
      itemGroupId: 0, itemTypeId: 1,
      defaultUOMId: 0, defaultPurchaseUOMId: undefined, defaultSellingUOMId: undefined,
      acceptSelling: true,
      description: '', openingStock: undefined, expirationDate: undefined,
      minOrderQuantity: undefined, safetyStock: undefined,
      isActive: true, isFavorite: false,
    };
  }

  private validate(): boolean {
    const missing: string[] = [];

    if (!this.form.internalCode?.trim())
      missing.push(this.translate.instant('common.internal_code'));

    if (!this.form.name_EN?.trim())
      missing.push(`${this.translate.instant('common.name')} (EN)`);

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
      ? this.api.put<Item>(`items/${this.form.id}`, this.form)
      : this.api.post<Item>('items', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          const group = this.groups().find(g => g.itemGroupId === this.form.itemGroupId);
          const type  = this.types().find(t => t.itemTypeId  === this.form.itemTypeId);
          const uom   = this.uoms().find(u => u.id           === this.form.defaultUOMId);
          this.savedRows.update(rows => [...rows, {
            internalCode: this.form.internalCode ?? '',
            name_EN:      this.form.name_EN      ?? '',
            groupName:    group ? this.groupLabel(group) : '—',
            typeName:     type?.name             ?? '—',
            uomName:      uom ? this.uomLabel(uom) : '—',
          }]);
          this.form = this.blank();
          this.isEdit.set(false);
          this.savingNew.set(false);
          this.activeTab.set('general');
          this.loadNextCode();
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

  resetForm() { this.form = this.blank(); this.activeTab.set('general'); this.loadNextCode(); }

  back() { this.router.navigate(['/stock/items']); }
}
