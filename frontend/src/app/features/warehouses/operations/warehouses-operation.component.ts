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

interface Warehouse { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; description: string | null; parentWarehouseId: number | null; wareHouseTypeId: number | null; wareHouseCategoryId: number | null; isParent: boolean; isActive: boolean; }
interface WareHouseType { id: number; name_EN: string; name_AR: string | null; }
interface WareHouseCategory { id: number; name_EN: string; name_AR: string | null; wareHouseTypeId: number | null; }
interface SavedRow { name_EN: string; name_AR: string; parentName: string; isParent: boolean; }

@Component({
  selector: 'app-warehouses-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './warehouses-operation.component.html',
  styleUrl: './warehouses-operation.component.less',
})
export class WarehousesOperationComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit = signal(false);
  saving = signal(false);
  savingNew = signal(false);
  savedRows = signal<SavedRow[]>([]);

  warehouses = signal<Warehouse[]>([]);
  wareHouseTypes = signal<WareHouseType[]>([]);
  wareHouseCategories = signal<WareHouseCategory[]>([]);
  form: Partial<Warehouse> = this.blank();

  ngOnInit() {
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
    this.api.get<WareHouseType[]>('warehousetypes').subscribe(d => this.wareHouseTypes.set(d));
    this.api.get<WareHouseCategory[]>('warehousecategories').subscribe(d => this.wareHouseCategories.set(d));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<Warehouse>(`warehouses/${id}`).subscribe(w => this.form = { ...w });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('warehouses/nextcode').subscribe(r => this.form.internalCode = r.code);
  }

  private label<T extends { name_EN: string; name_AR: string | null }>(item: T): string {
    return this.isRtl ? (item.name_AR || item.name_EN) : (item.name_EN || item.name_AR || '');
  }

  get parentOptions(): SelectOption[] {
    return this.warehouses()
      .filter(w => w.id !== this.form.id)
      .map(w => ({ value: w.id, label: this.label(w) }));
  }

  get typeOptions(): SelectOption[] {
    return this.wareHouseTypes().map(t => ({ value: t.id, label: this.label(t) }));
  }

  get categoryOptions(): SelectOption[] {
    return this.wareHouseCategories().map(c => ({ value: c.id, label: this.label(c) }));
  }

  private blank(): Partial<Warehouse> {
    return { id: 0, internalCode: null, name_EN: '', name_AR: null, description: null, parentWarehouseId: null, wareHouseTypeId: null, wareHouseCategoryId: null, isParent: false, isActive: true };
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
      ? this.api.put<Warehouse>(`warehouses/${this.form.id}`, this.form)
      : this.api.post<Warehouse>('warehouses', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          const parent = this.warehouses().find(w => w.id === this.form.parentWarehouseId);
          this.savedRows.update(rows => [...rows, {
            name_EN: this.form.name_EN ?? '',
            name_AR: this.form.name_AR ?? '',
            parentName: parent ? this.label(parent) : '—',
            isParent: this.form.isParent ?? false,
          }]);
          this.form = this.blank();
          this.isEdit.set(false);
          this.savingNew.set(false);
          this.loadNextCode();
          this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
        } else {
          this.back();
        }
      },
      error: () => { this.saving.set(false); this.savingNew.set(false); },
    });
  }

  onCategoryChange(value: string | number | null): void {
    const id = value ? +value : null;
    this.form.wareHouseCategoryId = id;
    if (id != null) {
      const cat = this.wareHouseCategories().find(c => c.id === id);
      if (cat?.wareHouseTypeId != null) {
        this.form.wareHouseTypeId = cat.wareHouseTypeId;
      }
    }
  }

  save() { this.submit(false); }
  saveAndNew() { this.submit(true); }
  resetForm() { this.form = this.blank(); this.loadNextCode(); }
  back() { this.router.navigate(['/stock/warehouses']); }
}
