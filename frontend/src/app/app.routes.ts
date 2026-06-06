import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'items', pathMatch: 'full' },
  { path: 'items',          loadComponent: () => import('./features/items/items.component').then(m => m.ItemsComponent) },
  { path: 'item-groups',    loadComponent: () => import('./features/item-groups/item-groups.component').then(m => m.ItemGroupsComponent) },
  { path: 'uom-conversion-groups', loadComponent: () => import('./features/uom-conversion-groups/uom-conversion-groups.component').then(m => m.UomConversionGroupsComponent) },
  { path: 'uom-conversion-groups/operation',     loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },
  { path: 'uom-conversion-groups/operation/:id', loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },
  { path: 'uoms',                   loadComponent: () => import('./features/uoms/uoms.component').then(m => m.UomsComponent) },
  { path: 'uoms/operation',         loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },
  { path: 'uoms/operation/:id',     loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },
  { path: 'uom-conversion-factors',             loadComponent: () => import('./features/uom-conversion-factors/uom-conversion-factors.component').then(m => m.UomConversionFactorsComponent) },
  { path: 'uom-conversion-factors/operation',     loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },
  { path: 'uom-conversion-factors/operation/:id', loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },
  { path: 'warehouses',     loadComponent: () => import('./features/warehouses/warehouses.component').then(m => m.WarehousesComponent) },
  { path: 'stock',          loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
  { path: 'stock-movement', loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
];
