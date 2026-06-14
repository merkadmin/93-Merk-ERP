import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'stock/items', pathMatch: 'full' },
  { path: 'login',    loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./features/register/register.component').then(m => m.RegisterComponent) },

  {
    path: 'stock',
    canActivate: [authGuard],
    children: [
      { path: 'items',                     loadComponent: () => import('./features/items/items.component').then(m => m.ItemsComponent) },
      { path: 'items/operation',           loadComponent: () => import('./features/items/operations/items-operation.component').then(m => m.ItemsOperationComponent) },
      { path: 'items/operation/:id',       loadComponent: () => import('./features/items/operations/items-operation.component').then(m => m.ItemsOperationComponent) },

      { path: 'item-groups',               loadComponent: () => import('./features/item-groups/item-groups.component').then(m => m.ItemGroupsComponent) },
      { path: 'item-groups/operation',     loadComponent: () => import('./features/item-groups/operations/item-groups-operation.component').then(m => m.ItemGroupsOperationComponent) },
      { path: 'item-groups/operation/:id', loadComponent: () => import('./features/item-groups/operations/item-groups-operation.component').then(m => m.ItemGroupsOperationComponent) },

      { path: 'uoms',                      loadComponent: () => import('./features/uoms/uoms.component').then(m => m.UomsComponent) },
      { path: 'uoms/operation',            loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },
      { path: 'uoms/operation/:id',        loadComponent: () => import('./features/uoms/operations/uoms-operation.component').then(m => m.UomsOperationComponent) },

      { path: 'uom-conversion-groups',               loadComponent: () => import('./features/uom-conversion-groups/uom-conversion-groups.component').then(m => m.UomConversionGroupsComponent) },
      { path: 'uom-conversion-groups/operation',     loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },
      { path: 'uom-conversion-groups/operation/:id', loadComponent: () => import('./features/uom-conversion-groups/operations/uom-conversion-groups-operation.component').then(m => m.UomConversionGroupsOperationComponent) },

      { path: 'uom-conversion-factors',               loadComponent: () => import('./features/uom-conversion-factors/uom-conversion-factors.component').then(m => m.UomConversionFactorsComponent) },
      { path: 'uom-conversion-factors/operation',     loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },
      { path: 'uom-conversion-factors/operation/:id', loadComponent: () => import('./features/uom-conversion-factors/operations/uom-conversion-factors-operation.component').then(m => m.UomConversionFactorsOperationComponent) },

      { path: 'warehouse-categories',               loadComponent: () => import('./features/warehouse-categories/warehouse-categories.component').then(m => m.WarehouseCategoriesComponent) },
      { path: 'warehouse-categories/operation',     loadComponent: () => import('./features/warehouse-categories/operations/warehouse-categories-operation.component').then(m => m.WarehouseCategoriesOperationComponent) },
      { path: 'warehouse-categories/operation/:id', loadComponent: () => import('./features/warehouse-categories/operations/warehouse-categories-operation.component').then(m => m.WarehouseCategoriesOperationComponent) },

      { path: 'warehouses',               loadComponent: () => import('./features/warehouses/warehouses.component').then(m => m.WarehousesComponent) },
      { path: 'warehouses/operation',     loadComponent: () => import('./features/warehouses/operations/warehouses-operation.component').then(m => m.WarehousesOperationComponent) },
      { path: 'warehouses/operation/:id', loadComponent: () => import('./features/warehouses/operations/warehouses-operation.component').then(m => m.WarehousesOperationComponent) },

      { path: 'current-stock',       loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
      { path: 'stock-movement',      loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },

      { path: 'stock-reconciliation',           loadComponent: () => import('./features/stock-reconciliation/stock-reconciliation.component').then(m => m.StockReconciliationComponent) },
      { path: 'stock-reconciliation/operation',     loadComponent: () => import('./features/stock-reconciliation/operations/stock-reconciliation-operation.component').then(m => m.StockReconciliationOperationComponent) },
      { path: 'stock-reconciliation/operation/:id', loadComponent: () => import('./features/stock-reconciliation/operations/stock-reconciliation-operation.component').then(m => m.StockReconciliationOperationComponent) },
      { path: 'purchase-receipt',     loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
      { path: 'delivery-note',        loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
      { path: 'transfer-entry',       loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
      { path: 'stock-balance',        loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
      { path: 'stock-settings',       loadComponent: () => import('./features/stock-movement/stock-movement.component').then(m => m.StockMovementComponent) },
    ],
  },
];
