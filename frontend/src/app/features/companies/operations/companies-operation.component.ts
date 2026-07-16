import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';

interface Currency { id: number; code: string; name_EN: string; name_AR: string | null; }
interface Company {
  id: number;
  internalCode: string | null;
  name_EN: string;
  name_AR: string | null;
  abbr: string;
  defaultCurrencyId: number | null;
  country: string | null;
  taxId: string | null;
  phone: string | null;
  email: string | null;
  website: string | null;
  address: string | null;
  isActive: boolean;
}
interface SavedRow { internalCode: string; name_EN: string; name_AR: string; abbr: string; }

@Component({
  selector: 'app-companies-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent],
  templateUrl: './companies-operation.component.html',
  styleUrl: './companies-operation.component.less',
})
export class CompaniesOperationComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit     = signal(false);
  saving     = signal(false);
  savingNew  = signal(false);
  savedRows  = signal<SavedRow[]>([]);
  currencies = signal<Currency[]>([]);

  form: Partial<Company> = this.blank();

  ngOnInit() {
    this.api.get<Currency[]>('currencies').subscribe(c => this.currencies.set(c));
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<Company>(`companies/${id}`).subscribe(c => this.form = { ...c });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('companies/nextcode').subscribe(r => this.form.internalCode = r.code);
  }

  private blank(): Partial<Company> {
    return {
      id: 0, internalCode: null, name_EN: '', name_AR: null, abbr: '',
      defaultCurrencyId: null, country: null, taxId: null,
      phone: null, email: null, website: null, address: null,
      isActive: true,
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
    if (!this.form.abbr?.trim()) {
      this.toastr.error(
        this.translate.instant('companies.abbr'),
        this.translate.instant('common.validation_error')
      );
      return false;
    }
    if (!this.form.defaultCurrencyId) {
      this.toastr.error(
        this.translate.instant('companies.default_currency'),
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
      ? this.api.put<Company>(`companies/${this.form.id}`, this.form)
      : this.api.post<Company>('companies', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          this.savedRows.update(rows => [...rows, { internalCode: this.form.internalCode ?? '', name_EN: this.form.name_EN ?? '', name_AR: this.form.name_AR ?? '', abbr: this.form.abbr ?? '' }]);
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

  currencyLabel(c: Currency): string {
    return this.isRtl ? `${c.code} — ${c.name_AR || c.name_EN}` : `${c.code} — ${c.name_EN}`;
  }

  resetForm() { this.form = this.blank(); this.loadNextCode(); }
  back() { this.router.navigate(['/buying/companies']); }
}
