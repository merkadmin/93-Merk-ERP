import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';

interface SupplierType { id: number; name_EN: string; name_AR: string | null; }
interface Currency { id: number; code: string; name_EN: string; name_AR: string | null; }
interface Supplier {
  id: number;
  internalCode: string | null;
  name_EN: string;
  name_AR: string | null;
  supplierTypeId: number | null;
  defaultCurrencyId: number | null;
  country: string | null;
  taxId: string | null;
  phone: string | null;
  email: string | null;
  website: string | null;
  address: string | null;
  contactPersonName: string | null;
  contactMobile: string | null;
  contactEmail: string | null;
  isOnHold: boolean;
  notes: string | null;
  isActive: boolean;
}
interface SavedRow { internalCode: string; name_EN: string; name_AR: string; supplierTypeName: string; }

@Component({
  selector: 'app-suppliers-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent],
  templateUrl: './suppliers-operation.component.html',
  styleUrl: './suppliers-operation.component.less',
})
export class SuppliersOperationComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit         = signal(false);
  saving         = signal(false);
  savingNew      = signal(false);
  savedRows      = signal<SavedRow[]>([]);
  supplierTypes  = signal<SupplierType[]>([]);
  currencies     = signal<Currency[]>([]);

  form: Partial<Supplier> = this.blank();

  ngOnInit() {
    this.api.get<SupplierType[]>('suppliertypes').subscribe(t => this.supplierTypes.set(t));
    this.api.get<Currency[]>('currencies').subscribe(c => this.currencies.set(c));
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<Supplier>(`suppliers/${id}`).subscribe(s => this.form = { ...s });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('suppliers/nextcode').subscribe(r => this.form.internalCode = r.code);
  }

  private blank(): Partial<Supplier> {
    return {
      id: 0, internalCode: null, name_EN: '', name_AR: null,
      supplierTypeId: null, defaultCurrencyId: null, country: null, taxId: null,
      phone: null, email: null, website: null, address: null,
      contactPersonName: null, contactMobile: null, contactEmail: null,
      isOnHold: false, notes: null, isActive: true,
    };
  }

  private validate(): boolean {
    if (!this.form.name_EN?.trim()) {
      this.toastr.error(
        this.translate.instant('common.name_en'),
        this.translate.instant('common.validation_error')
      );
      return false;
    }
    return true;
  }

  private submit(andNew: boolean) {
    if (!this.validate()) return;
    andNew ? this.savingNew.set(true) : this.saving.set(true);

    const req = this.isEdit()
      ? this.api.put<Supplier>(`suppliers/${this.form.id}`, this.form)
      : this.api.post<Supplier>('suppliers', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          this.savedRows.update(rows => [...rows, { internalCode: this.form.internalCode ?? '', name_EN: this.form.name_EN ?? '', name_AR: this.form.name_AR ?? '', supplierTypeName: this.typeLabel(this.form.supplierTypeId) }]);
          this.form = this.blank();
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

  typeLabel(id: number | null | undefined): string {
    if (id == null) return '—';
    const t = this.supplierTypes().find(x => x.id === id);
    if (!t) return '—';
    return this.isRtl ? (t.name_AR || t.name_EN) : t.name_EN;
  }

  currencyLabel(c: Currency): string {
    return this.isRtl ? `${c.code} — ${c.name_AR || c.name_EN}` : `${c.code} — ${c.name_EN}`;
  }

  resetForm() { this.form = this.blank(); this.loadNextCode(); }
  back() { this.router.navigate(['/buying/suppliers']); }
}
