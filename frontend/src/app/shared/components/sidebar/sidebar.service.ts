import { Injectable } from '@angular/core';

export interface SidebarItem {
  route?: string;
  icon: string;
  labelKey: string;
  children?: SidebarItem[];
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
        { route: '/stock/warehouse-categories', icon: 'ki-outline ki-abstract-28', labelKey: 'nav.warehouse_categories' },
        { route: '/stock/warehouses', icon: 'ki-outline ki-home-2', labelKey: 'nav.warehouses' },
        { route: '/stock/items', icon: 'ki-outline ki-package', labelKey: 'nav.items' },
        { route: '/stock/item-groups', icon: 'ki-outline ki-category', labelKey: 'nav.item_groups' },
        { route: '/stock/uoms', icon: 'ki-outline ki-abstract-26', labelKey: 'nav.uoms' },
        { route: '/stock/uom-conversion-groups', icon: 'ki-outline ki-abstract-14', labelKey: 'nav.uom_conversion_groups' },
        { route: '/stock/uom-conversion-factors', icon: 'ki-outline ki-arrows-circle', labelKey: 'nav.uom_conversion_factors' },
      ],
    },
    {
      id: 'buying',
      labelKey: 'nav.buying',
      icon: 'ki-basket',
      iconColor: 'text-warning',
      isVisible: () => true,
      getChildren: () => [
        { route: '/buying/suppliers', icon: 'ki-outline ki-people', labelKey: 'nav.suppliers' },
        { route: '/buying/companies', icon: 'ki-outline ki-office-bag', labelKey: 'nav.companies' },
      ],
    },
    {
      id: 'warehousing',
      labelKey: 'nav.warehousing',
      icon: 'ki-home-2',
      iconColor: 'text-success',
      isVisible: () => true,
      getChildren: () => [
        { route: '/stock/current-stock', icon: 'ki-outline ki-chart-simple', labelKey: 'nav.current_stock' },
        {
          icon: 'ki-outline ki-arrows-loop',
          labelKey: 'nav.stock_movement',
          children: [
            { route: '/stock/stock-reconciliation', icon: 'ki-outline ki-document',        labelKey: 'nav.stock_reconciliation' },
            { route: '/stock/purchase-receipt',     icon: 'ki-outline ki-basket-ok',       labelKey: 'nav.purchase_receipt' },
            { route: '/stock/delivery-note',        icon: 'ki-outline ki-delivery',        labelKey: 'nav.delivery_note' },
            { route: '/stock/transfer-entry',       icon: 'ki-outline ki-arrows-loop',     labelKey: 'nav.transfer_entry' },
            { route: '/stock/stock-balance',        icon: 'ki-outline ki-chart-pie-simple',labelKey: 'nav.stock_balance' },
            { route: '/stock/stock-settings',       icon: 'ki-outline ki-setting-2',       labelKey: 'nav.stock_settings' },
          ],
        },
      ],
    },
  ];
}
