import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface UOM { uomId: number; name: string; mustBeWholeNumber: boolean; }

@Component({
  selector: 'app-uoms',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  templateUrl: './uoms.component.html',
  styleUrl: './uoms.component.less',
})
export class UomsComponent implements OnInit {
  api = inject(ApiService);
  translate = inject(TranslateService);
  uoms = signal<UOM[]>([]);
  showForm = false;
  editing = false;
  form: Partial<UOM> = {};

  ngOnInit() { this.load(); }
  load() { this.api.get<UOM[]>('uoms').subscribe(d => this.uoms.set(d)); }
  openNew() { this.form = { name: '', mustBeWholeNumber: false }; this.editing = false; this.showForm = true; }
  edit(u: UOM) { this.form = { ...u }; this.editing = true; this.showForm = true; }
  cancel() { this.showForm = false; }
  save() {
    const obs = this.editing
      ? this.api.put(`uoms/${this.form.uomId}`, this.form)
      : this.api.post('uoms', this.form);
    obs.subscribe(() => { this.load(); this.cancel(); });
  }
  delete(id: number) {
    if (confirm(this.translate.instant('uoms.delete_confirm')))
      this.api.delete(`uoms/${id}`).subscribe(() => this.load());
  }
}


