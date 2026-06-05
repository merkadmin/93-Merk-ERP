import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
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
  imports: [TranslatePipe],
  templateUrl: './uom-conversion-groups.component.html',
  styleUrl: './uom-conversion-groups.component.less',
})
export class UomConversionGroupsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);

  groups = signal<UomConversionGroup[]>([]);

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UomConversionGroup[]>('uomconversiongroups').subscribe(d => this.groups.set(d));
  }

  addNew() {
    this.router.navigate(['/uom-conversion-groups/operation']);
  }

  edit(id: number) {
    this.router.navigate(['/uom-conversion-groups/operation', id]);
  }

  delete(id: number) {
    if (!confirm(this.translate.instant('uom_conversion_groups.delete_confirm'))) return;
    this.api.delete(`uomconversiongroups/${id}`).subscribe(() => this.load());
  }
}
