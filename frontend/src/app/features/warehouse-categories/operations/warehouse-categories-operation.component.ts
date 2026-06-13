import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';

interface WareHouseType { id: number; name_EN: string; name_AR: string | null; }
interface WareHouseCategory { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; description: string | null; isActive: boolean; wareHouseTypeId: number | null; }
interface SavedRow { internalCode: string; name_EN: string; name_AR: string; wareHouseTypeName: string; }

@Component({
  selector: 'app-warehouse-categories-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent],
  templateUrl: './warehouse-categories-operation.component.html',
  styleUrl: './warehouse-categories-operation.component.less',
})
export class WarehouseCategoriesOperationComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit        = signal(false);
  saving        = signal(false);
  savingNew     = signal(false);
  savedRows     = signal<SavedRow[]>([]);
  wareHouseTypes = signal<WareHouseType[]>([]);

  form: Partial<WareHouseCategory> = this.blank();

  ngOnInit() {
    this.api.get<WareHouseType[]>('warehousetypes').subscribe(t => this.wareHouseTypes.set(t));
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<WareHouseCategory>(`warehousecategories/${id}`).subscribe(c => this.form = { ...c });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('warehousecategories/nextcode').subscribe(r => this.form.internalCode = r.code);
  }

  private blank(): Partial<WareHouseCategory> {
    return { id: 0, internalCode: null, name_EN: '', name_AR: null, description: null, isActive: true, wareHouseTypeId: null };
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
      ? this.api.put<WareHouseCategory>(`warehousecategories/${this.form.id}`, this.form)
      : this.api.post<WareHouseCategory>('warehousecategories', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          this.savedRows.update(rows => [...rows, { internalCode: this.form.internalCode ?? '', name_EN: this.form.name_EN ?? '', name_AR: this.form.name_AR ?? '', wareHouseTypeName: this.typeLabel(this.form.wareHouseTypeId) }]);
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
    const t = this.wareHouseTypes().find(x => x.id === id);
    if (!t) return '—';
    return this.isRtl ? (t.name_AR || t.name_EN) : t.name_EN;
  }

  resetForm() { this.form = this.blank(); this.loadNextCode(); }
  back() { this.router.navigate(['/stock/warehouse-categories']); }
}
