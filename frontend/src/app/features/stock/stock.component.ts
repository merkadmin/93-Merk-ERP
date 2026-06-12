import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, NgIf, DecimalPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface StockBin {
  binId: number;
  item: { internalCode: string; name_EN: string; name_AR?: string };
  warehouse: { name: string };
  actualQty: number;
  reservedQty: number;
  orderedQty: number;
  valuationRate: number;
}

@Component({
  selector: 'app-stock',
  standalone: true,
  imports: [NgFor, NgIf, DecimalPipe, TranslatePipe],
  templateUrl: './stock.component.html',
  styleUrl: './stock.component.less',
})
export class StockComponent implements OnInit {
  api = inject(ApiService);
  bins = signal<StockBin[]>([]);

  ngOnInit() { this.load(); }
  load() { this.api.get<StockBin[]>('stock').subscribe(d => this.bins.set(d)); }
}


