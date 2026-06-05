import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'items', pathMatch: 'full' },
  { path: 'items',          loadComponent: () => import('./features/items/items.component').then(m => m.ItemsComponent) },
  { path: 'item-groups',    loadComponent: () => import('./features/item-groups/item-groups.component').then(m => m.ItemGroupsComponent) },
  { path: 'uom-conversion-groups', loadComponent: () => import('./features/uom-conversion-groups/uom-conversion-groups.component').then(m => m.UomConversionGroupsComponent) },
  { path: 'uoms',           loadComponent: () => import('./features/uoms/uoms.component').then(m => m.UomsComponent) },
  { path: 'warehouses',     loadComponent: () => import('./features/warehouses/warehouses.component').then(m => m.WarehousesComponent) },
  { path: 'stock',          loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
  { path: 'stock-movement', loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
];
