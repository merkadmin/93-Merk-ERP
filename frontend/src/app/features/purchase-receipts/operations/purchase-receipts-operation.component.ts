import { DecimalPipe, DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { AuthService } from '../../../core/auth.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';
import { CustomSelectInputComponent, SelectOption } from '../../../shared/components/custom-controls/custom-select-input/custom-select-input.component';

interface Supplier  { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; defaultCurrencyId: number | null; }
interface Company   { id: number; name_EN: string; name_AR: string | null; defaultCurrencyId: number | null; }
interface Currency  { id: number; code: string; name_EN: string; name_AR: string | null; }
interface Warehouse { id: number; name_EN: string; name_AR: string | null; isParent?: boolean; }
interface Item      { id: number; internalCode: string; name_EN: string; name_AR?: string; defaultUOMId: number; }
interface UOM       { id: number; name_EN: string; name_AR: string; }

interface PRForm {
  id: number;
  internalCode: string;
  supplierId: number | null;
  companyId: number | null;
  currencyId: number | null;
  supplierDeliveryNote: string;
  postingDate: string;
  postingTime: string;
  setWarehouseId: number | null;
  remarks: string;
}

interface ItemRowForm { itemId: number; uomId: number; quantity: number; rate: number; }
interface TaxRowForm  { description: string; rate: number | null; amount: number; }

interface ApiItemRow { id: number; purchaseReceiptId: number; itemId: number; uomId: number; quantity: number; rate: number; amount: number; }
interface ApiTaxRow  { id: number; purchaseReceiptId: number; description: string; rate: number | null; amount: number; }

interface SavedRow { internalCode: string; supplierName: string; postingDate: string; itemCount: number; }

@Component({
  selector: 'app-purchase-receipts-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, DecimalPipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './purchase-receipts-operation.component.html',
  styleUrl: './purchase-receipts-operation.component.less',
})
export class PurchaseReceiptsOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private auth      = inject(AuthService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit       = signal(false);
  readonly     = signal(false);
  saving       = signal(false);
  savingNew    = signal(false);
  addingItem   = signal(false);
  addingTax    = signal(false);

  suppliers  = signal<Supplier[]>([]);
  companies  = signal<Company[]>([]);
  currencies = signal<Currency[]>([]);
  warehouses = signal<Warehouse[]>([]);
  items      = signal<Item[]>([]);
  uoms       = signal<UOM[]>([]);

  apiItems     = signal<ApiItemRow[]>([]);
  pendingItems = signal<ItemRowForm[]>([]);
  apiTaxes     = signal<ApiTaxRow[]>([]);
  pendingTaxes = signal<TaxRowForm[]>([]);
  savedRows    = signal<SavedRow[]>([]);

  form: PRForm = this.blank();
  newItem: ItemRowForm = this.blankItem();
  newTax: TaxRowForm = this.blankTax();

  get displayItems(): (ItemRowForm | ApiItemRow)[] {
    return this.isEdit() ? this.apiItems() : this.pendingItems();
  }
  get displayTaxes(): (TaxRowForm | ApiTaxRow)[] {
    return this.isEdit() ? this.apiTaxes() : this.pendingTaxes();
  }

  get itemsTotal(): number {
    return this.displayItems.reduce((sum, r: any) => sum + (r.amount ?? (r.quantity * r.rate)), 0);
  }
  get taxesTotal(): number {
    return this.displayTaxes.reduce((sum, r: any) => sum + (r.amount ?? 0), 0);
  }
  get grandTotal(): number {
    return this.itemsTotal + this.taxesTotal;
  }

  get supplierOptions(): SelectOption[] {
    return this.suppliers().map(s => ({
      value: s.id,
      label: this.isRtl ? (s.name_AR || s.name_EN) : s.name_EN,
      sublabel: s.internalCode ?? undefined,
    }));
  }
  get warehouseOptions(): SelectOption[] {
    return this.warehouses().filter(w => !w.isParent)
      .map(w => ({ value: w.id, label: this.isRtl ? (w.name_AR || w.name_EN) : w.name_EN }));
  }
  get itemOptions(): SelectOption[] {
    return this.items().map(i => ({
      value: i.id,
      label: this.isRtl ? (i.name_AR || i.name_EN) : i.name_EN,
      sublabel: i.internalCode,
    }));
  }
  get uomOptions(): SelectOption[] {
    return this.uoms().map(u => ({ value: u.id, label: this.isRtl ? u.name_AR : u.name_EN }));
  }

  supplierName(id: number | null | undefined): string {
    const s = this.suppliers().find(x => x.id === id);
    return s ? (this.isRtl ? (s.name_AR || s.name_EN) : s.name_EN) : '—';
  }
  companyName(id: number | null | undefined): string {
    const c = this.companies().find(x => x.id === id);
    return c ? (this.isRtl ? (c.name_AR || c.name_EN) : c.name_EN) : '—';
  }
  currencyLabel(c: Currency): string {
    return this.isRtl ? `${c.code} — ${c.name_AR || c.name_EN}` : `${c.code} — ${c.name_EN}`;
  }
  itemName(id: number): string {
    const i = this.items().find(x => x.id === id);
    return i ? (this.isRtl ? (i.name_AR || i.name_EN) : i.name_EN) : '—';
  }
  uomName(id: number): string {
    const u = this.uoms().find(x => x.id === id);
    return u ? (this.isRtl ? u.name_AR : u.name_EN) : '—';
  }

  ngOnInit() {
    this.api.get<Supplier[]>('suppliers').subscribe(d => this.suppliers.set(d));
    this.api.get<Company[]>('companies').subscribe(d => this.companies.set(d));
    this.api.get<Currency[]>('currencies').subscribe(d => this.currencies.set(d));
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
    this.api.get<Item[]>('items').subscribe(d => this.items.set(d));
    this.api.get<UOM[]>('uoms').subscribe(d => this.uoms.set(d));

    if (this.route.snapshot.queryParamMap.get('readonly') === 'true')
      this.readonly.set(true);

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<any>(`purchasereceipts/${id}`).subscribe(data => {
        this.form = {
          id: data.id,
          internalCode: data.internalCode ?? '',
          supplierId: data.supplierId,
          companyId: data.companyId,
          currencyId: data.currencyId,
          supplierDeliveryNote: data.supplierDeliveryNote ?? '',
          postingDate: data.postingDate,
          postingTime: data.postingTime?.substring(0, 5) ?? '',
          setWarehouseId: data.setWarehouseId ?? null,
          remarks: data.remarks ?? '',
        };
        this.apiItems.set(data.items ?? []);
        this.apiTaxes.set(data.taxes ?? []);
      });
    } else {
      this.loadNextCode();
    }
  }

  onSupplierChange(id: number | string | null) {
    this.form.supplierId = id ? +id : null;
    if (!this.form.currencyId) {
      const s = this.suppliers().find(x => x.id === this.form.supplierId);
      if (s?.defaultCurrencyId) this.form.currencyId = s.defaultCurrencyId;
    }
  }

  onItemPicked(id: number | string | null) {
    this.newItem.itemId = id ? +id : 0;
    const item = this.items().find(x => x.id === this.newItem.itemId);
    if (item && !this.newItem.uomId) this.newItem.uomId = item.defaultUOMId;
  }

  addItemRow() {
    if (!this.newItem.itemId || !this.newItem.uomId || !this.newItem.quantity || this.newItem.rate == null) {
      this.toastr.warning(this.translate.instant('purchase_receipts.item_fill_all'));
      return;
    }

    if (!this.isEdit()) {
      this.pendingItems.update(list => [...list, { ...this.newItem }]);
      this.newItem = this.blankItem();
      return;
    }

    this.addingItem.set(true);
    this.api.post<ApiItemRow>(`purchasereceipts/${this.form.id}/items`, this.newItem).subscribe({
      next: saved => {
        this.apiItems.update(list => [...list, saved]);
        this.newItem = this.blankItem();
        this.addingItem.set(false);
      },
      error: () => this.addingItem.set(false),
    });
  }

  removePendingItem(index: number) {
    this.pendingItems.update(list => list.filter((_, i) => i !== index));
  }
  removeApiItem(rowId: number) {
    this.api.delete(`purchasereceipts/${this.form.id}/items/${rowId}`)
      .subscribe(() => this.apiItems.update(list => list.filter(r => r.id !== rowId)));
  }

  addTaxRow() {
    if (!this.newTax.description?.trim()) {
      this.toastr.warning(this.translate.instant('purchase_receipts.tax_fill_description'));
      return;
    }
    if (this.newTax.rate != null) {
      this.newTax.amount = +(this.itemsTotal * (this.newTax.rate / 100)).toFixed(2);
    }
    if (!this.newTax.amount) {
      this.toastr.warning(this.translate.instant('purchase_receipts.tax_fill_amount'));
      return;
    }

    if (!this.isEdit()) {
      this.pendingTaxes.update(list => [...list, { ...this.newTax }]);
      this.newTax = this.blankTax();
      return;
    }

    this.addingTax.set(true);
    this.api.post<ApiTaxRow>(`purchasereceipts/${this.form.id}/taxes`, this.newTax).subscribe({
      next: saved => {
        this.apiTaxes.update(list => [...list, saved]);
        this.newTax = this.blankTax();
        this.addingTax.set(false);
      },
      error: () => this.addingTax.set(false),
    });
  }

  removePendingTax(index: number) {
    this.pendingTaxes.update(list => list.filter((_, i) => i !== index));
  }
  removeApiTax(rowId: number) {
    this.api.delete(`purchasereceipts/${this.form.id}/taxes/${rowId}`)
      .subscribe(() => this.apiTaxes.update(list => list.filter(r => r.id !== rowId)));
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('purchasereceipts/nextcode').subscribe(r => { this.form.internalCode = r.code; });
  }

  private blank(): PRForm {
    const today = new Date().toISOString().split('T')[0];
    const now = new Date().toTimeString().substring(0, 5);
    return {
      id: 0, internalCode: '', supplierId: null, companyId: null, currencyId: null,
      supplierDeliveryNote: '', postingDate: today, postingTime: now,
      setWarehouseId: null, remarks: '',
    };
  }
  private blankItem(): ItemRowForm { return { itemId: 0, uomId: 0, quantity: 0, rate: 0 }; }
  private blankTax(): TaxRowForm { return { description: '', rate: null, amount: 0 }; }

  private validate(): boolean {
    const missing: string[] = [];
    if (!this.form.internalCode?.trim()) missing.push(this.translate.instant('common.internal_code'));
    if (!this.form.supplierId) missing.push(this.translate.instant('purchase_receipts.supplier'));
    if (!this.form.companyId) missing.push(this.translate.instant('purchase_receipts.company'));
    if (!this.form.currencyId) missing.push(this.translate.instant('purchase_receipts.currency'));
    if (!this.form.postingDate) missing.push(this.translate.instant('purchase_receipts.posting_date'));

    if (missing.length) {
      this.toastr.error(missing.join('<br>'), this.translate.instant('common.validation_error'), { enableHtml: true });
      return false;
    }
    return true;
  }

  private submit(andNew: boolean) {
    if (!this.validate()) return;
    andNew ? this.savingNew.set(true) : this.saving.set(true);

    const userId = this.auth.user()?.id ?? null;

    const req = this.isEdit()
      ? this.api.put<any>(`purchasereceipts/${this.form.id}`, {
          internalCode: this.form.internalCode,
          supplierId: this.form.supplierId,
          companyId: this.form.companyId,
          currencyId: this.form.currencyId,
          supplierDeliveryNote: this.form.supplierDeliveryNote || null,
          postingDate: this.form.postingDate,
          postingTime: this.form.postingTime + ':00',
          setWarehouseId: this.form.setWarehouseId,
          remarks: this.form.remarks || null,
        })
      : this.api.post<any>('purchasereceipts', {
          internalCode: this.form.internalCode,
          supplierId: this.form.supplierId,
          companyId: this.form.companyId,
          currencyId: this.form.currencyId,
          supplierDeliveryNote: this.form.supplierDeliveryNote || null,
          postingDate: this.form.postingDate,
          postingTime: this.form.postingTime + ':00',
          setWarehouseId: this.form.setWarehouseId,
          remarks: this.form.remarks || null,
          insertedBy: userId,
          items: this.pendingItems(),
          taxes: this.pendingTaxes(),
        });

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          this.savedRows.update(rows => [...rows, {
            internalCode: this.form.internalCode,
            supplierName: this.supplierName(this.form.supplierId),
            postingDate: this.form.postingDate,
            itemCount: this.pendingItems().length,
          }]);
          this.form = this.blank();
          this.pendingItems.set([]);
          this.pendingTaxes.set([]);
          this.isEdit.set(false);
          this.savingNew.set(false);
          this.loadNextCode();
        } else {
          this.back();
        }
      },
      error: () => { this.saving.set(false); this.savingNew.set(false); },
    });
  }

  save() { this.submit(false); }
  saveAndNew() { this.submit(true); }

  resetForm() {
    this.form = this.blank();
    this.pendingItems.set([]);
    this.pendingTaxes.set([]);
    this.loadNextCode();
  }

  back() { this.router.navigate(['/stock/purchase-receipt']); }
}
