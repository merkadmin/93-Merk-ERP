import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface UomConversionGroup {
  id: number;
  name_EN: string;
  name_AR: string;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uom-conversion-groups',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  templateUrl: './uom-conversion-groups.component.html',
})
export class UomConversionGroupsComponent implements OnInit {
  api      = inject(ApiService);
  translate = inject(TranslateService);
  groups   = signal<UomConversionGroup[]>([]);
  editing  = signal(false);
  form: Partial<UomConversionGroup> = this.blank();

  blank(): Partial<UomConversionGroup> {
    return { id: 0, name_EN: '', name_AR: '' };
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UomConversionGroup[]>('uomconversiongroups').subscribe(d => this.groups.set(d));
  }

  save() {
    if (this.editing()) {
      this.api.put<UomConversionGroup>(`uomconversiongroups/${this.form.id}`, this.form)
        .subscribe(() => { this.cancelEdit(); this.load(); });
    } else {
      this.api.post<UomConversionGroup>('uomconversiongroups', this.form)
        .subscribe(() => { this.form = this.blank(); this.load(); });
    }
  }

  startEdit(g: UomConversionGroup) {
    this.form = { ...g };
    this.editing.set(true);
  }

  cancelEdit() {
    this.form = this.blank();
    this.editing.set(false);
  }

  delete(id: number) {
    if (!confirm(this.translate.instant('uom_conversion_groups.delete_confirm'))) return;
    this.api.delete(`uomconversiongroups/${id}`).subscribe(() => this.load());
  }
}

