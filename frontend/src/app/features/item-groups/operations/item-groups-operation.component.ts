import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';
import { CustomSelectInputComponent, SelectOption } from '../../../shared/components/custom-controls/custom-select-input/custom-select-input.component';

interface ItemGroup {
  itemGroupId: number;
  name: string;
  parentItemGroupId: number | null;
  isGroup: boolean;
  isActive: boolean;
  isFavorite: boolean;
}

interface SavedRow {
  name: string;
  parentName: string;
  isGroup: boolean;
}

@Component({
  selector: 'app-item-groups-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './item-groups-operation.component.html',
  styleUrl: './item-groups-operation.component.less',
})
export class ItemGroupsOperationComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);

  isEdit    = signal(false);
  saving    = signal(false);
  savingNew = signal(false);
  savedRows = signal<SavedRow[]>([]);
  allGroups = signal<ItemGroup[]>([]);

  form: Partial<ItemGroup> = this.blank();

  ngOnInit() {
    this.api.get<ItemGroup[]>('itemgroups').subscribe(g => this.allGroups.set(g));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<ItemGroup>(`itemgroups/${id}`).subscribe(g => this.form = { ...g });
    }
  }

  get parentGroupOptions(): SelectOption[] {
    const filtered = this.isEdit()
      ? this.allGroups().filter(g => g.itemGroupId !== this.form.itemGroupId)
      : this.allGroups();
    return filtered.map(g => ({ value: g.itemGroupId, label: g.name }));
  }

  private blank(): Partial<ItemGroup> {
    return { itemGroupId: 0, name: '', parentItemGroupId: null, isGroup: false, isActive: true, isFavorite: false };
  }

  private validate(): boolean {
    const missing: string[] = [];

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
      ? this.api.put<ItemGroup>(`itemgroups/${this.form.itemGroupId}`, this.form)
      : this.api.post<ItemGroup>('itemgroups', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          const parent = this.allGroups().find(g => g.itemGroupId === this.form.parentItemGroupId);
          this.savedRows.update(rows => [...rows, {
            name:       this.form.name ?? '',
            parentName: parent?.name ?? '—',
            isGroup:    this.form.isGroup ?? false,
          }]);
          this.form = this.blank();
          this.isEdit.set(false);
          this.savingNew.set(false);
          this.api.get<ItemGroup[]>('itemgroups').subscribe(g => this.allGroups.set(g));
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

  resetForm() { this.form = this.blank(); }

  back() { this.router.navigate(['/stock/item-groups']); }
}
