import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'stock/items', pathMatch: 'full' },

  { path: 'stock/items',       loadComponent: () => import('./features/items/items.component').then(m => m.ItemsComponent) },
  { path: 'stock/item-groups', loadComponent: () => import('./features/item-groups/item-groups.component').then(m => m.ItemGroupsComponent) },

  { path: 'stock/uoms',                loadComponent: () => import('./features/uoms/uoms.component').then(m => m.UomsComponent) },
  { path: 'stock/uoms/operation',      loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },
  { path: 'stock/uoms/operation/:id',  loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },

  { path: 'stock/uom-conversion-groups',                loadComponent: () => import('./features/uom-conversion-groups/uom-conversion-groups.component').then(m => m.UomConversionGroupsComponent) },
  { path: 'stock/uom-conversion-groups/operation',      loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },
  { path: 'stock/uom-conversion-groups/operation/:id',  loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },

  { path: 'stock/uom-conversion-factors',                loadComponent: () => import('./features/uom-conversion-factors/uom-conversion-factors.component').then(m => m.UomConversionFactorsComponent) },
  { path: 'stock/uom-conversion-factors/operation',      loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },
  { path: 'stock/uom-conversion-factors/operation/:id',  loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },

  { path: 'stock/warehouses',      loadComponent: () => import('./features/warehouses/warehouses.component').then(m => m.WarehousesComponent) },
  { path: 'stock/current-stock',   loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
  { path: 'stock/stock-movement',  loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
];
