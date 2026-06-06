import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';

interface UOM {
  uomId: number;
  internalCode: string;
  name: string;
  mustBeWholeNumber: boolean;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uoms-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent],
  templateUrl: './uoms-operation.component.html',
  styleUrl: './uoms-operation.component.less',
})
export class UomsOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);

  isEdit    = signal(false);
  saving    = signal(false);
  savingNew = signal(false);

  form: Partial<UOM> = this.blank();

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<UOM>(`uoms/${id}`).subscribe(u => this.form = { ...u });
    } else {
      this.loadNextCode();
    }
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('uoms/nextcode').subscribe(r => {
      this.form.internalCode = r.code;
    });
  }

  private blank(): Partial<UOM> {
    return { uomId: 0, internalCode: '', name: '', mustBeWholeNumber: false, isActive: true, isFavorite: false };
  }

  private validate(): boolean {
    const missing: string[] = [];

    if (!this.form.internalCode?.trim())
      missing.push(this.translate.instant('uoms.internal_code'));

    if (!this.form.name?.trim())
      missing.push(this.translate.instant('common.name'));

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
      ? this.api.put<UOM>(`uoms/${this.form.uomId}`, this.form)
      : this.api.post<UOM>('uoms', this.form);

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
    this.router.navigate(['/uoms']);
  }
}
