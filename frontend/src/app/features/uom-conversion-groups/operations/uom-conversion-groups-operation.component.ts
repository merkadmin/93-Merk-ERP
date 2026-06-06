import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';

interface UomConversionGroup {
  id: number;
  internalCode: string;
  name_EN: string;
  name_AR: string;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uom-conversion-groups-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent],
  templateUrl: './uom-conversion-groups-operation.component.html',
  styleUrl: './uom-conversion-groups-operation.component.less',
})
export class UomConversionGroupsOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);

  isEdit     = signal(false);
  saving     = signal(false);
  savingNew  = signal(false);

  form: Partial<UomConversionGroup> = this.blank();

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<UomConversionGroup>(`uomconversiongroups/${id}`).subscribe(g => this.form = { ...g });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('uomconversiongroups/nextcode').subscribe(r => {
      this.form.internalCode = r.code;
    });
  }

  private blank(): Partial<UomConversionGroup> {
    return { id: 0, internalCode: '', name_EN: '', name_AR: '', isActive: true, isFavorite: false };
  }

  private validate(): boolean {
    const missing: string[] = [];

    if (!this.form.internalCode?.trim())
      missing.push(this.translate.instant('uom_conversion_groups.internal_code'));

    if (!this.form.name_EN?.trim())
      missing.push(`${this.translate.instant('common.name')} (EN)`);

    if (!this.form.name_AR?.trim())
      missing.push(`${this.translate.instant('common.name')} (AR)`);

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
      ? this.api.put<UomConversionGroup>(`uomconversiongroups/${this.form.id}`, this.form)
      : this.api.post<UomConversionGroup>('uomconversiongroups', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
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

  save()       { this.submit(false); }
  saveAndNew() { this.submit(true);  }

  resetForm() {
    this.form = this.blank();
    this.loadNextCode();
  }

  back() {
    this.router.navigate(['/stock/uom-conversion-groups']);
  }
}
