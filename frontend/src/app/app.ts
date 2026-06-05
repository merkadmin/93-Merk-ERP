import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark px-3">
      <a class="navbar-brand fw-bold" routerLink="/"><i class="bi bi-boxes me-2"></i>Merk ERP</a>
      <div class="navbar-nav flex-row gap-1 ms-3">
        <a class="nav-link px-2" routerLink="/items"          routerLinkActive="text-warning">Items</a>
        <a class="nav-link px-2" routerLink="/item-groups"    routerLinkActive="text-warning">Item Groups</a>
        <a class="nav-link px-2" routerLink="/uoms"           routerLinkActive="text-warning">UOMs</a>
        <a class="nav-link px-2" routerLink="/warehouses"     routerLinkActive="text-warning">Warehouses</a>
        <a class="nav-link px-2" routerLink="/stock"          routerLinkActive="text-warning">Current Stock</a>
        <a class="nav-link px-2" routerLink="/stock-movement" routerLinkActive="text-warning">Stock Movement</a>
      </div>
    </nav>
    <div class="container-fluid p-4">
      <router-outlet />
    </div>
  `,
})
export class App {}
