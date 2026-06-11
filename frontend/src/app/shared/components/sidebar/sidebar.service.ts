import { Injectable } from '@angular/core';

export interface SidebarItem {
  route: string;
  icon: string;
  labelKey: string;
}

export interface SidebarSection {
  id: string;
  labelKey: string;
  icon: string;
  iconColor: string;
  isVisible: () => boolean;
  getChildren: () => SidebarItem[];
}

@Injectable({ providedIn: 'root' })
export class SidebarService {
  readonly sections: SidebarSection[] = [
    {
      id: 'inventory',
      labelKey: 'nav.inventory',
      icon: 'ki-package',
      iconColor: 'text-primary',
      isVisible: () => true,
      getChildren: () => [
        { route: '/stock/warehouse-categories',    icon: 'ki-outline ki-abstract-28',  labelKey: 'nav.warehouse_categories' },
        { route: '/stock/warehouses',              icon: 'ki-outline ki-home-2',       labelKey: 'nav.warehouses' },
        { route: '/stock/items',                   icon: 'ki-outline ki-package',      labelKey: 'nav.items' },
        { route: '/stock/item-groups',             icon: 'ki-outline ki-category',     labelKey: 'nav.item_groups' },
        { route: '/stock/uoms',                    icon: 'ki-outline ki-abstract-26',  labelKey: 'nav.uoms' },
        { route: '/stock/uom-conversion-groups',   icon: 'ki-outline ki-abstract-14',  labelKey: 'nav.uom_conversion_groups' },
        { route: '/stock/uom-conversion-factors',  icon: 'ki-outline ki-arrows-circle',labelKey: 'nav.uom_conversion_factors' },
      ],
    },
    {
      id: 'warehousing',
      labelKey: 'nav.warehousing',
      icon: 'ki-home-2',
      iconColor: 'text-success',
      isVisible: () => true,
      getChildren: () => [
        { route: '/stock/current-stock',  icon: 'ki-outline ki-chart-simple', labelKey: 'nav.current_stock' },
        { route: '/stock/stock-movement', icon: 'ki-outline ki-arrows-loop',  labelKey: 'nav.stock_movement' },
      ],
    },
  ];
}
