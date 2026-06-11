import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'stock/items', pathMatch: 'full' },

  { path: 'stock/items',            loadComponent: () => import('./features/items/items.component').then(m => m.ItemsComponent) },
  { path: 'stock/items/operation',  loadComponent: () => import('./features/items/operations/items-operation.component').then(m => m.ItemsOperationComponent) },
  { path: 'stock/items/operation/:id', loadComponent: () => import('./features/items/operations/items-operation.component').then(m => m.ItemsOperationComponent) },

  { path: 'stock/item-groups',             loadComponent: () => import('./features/item-groups/item-groups.component').then(m => m.ItemGroupsComponent) },
  { path: 'stock/item-groups/operation',   loadComponent: () => import('./features/item-groups/operations/item-groups-operation.component').then(m => m.ItemGroupsOperationComponent) },
  { path: 'stock/item-groups/operation/:id', loadComponent: () => import('./features/item-groups/operations/item-groups-operation.component').then(m => m.ItemGroupsOperationComponent) },

  { path: 'stock/uoms',                loadComponent: () => import('./features/uoms/uoms.component').then(m => m.UomsComponent) },
  { path: 'stock/uoms/operation',      loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },
  { path: 'stock/uoms/operation/:id',  loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },

  { path: 'stock/uom-conversion-groups',                loadComponent: () => import('./features/uom-conversion-groups/uom-conversion-groups.component').then(m => m.UomConversionGroupsComponent) },
  { path: 'stock/uom-conversion-groups/operation',      loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },
  { path: 'stock/uom-conversion-groups/operation/:id',  loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },

  { path: 'stock/uom-conversion-factors',                loadComponent: () => import('./features/uom-conversion-factors/uom-conversion-factors.component').then(m => m.UomConversionFactorsComponent) },
  { path: 'stock/uom-conversion-factors/operation',      loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },
  { path: 'stock/uom-conversion-factors/operation/:id',  loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },

  { path: 'stock/warehouse-categories',               loadComponent: () => import('./features/warehouse-categories/warehouse-categories.component').then(m => m.WarehouseCategoriesComponent) },
  { path: 'stock/warehouse-categories/operation',     loadComponent: () => import('./features/warehouse-categories/operations/warehouse-categories-operation.component').then(m => m.WarehouseCategoriesOperationComponent) },
  { path: 'stock/warehouse-categories/operation/:id', loadComponent: () => import('./features/warehouse-categories/operations/warehouse-categories-operation.component').then(m => m.WarehouseCategoriesOperationComponent) },

  { path: 'stock/warehouses',               loadComponent: () => import('./features/warehouses/warehouses.component').then(m => m.WarehousesComponent) },
  { path: 'stock/warehouses/operation',     loadComponent: () => import('./features/warehouses/operations/warehouses-operation.component').then(m => m.WarehousesOperationComponent) },
  { path: 'stock/warehouses/operation/:id', loadComponent: () => import('./features/warehouses/operations/warehouses-operation.component').then(m => m.WarehousesOperationComponent) },
  { path: 'stock/current-stock',   loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
  { path: 'stock/stock-movement',  loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
];
