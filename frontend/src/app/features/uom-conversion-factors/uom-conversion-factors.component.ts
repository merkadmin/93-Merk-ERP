import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface UOM { uomId: number; name: string; }

interface UomConversionFactor {
  id: number;
  uomFromId: number;
  uomToId: number;
  value: number;
  isActive: boolean;
  isFavorite: boolean;
  uomFrom?: { name: string };
  uomTo?: { name: string };
}

@Component({
  selector: 'app-uom-conversion-factors',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  templateUrl: './uom-conversion-factors.component.html',
  styleUrl: './uom-conversion-factors.component.less',
})
export class UomConversionFactorsComponent implements OnInit {
  api     = inject(ApiService);
  private translate = inject(TranslateService);

  factors = signal<UomConversionFactor[]>([]);
  uoms    = signal<UOM[]>([]);
  editing = signal(false);

  form: Partial<UomConversionFactor> = this.blank();

  private blank(): Partial<UomConversionFactor> {
    return { id: 0, uomFromId: 0, uomToId: 0, value: 1, isActive: true, isFavorite: false };
  }

  ngOnInit() {
    this.load();
    this.api.get<UOM[]>('uoms').subscribe(u => this.uoms.set(u));
  }

  load() {
    this.api.get<UomConversionFactor[]>('uomconversionfactors').subscribe(f => this.factors.set(f));
  }

  startEdit(f: UomConversionFactor) {
    this.form = { ...f };
    this.editing.set(true);
  }

  cancelEdit() {
    this.form = this.blank();
    this.editing.set(false);
  }

  save() {
    const isNew = !this.form.id;
    const req = isNew
      ? this.api.post<UomConversionFactor>('uomconversionfactors', this.form)
      : this.api.put<UomConversionFactor>(`uomconversionfactors/${this.form.id}`, this.form);

    req.subscribe(() => {
      this.form = this.blank();
      this.editing.set(false);
      this.load();
    });
  }

  delete(id: number) {
    this.translate.get('uom_conversion_factors.delete_confirm').subscribe(msg => {
      if (!confirm(msg)) return;
      this.api.delete(`uomconversionfactors/${id}`).subscribe(() => this.load());
    });
  }

  uomName(id: number) {
    return this.uoms().find(u => u.uomId === id)?.name ?? id;
  }
}
