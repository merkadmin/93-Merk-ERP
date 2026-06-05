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
        { route: '/items',                   icon: 'ki-outline ki-package',     labelKey: 'nav.items' },
        { route: '/item-groups',             icon: 'ki-outline ki-category',    labelKey: 'nav.item_groups' },
        { route: '/uoms',                    icon: 'ki-outline ki-ruler',        labelKey: 'nav.uoms' },
        { route: '/uom-conversion-groups',   icon: 'ki-outline ki-abstract-14', labelKey: 'nav.uom_conversion_groups' },
        { route: '/uom-conversion-factors',  icon: 'ki-outline ki-arrows-circle',labelKey: 'nav.uom_conversion_factors' },
      ],
    },
    {
      id: 'warehousing',
      labelKey: 'nav.warehousing',
      icon: 'ki-home-2',
      iconColor: 'text-success',
      isVisible: () => true,
      getChildren: () => [
        { route: '/warehouses',     icon: 'ki-outline ki-home-2',       labelKey: 'nav.warehouses' },
        { route: '/stock',          icon: 'ki-outline ki-chart-simple', labelKey: 'nav.current_stock' },
        { route: '/stock-movement', icon: 'ki-outline ki-arrows-loop',  labelKey: 'nav.stock_movement' },
      ],
    },
  ];
}
