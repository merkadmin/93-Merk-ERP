import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';

interface UomConversionGroup {
  id: number;
  name_EN: string;
  name_AR: string;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uom-conversion-groups-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './uom-conversion-groups-operation.component.html',
  styleUrl: './uom-conversion-groups-operation.component.less',
})
export class UomConversionGroupsOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);

  isEdit = signal(false);
  saving = signal(false);

  form: Partial<UomConversionGroup> = { id: 0, name_EN: '', name_AR: '', isActive: true, isFavorite: false };

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<UomConversionGroup>(`uomconversiongroups/${id}`).subscribe(g => this.form = { ...g });
    }
  }

  private validate(): boolean {
    const missing: string[] = [];

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

  save() {
    if (!this.validate()) return;

    this.saving.set(true);
    const req = this.isEdit()
      ? this.api.put<UomConversionGroup>(`uomconversiongroups/${this.form.id}`, this.form)
      : this.api.post<UomConversionGroup>('uomconversiongroups', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        this.back();
      },
      error: () => this.saving.set(false),
    });
  }

  back() {
    this.router.navigate(['/uom-conversion-groups']);
  }
}
