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

interface UOM   { id: number; name_EN: string; name_AR: string; }
interface Group { id: number; name_EN: string; name_AR: string; }

interface UomConversionFactor {
  id: number;
  uomFromId: number;
  uomToId: number;
  value: number;
  uomConversionGroupId: number | null;
  internalCode: string | null;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uom-conversion-factors-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './uom-conversion-factors-operation.component.html',
  styleUrl: './uom-conversion-factors-operation.component.less',
})
export class UomConversionFactorsOperationComponent implements OnInit {
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
  uoms      = signal<UOM[]>([]);
  groups    = signal<Group[]>([]);

  form: Partial<UomConversionFactor> = this.blank();
  valueExpression = '1';

  ngOnInit() {
    this.api.get<UOM[]>('uoms').subscribe(u => this.uoms.set(u));
    this.api.get<Group[]>('uomconversiongroups').subscribe(g => this.groups.set(g));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<UomConversionFactor>(`uomconversionfactors/${id}`).subscribe(f => {
        this.form = { ...f };
        this.valueExpression = f.value?.toString() ?? '1';
      });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('uomconversionfactors/nextcode').subscribe(r => {
      this.form.internalCode = r.code;
    });
  }

  evaluateExpression() {
    const sanitized = this.valueExpression.trim().replace(/\s+/g, '');
    if (!sanitized) return;
    // Only allow digits, decimal points, and arithmetic operators — no letters possible
    if (!/^[0-9+\-*/().]+$/.test(sanitized)) return;
    try {
      // Safe: regex above guarantees only numbers and math operators
      // eslint-disable-next-line no-new-func
      const result = Function(`'use strict'; return (${sanitized})`)() as number;
      if (typeof result === 'number' && isFinite(result) && result > 0) {
        this.form.value = result;
        this.valueExpression = parseFloat(result.toFixed(10)).toString();
      }
    } catch { /* invalid expression — leave as-is */ }
  }

  uomLabel(uom: UOM): string {
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR);
  }

  groupLabel(g: Group): string {
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR);
  }

  get uomOptions(): SelectOption[] {
    return this.uoms().map(u => ({ value: u.id, label: this.uomLabel(u) }));
  }

  get groupOptions(): SelectOption[] {
    return this.groups().map(g => ({ value: g.id, label: this.groupLabel(g) }));
  }

  private blank(): Partial<UomConversionFactor> {
    return { id: 0, uomFromId: 0, uomToId: 0, value: 1, uomConversionGroupId: null, internalCode: null, isActive: true, isFavorite: false };
  }

  private resetAll() {
    this.form.id                   = 0;
    this.form.uomFromId            = 0;
    this.form.uomToId              = 0;
    this.form.value                = 1;
    this.form.uomConversionGroupId = null;
    this.form.internalCode         = null;
    this.form.isActive             = true;
    this.form.isFavorite           = false;
    this.valueExpression           = '1';
  }

  private validate(): boolean {
    const missing: string[] = [];

    if (!this.form.uomFromId)
      missing.push(this.translate.instant('uom_conversion_factors.from_uom'));

    if (!this.form.uomToId)
      missing.push(this.translate.instant('uom_conversion_factors.to_uom'));

    if (!this.form.value || this.form.value <= 0)
      missing.push(this.translate.instant('uom_conversion_factors.value'));

    if (!this.form.uomConversionGroupId)
      missing.push(this.translate.instant('uom_conversion_factors.conversion_group'));

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
      ? this.api.put<UomConversionFactor>(`uomconversionfactors/${this.form.id}`, this.form)
      : this.api.post<UomConversionFactor>('uomconversionfactors', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          this.resetAll();
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

  save()       { this.submit(false); }
  saveAndNew() { this.submit(true);  }

  resetForm() { this.resetAll(); this.loadNextCode(); }

  back() { this.router.navigate(['/stock/uom-conversion-factors']); }
}
