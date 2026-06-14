import { DOCUMENT } from '@angular/common';
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

interface TxnType   { id: number; name_EN: string; name_AR: string; }
interface TxnStatus { id: number; name_EN: string; name_AR: string; }
interface Warehouse  { id: number; name_EN: string; name_AR: string | null; }

interface SRTForm {
  id: number;
  stockTransactionTypeId: number;
  stockTransactionStatusId: number;
  internalCode: string;
  postingDate: string;
  postingTime: string;
  setWarehouseId: number | null;
}

interface SavedRow {
  internalCode: string;
  txnType: string;
  status: string;
  postingDate: string;
}

@Component({
  selector: 'app-stock-reconciliation-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './stock-reconciliation-operation.component.html',
  styleUrl: './stock-reconciliation-operation.component.less',
})
export class StockReconciliationOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private auth      = inject(AuthService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit    = signal(false);
  saving    = signal(false);
  savingNew = signal(false);

  txnTypes    = signal<TxnType[]>([]);
  txnStatuses = signal<TxnStatus[]>([]);
  warehouses  = signal<Warehouse[]>([]);

  savedRows = signal<SavedRow[]>([]);

  form: SRTForm = this.blank();

  get allWarehouseOptions(): SelectOption[] {
    return this.warehouses()
      .map(w => ({ value: w.id, label: this.isRtl ? (w.name_AR || w.name_EN) : w.name_EN }));
  }

  ngOnInit() {
    this.api.get<TxnType[]>('stocktransactiontypes').subscribe(d => this.txnTypes.set(d));
    this.api.get<TxnStatus[]>('stocktransactionstatuses').subscribe(d => this.txnStatuses.set(d));
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<any>(`stockreconciliationtransactions/${id}`).subscribe(data => {
        this.form = {
          id:                      data.id,
          stockTransactionTypeId:   data.stockTransactionTypeId,
          stockTransactionStatusId: data.stockTransactionStatusId,
          internalCode:             data.internalCode ?? '',
          postingDate:              data.postingDate,
          postingTime:              data.postingTime?.substring(0, 5) ?? '',
          setWarehouseId:           data.setWarehouseId ?? null,
        };
      });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('stockreconciliationtransactions/nextcode')
      .subscribe(r => { this.form.internalCode = r.code; });
  }

  private blank(): SRTForm {
    const today = new Date().toISOString().split('T')[0];
    const now   = new Date().toTimeString().substring(0, 5);
    return {
      id: 0,
      stockTransactionTypeId:   2,
      stockTransactionStatusId: 1,
      internalCode: '',
      postingDate:  today,
      postingTime:  now,
      setWarehouseId: null,
    };
  }

  private validate(): boolean {
    const missing: string[] = [];
    if (!this.form.internalCode?.trim())
      missing.push(this.translate.instant('common.internal_code'));
    if (!this.form.stockTransactionTypeId)
      missing.push(this.translate.instant('stock_reconciliation.txn_type'));
    if (!this.form.stockTransactionStatusId)
      missing.push(this.translate.instant('stock_reconciliation.status'));
    if (!this.form.postingDate)
      missing.push(this.translate.instant('stock_reconciliation.posting_date'));

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
      ? this.api.put<any>(`stockreconciliationtransactions/${this.form.id}`, {
          stockTransactionTypeId:   this.form.stockTransactionTypeId,
          stockTransactionStatusId: this.form.stockTransactionStatusId,
          internalCode:             this.form.internalCode,
          postingDate:              this.form.postingDate,
          postingTime:              this.form.postingTime + ':00',
          setWarehouseId:           this.form.setWarehouseId,
        })
      : this.api.post<any>('stockreconciliationtransactions', {
          stockTransactionTypeId:   this.form.stockTransactionTypeId,
          stockTransactionStatusId: this.form.stockTransactionStatusId,
          internalCode:             this.form.internalCode,
          postingDate:              this.form.postingDate,
          postingTime:              this.form.postingTime + ':00',
          setWarehouseId:           this.form.setWarehouseId,
          insertedBy:               userId,
          details:                  [],
        });

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          const typeName = this.txnTypes().find(t => t.id === this.form.stockTransactionTypeId);
          const statusName = this.txnStatuses().find(s => s.id === this.form.stockTransactionStatusId);
          this.savedRows.update(rows => [...rows, {
            internalCode: this.form.internalCode,
            txnType:      typeName ? (this.isRtl ? typeName.name_AR : typeName.name_EN) : '—',
            status:       statusName ? (this.isRtl ? statusName.name_AR : statusName.name_EN) : '—',
            postingDate:  this.form.postingDate,
          }]);
          this.form = this.blank();
          this.isEdit.set(false);
          this.savingNew.set(false);
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

  save() { this.submit(false); }
  saveAndNew() { this.submit(true); }

  resetForm() {
    this.form = this.blank();
    this.loadNextCode();
  }

  back() { this.router.navigate(['/stock/stock-reconciliation']); }
}
