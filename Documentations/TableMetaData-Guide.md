# TableMetaData System — Developer Guide

This document explains every step required to:
- Register a **new column** for an existing table in TableMetaData
- Register a **brand-new table** from scratch

Work through each section top-to-bottom. Do not skip steps.

---

## 1. How the System Works (Overview)

```
Database
  TableName_s          ← one row per registered entity (e.g. "warehouses")
  TableMetaData        ← one row per column you want to display/filter
  sp_GetTableMetaData  ← stored procedure that returns metadata by TableNameId

Backend
  MetadataController   ← GET /api/metadata/{entityKey}
                          calls the SP and returns EntityMeta JSON

Frontend
  MetadataService      ← fetches /api/metadata/{entity} and caches 30 min
  custom-table-with-pagination  ← renders columns from metadata
  regular-list-search-actions   ← renders filter bar from metadata
  cellRenderers (in list TS)    ← custom display logic for FK columns
```

The `EntityKey` in `TableName_s` is the string your Angular component passes to the
API (e.g. "warehouses", "items"). Everything flows from that key.

---

## 2. TableMetaData Column Reference

When inserting a row into `TableMetaData`, use these fields:

| Field              | Type          | Purpose                                                    |
|--------------------|---------------|------------------------------------------------------------|
| Id                 | bigint PK     | Unique integer. Use MAX(Id)+1 to find the next safe value. |
| TableNameId        | int FK        | References TableName_s.Id                                  |
| Key                | nvarchar(100) | camelCase identifier. MUST match the cellRenderer key in Angular and the JSON property name returned by the API. |
| LabelEN            | nvarchar(200) | Column header in English                                   |
| LabelAR            | nvarchar(200) | Column header in Arabic                                    |
| ColumnOrder        | int           | Display order (1 = leftmost). Does not need to be unique.  |
| EntityProperty     | nvarchar(100) | PascalCase name of the C# property (used by the SP)        |
| ForeignKeyProperty | nvarchar(100) | PascalCase name of the FK id property, or NULL if not a FK |
| FilterType         | nvarchar(20)  | "text", "number", "boolean", "select", "none"              |
| DataType           | nvarchar(20)  | "string", "number", "boolean"                              |
| RenderAs           | nvarchar(20)  | "text", "badge", "yesno", "tree"                           |
| IsSortable         | bit           | 1 = column header is clickable to sort                     |
| IsFilterable       | bit           | 1 = column appears in the filter bar                       |
| IsVisible          | bit           | 1 = column appears in the table (0 = hidden/filter-only)   |
| MinWidth           | int NULL      | Minimum pixel width for the column, or NULL                |

### FilterType values explained

- **text**    — free-text input box in the filter bar
- **number**  — numeric input box
- **boolean** — renders as a Yes / No dropdown
- **select**  — renders as a searchable dropdown; you must supply options from the
                Angular component (see Section 4.3)
- **none**    — column is not filterable regardless of IsFilterable

### RenderAs values explained

- **text**   — plain text cell
- **badge**  — green/red Active/Inactive badge (used with boolean Active fields)
- **yesno**  — neutral Yes/No badge (used for boolean flags like IsParent, IsMain)
- **tree**   — indented tree cell with icon; the Angular component must supply a
               `#treeCell` ng-template

---

## 3. Adding a New Column to an Existing Table

### Step 3.1 — Find the TableNameId

```sql
SELECT Id, Name, EntityKey FROM TableName_s ORDER BY Id;
```

Note the `Id` of the table you are adding the column to.

### Step 3.2 — Find the next safe MetaData Id

```sql
SELECT MAX(Id) FROM TableMetaData;
-- Use MAX(Id) + 1 as your new Id
```

### Step 3.3 — Insert the metadata row

IMPORTANT: Put the Arabic text in a T-SQL variable BEFORE the INSERT.
Putting Arabic directly inside a VALUES(...) list inside a -Q string
causes the bidi characters to corrupt the surrounding values.

```sql
DECLARE @labelAR NVARCHAR(200) = N'النص العربي هنا';

INSERT INTO TableMetaData
  (Id, TableNameId, [Key], LabelEN, LabelAR, ColumnOrder,
   EntityProperty, ForeignKeyProperty, FilterType, DataType,
   RenderAs, IsSortable, IsFilterable, IsVisible, MinWidth)
VALUES
  (
    <nextId>,          -- e.g. 47
    <tableNameId>,     -- e.g. 14 for warehouses
    'myColumnKey',     -- camelCase, matches Angular cellRenderer key
    'My Column',       -- English header
    @labelAR,          -- Arabic header (from variable above)
    <columnOrder>,     -- e.g. 5
    'MyEntityProperty',-- PascalCase C# property name
    'MyFkPropertyId',  -- PascalCase FK id property, or NULL
    'text',            -- FilterType
    'string',          -- DataType
    'text',            -- RenderAs
    1,                 -- IsSortable
    1,                 -- IsFilterable
    1,                 -- IsVisible
    NULL               -- MinWidth
  );
```

Run this against the correct database instance:
```
sqlcmd -S "RAYADDELLWS\RAYADDELLSQLSRV" -d MerkERPDB -E -Q "..."
```

### Step 3.4 — Record in EF migration history

So that `dotnet ef` does not try to re-run this change:

```sql
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('YYYYMMDDHHMMSS_DescriptiveName', '9.0.0');
```

Use the current UTC datetime in `YYYYMMDDHHmmss` format for the MigrationId.

### Step 3.5 — Add a cellRenderer in the Angular list component (FK columns only)

If the column displays a resolved name (not a raw value), open the list component
TypeScript file, e.g.:

```
frontend/src/app/features/warehouses/warehouses.component.ts
```

Add your key to `cellRenderers`:

```typescript
readonly cellRenderers: Record<string, (item: any) => string> = {
  // existing renderers ...
  myColumnKey: (item) => this.myLabel(item.myFkPropertyId),
};
```

Add the lookup method:

```typescript
myLabel(id: number | null): string {
  if (!id) return '—';
  const found = this.myItems().find(x => x.id === id);
  return found
    ? (this.isRtl ? (found.name_AR || found.name_EN) : (found.name_EN || found.name_AR || ''))
    : '—';
}
```

Make sure the data needed for the lookup is loaded in `ngOnInit` / `load()`:

```typescript
this.api.get<MyItem[]>('myitems').subscribe(d => this.myItems.set(d));
```

### Step 3.6 — Add select options for filterable FK columns

If `FilterType = 'select'`, open `searchFields` in the list component and add
the options:

```typescript
get searchFields(): SearchField[] {
  const myOpts = this.myItems()
    .map(x => ({
      value: x.id,
      label: this.isRtl ? (x.name_AR || x.name_EN) : (x.name_EN || x.name_AR || ''),
    }))
    .sort((a, b) => a.label.localeCompare(b.label));

  return this.meta.toSearchFields(this.columnMeta(), this.isRtl, {
    // existing options ...
    myColumnKey: myOpts,
  });
}
```

The key in the object literal MUST match the `Key` field stored in `TableMetaData`.

### Step 3.7 — Add filtering logic (if IsFilterable = true)

In the `flatFiltered` getter (or equivalent filter method) in the list component,
add the filter condition:

```typescript
if (f['myColumnKey'] != null && item.myFkPropertyId !== f['myColumnKey']) return false;
```

For text fields:
```typescript
if (f['myColumnKey'] != null && !(item.myProperty ?? '').toLowerCase()
    .includes((f['myColumnKey'] as string).toLowerCase())) return false;
```

For boolean fields:
```typescript
if (f['myColumnKey'] != null && item.myBoolProp !== (f['myColumnKey'] === 1)) return false;
```

### Step 3.8 — Invalidate the metadata cache

The `MetadataService` caches metadata for 30 minutes. After the DB change:
- Hard refresh the browser (Ctrl+Shift+R) to force a fresh fetch, OR
- Call `meta.invalidate('entityKey')` from the Angular component if you want
  programmatic invalidation.

---

## 4. Registering a Brand-New Table (Full Walkthrough)

### BACKEND STEPS

#### Step 4.1 — Create the C# model

In `backend/MerkERP.Core/Models/`, create `MyEntity_cs.cs`:

```csharp
namespace MerkERP.Core.Models;

public class MyEntity_cs
{
    public long Id { get; set; }
    public string? InternalCode { get; set; }
    public string Name_EN { get; set; } = string.Empty;
    public string? Name_AR { get; set; }
    public bool IsActive { get; set; } = true;
    public long? InsertedBy { get; set; }
    public DateTime? InsertedDate { get; set; }
}
```

#### Step 4.2 — Add DbSet to MerkDbContext

In `backend/MerkERP.DAL/Context/MerkDbContext.cs`:

```csharp
public DbSet<MyEntity_cs> MyEntity_cs => Set<MyEntity_cs>();
```

Configure it in `OnModelCreating` if needed (indexes, seed data, etc.).

#### Step 4.3 — Create the EF migration

Since the API is likely running and `dotnet ef` with `--no-build` uses stale DLLs,
write the migration manually:

1. Create the migration file in `backend/MerkERP.DAL/Migrations/`:
   Filename: `YYYYMMDDHHMMSS_AddMyEntity.cs`

   Minimum content:
   ```csharp
   using Microsoft.EntityFrameworkCore.Migrations;

   public partial class AddMyEntity : Migration
   {
       protected override void Up(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.CreateTable(
               name: "MyEntity_cs",
               columns: table => new
               {
                   Id = table.Column<long>(nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   InternalCode = table.Column<string>(nullable: true),
                   Name_EN = table.Column<string>(nullable: false),
                   Name_AR = table.Column<string>(nullable: true),
                   IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                   InsertedBy = table.Column<long>(nullable: true),
                   InsertedDate = table.Column<DateTime>(nullable: true),
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_MyEntity_cs", x => x.Id);
               });
       }

       protected override void Down(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.DropTable(name: "MyEntity_cs");
       }
   }
   ```

2. Apply via sqlcmd:
   ```sql
   IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MyEntity_cs')
   BEGIN
     CREATE TABLE MyEntity_cs (
       Id           BIGINT IDENTITY(1,1) PRIMARY KEY,
       InternalCode NVARCHAR(100) NULL,
       Name_EN      NVARCHAR(200) NOT NULL,
       Name_AR      NVARCHAR(200) NULL,
       IsActive     BIT NOT NULL DEFAULT 1,
       InsertedBy   BIGINT NULL,
       InsertedDate DATETIME2 NULL
     );
   END
   ```

3. Record in EF history:
   ```sql
   INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
   VALUES ('YYYYMMDDHHMMSS_AddMyEntity', '9.0.0');
   ```

#### Step 4.4 — Create the API Controller

In `backend/MerkERP.API/Controllers/MyEntitiesController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

[ApiController]
[Route("api/[controller]")]
public class MyEntitiesController : ControllerBase
{
    private readonly MerkDbContext _db;
    public MyEntitiesController(MerkDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.MyEntity_cs.OrderBy(e => e.Name_EN).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id) =>
        await _db.MyEntity_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create(MyEntity_cs e)
    {
        _db.MyEntity_cs.Add(e);
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, MyEntity_cs e)
    {
        if (id != e.Id) return BadRequest();
        _db.MyEntity_cs.Update(e);
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var e = await _db.MyEntity_cs.FindAsync(id);
        if (e is null) return NotFound();
        _db.MyEntity_cs.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
    {
        var entities = await _db.MyEntity_cs.Where(e => ids.Contains(e.Id)).ToListAsync();
        _db.MyEntity_cs.RemoveRange(entities);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(long id)
    {
        var e = await _db.MyEntity_cs.FindAsync(id);
        if (e is null) return NotFound();
        e.IsActive = !e.IsActive;
        await _db.SaveChangesAsync();
        return Ok(e);
    }
}
```

#### Step 4.5 — Register the entity in TableName_s

```sql
-- Find next TableName_s Id
SELECT MAX(Id) FROM TableName_s;

INSERT INTO TableName_s (Id, Name, EntityKey)
VALUES (<nextId>, 'MyEntity_cs', 'myentities');
-- EntityKey must match the string Angular uses in the API call and the route
```

Record in EF history if you want this tracked:
```sql
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('YYYYMMDDHHMMSS_RegisterMyEntityMeta', '9.0.0');
```

#### Step 4.6 — Insert column rows into TableMetaData

Always declare Arabic strings in T-SQL variables first (see Section 3.3 warning).
Example for a typical entity with InternalCode, Name AR, Name EN, IsActive:

```sql
DECLARE @ar1 NVARCHAR(200) = N'الكود الداخلي';
DECLARE @ar2 NVARCHAR(200) = N'الاسم (AR)';
DECLARE @ar3 NVARCHAR(200) = N'الاسم (EN)';
DECLARE @ar4 NVARCHAR(200) = N'نشط';

DECLARE @tblId INT = (SELECT Id FROM TableName_s WHERE EntityKey = 'myentities');
DECLARE @baseId BIGINT = (SELECT MAX(Id) FROM TableMetaData);

INSERT INTO TableMetaData
  (Id, TableNameId, [Key], LabelEN, LabelAR, ColumnOrder,
   EntityProperty, ForeignKeyProperty, FilterType, DataType,
   RenderAs, IsSortable, IsFilterable, IsVisible, MinWidth)
VALUES
  (@baseId+1, @tblId, 'internalCode', 'Internal Code', @ar1, 1, 'InternalCode', NULL, 'text',    'string',  'text',  1, 1, 1, NULL),
  (@baseId+2, @tblId, 'name_AR',      'Name (AR)',      @ar2, 2, 'Name_AR',      NULL, 'text',    'string',  'text',  1, 1, 1, NULL),
  (@baseId+3, @tblId, 'name_EN',      'Name (EN)',      @ar3, 3, 'Name_EN',      NULL, 'text',    'string',  'text',  1, 1, 1, NULL),
  (@baseId+4, @tblId, 'isActive',     'Active',         @ar4, 4, 'IsActive',     NULL, 'boolean', 'boolean', 'badge', 1, 1, 1, NULL);
```

---

### FRONTEND STEPS

#### Step 4.7 — Add the Angular route

In `frontend/src/app/app.routes.ts`, inside the `stock` children array:

```typescript
{
  path: 'myentities',
  loadComponent: () => import('./features/my-entities/my-entities.component')
    .then(m => m.MyEntitiesComponent)
},
{
  path: 'myentities/operation',
  loadComponent: () => import('./features/my-entities/operations/my-entities-operation.component')
    .then(m => m.MyEntitiesOperationComponent)
},
{
  path: 'myentities/operation/:id',
  loadComponent: () => import('./features/my-entities/operations/my-entities-operation.component')
    .then(m => m.MyEntitiesOperationComponent)
},
```

#### Step 4.8 — Add the sidebar nav link

In `frontend/src/app/shared/components/sidebar/sidebar.service.ts`, add an item
to the appropriate section:

```typescript
{ label: 'nav.my_entities', icon: 'ki-outline ki-some-icon', route: '/stock/myentities' }
```

Add translation keys in both translation files:
- `frontend/public/assets/i18n/en.json` → `"my_entities": "My Entities"`
- `frontend/public/assets/i18n/ar.json` → `"my_entities": "كياناتي"`

#### Step 4.9 — Create the List component

Create folder: `frontend/src/app/features/my-entities/`

**my-entities.component.ts** — minimal working structure:

```typescript
import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { ColumnMeta, MetadataService } from '../../core/metadata.service';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { CustomTableWithPaginationComponent } from '../../shared/components/custom-controls/custom-table-with-pagination/custom-table-with-pagination.component';

interface MyEntity { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; isActive: boolean; }

@Component({
  selector: 'app-my-entities',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent,
            RegularListSearchActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './my-entities.component.html',
})
export class MyEntitiesComponent implements OnInit {
  private api     = inject(ApiService);
  private router  = inject(Router);
  private translate = inject(TranslateService);
  private toastr  = inject(ToastrService);
  private meta    = inject(MetadataService);
  private doc     = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  items         = signal<MyEntity[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  columnMeta    = signal<ColumnMeta[]>([]);
  activeFilter  = signal<Record<string, string | number | null>>({});

  // Add cellRenderers here if any FK columns need custom display
  readonly cellRenderers: Record<string, (item: any) => string> = {};

  get searchFields(): SearchField[] {
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl);
  }

  get displayRows(): MyEntity[] {
    const f = this.activeFilter();
    const anyActive = Object.values(f).some(v => v != null && v !== '');
    if (!anyActive) return this.items();
    return this.items().filter(item => {
      if (f['internalCode'] && !(item.internalCode ?? '').toLowerCase()
          .includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_EN'] && !item.name_EN.toLowerCase()
          .includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['name_AR'] && !(item.name_AR ?? '').toLowerCase()
          .includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['isActive'] != null && item.isActive !== (f['isActive'] === 1)) return false;
      return true;
    });
  }

  ngOnInit() { this.load(); }

  load() {
    this.metaReloadKey.update(n => n + 1);
    this.api.get<MyEntity[]>('myentities').subscribe(d => this.items.set(d));
  }

  addNew()  { this.router.navigate(['/stock/myentities/operation']); }
  edit(id: number) { this.router.navigate(['/stock/myentities/operation', id]); }

  onFilterChange(f: Record<string, string | number | null>) { this.activeFilter.set(f); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      icon: 'warning', showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => {
      if (r.isConfirmed) this.api.delete(`myentities/${id}`).subscribe(() => this.load());
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      icon: 'warning', showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => {
      if (r.isConfirmed)
        this.api.deleteBulk('myentities/bulk', ids).subscribe(() => this.load());
    });
  }
}
```

**my-entities.component.html**:

```html
<app-regular-list-header-with-actions
  [title]="'my_entities.title' | translate"
  icon="ki-outline ki-some-icon"
  color="primary"
  [selectedCount]="selectedIds().size"
  (add)="addNew()"
  (refresh)="load()"
  (deleteSelected)="deleteSelected()" />

<app-regular-list-search-actions
  [fields]="searchFields"
  (filterChange)="onFilterChange($event)" />

<app-custom-table-with-pagination
  entity="myentities"
  [rows]="displayRows"
  [cellRenderers]="cellRenderers"
  (metadataLoaded)="columnMeta.set($event)"
  (selectionChange)="selectedIds.set($event)"
  [reload]="metaReloadKey()">

  <ng-template #rowActions let-item>
    <div class="d-inline-flex align-items-center gap-2">
      <button class="btn btn-icon btn-sm btn-light-primary"
              (click)="edit(item.id)"
              [title]="'common.edit' | translate">
        <i class="ki-outline ki-pencil fs-4"></i>
      </button>
      <button class="btn btn-icon btn-sm btn-light-danger"
              (click)="delete(item.id)"
              [title]="'common.delete' | translate">
        <i class="ki-outline ki-trash fs-4"></i>
      </button>
    </div>
  </ng-template>
</app-custom-table-with-pagination>
```

#### Step 4.10 — Create the Operation component

Create folder: `frontend/src/app/features/my-entities/operations/`

**my-entities-operation.component.ts** — structure:

```typescript
import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { LoginService } from '../../../shared/services/login.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';

interface MyEntity {
  id: number;
  internalCode: string | null;
  name_EN: string;
  name_AR: string | null;
  isActive: boolean;
  insertedBy: number | null;
  insertedDate: string | null;
}

@Component({
  selector: 'app-my-entities-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent],
  templateUrl: './my-entities-operation.component.html',
})
export class MyEntitiesOperationComponent implements OnInit {
  private api     = inject(ApiService);
  private login   = inject(LoginService);
  private router  = inject(Router);
  private route   = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr  = inject(ToastrService);
  private doc     = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit   = signal(false);
  saving   = signal(false);
  savingNew = signal(false);
  form: Partial<MyEntity> = this.blank();

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<MyEntity>(`myentities/${id}`).subscribe(e => this.form = { ...e });
    }
  }

  private blank(): Partial<MyEntity> {
    return { id: 0, internalCode: null, name_EN: '', name_AR: null,
             isActive: true, insertedBy: null, insertedDate: null };
  }

  private validate(): boolean {
    if (!this.form.name_EN?.trim()) {
      this.toastr.error(
        this.translate.instant('common.name'),
        this.translate.instant('common.validation_error'));
      return false;
    }
    return true;
  }

  private submit(andNew: boolean) {
    if (!this.validate()) return;
    andNew ? this.savingNew.set(true) : this.saving.set(true);

    if (!this.isEdit()) {
      this.form.insertedBy   = this.login.currentUserId();
      this.form.insertedDate = new Date().toISOString();
    }

    const req = this.isEdit()
      ? this.api.put<MyEntity>(`myentities/${this.form.id}`, this.form)
      : this.api.post<MyEntity>('myentities', this.form);

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          this.form = this.blank();
          this.isEdit.set(false);
          this.savingNew.set(false);
        } else {
          this.back();
        }
      },
      error: () => { this.saving.set(false); this.savingNew.set(false); },
    });
  }

  save()      { this.submit(false); }
  saveAndNew() { this.submit(true); }
  resetForm() { this.form = this.blank(); }
  back()      { this.router.navigate(['/stock/myentities']); }
}
```

**my-entities-operation.component.html** — minimal structure:

```html
<app-regular-operation-header
  [title]="(isEdit() ? 'my_entities.edit_title' : 'my_entities.add_title') | translate"
  icon="ki-outline ki-some-icon"
  color="success"
  (cancel)="back()" />

<div class="card">
  <div class="card-body">
    <div class="row g-4">

      <div class="col-3">
        <label class="form-label">{{ 'common.internal_code' | translate }}</label>
        <input class="form-control" [(ngModel)]="form.internalCode" name="internalCode"
          [placeholder]="'common.internal_code' | translate">
      </div>

      <div class="col-3">
        <label class="form-label">{{ 'common.name' | translate }} (AR)</label>
        <input class="form-control" [(ngModel)]="form.name_AR" name="name_AR" dir="rtl"
          [placeholder]="'common.name' | translate">
      </div>

      <div class="col-3">
        <label class="form-label required">{{ 'common.name' | translate }} (EN)</label>
        <input class="form-control" [(ngModel)]="form.name_EN" name="name_EN"
          [placeholder]="'common.name' | translate">
      </div>

    </div>
  </div>

  <app-regular-operation-actions
    [saving]="saving()"
    [savingNew]="savingNew()"
    (save)="save()"
    (saveAndNew)="saveAndNew()"
    (reset)="resetForm()" />
</div>
```

#### Step 4.11 — Add translation keys

In `frontend/public/assets/i18n/en.json`:
```json
"my_entities": {
  "title":      "My Entities",
  "add_title":  "New Entity",
  "edit_title": "Edit Entity",
  "delete_confirm": "Delete this entity?",
  "delete_selected_confirm": "Delete {{count}} selected entities?"
}
```

In `frontend/public/assets/i18n/ar.json`:
```json
"my_entities": {
  "title":      "كياناتي",
  "add_title":  "كيان جديد",
  "edit_title": "تعديل الكيان",
  "delete_confirm": "حذف هذا الكيان؟",
  "delete_selected_confirm": "حذف {{count}} كيانات محددة؟"
}
```

---

## 5. Critical Rules & Common Mistakes

### Rule 1 — Arabic in SQL variables only
Never put Arabic text directly in a VALUES(...) clause inside a sqlcmd -Q string.
The RTL bidi characters corrupt the position of the surrounding values.
Always use: `DECLARE @ar NVARCHAR(200) = N'...'; ... VALUES(..., @ar, ...)`

### Rule 2 — Key must be camelCase and match exactly
The `Key` in `TableMetaData` must exactly match:
- The property name on the JavaScript object returned by the API (camelCase from System.Text.Json)
- The key in `cellRenderers` in the Angular list component
- The key in the filter options passed to `meta.toSearchFields()`

Example: if the C# property is `WareHouseType`, JSON serialization gives `wareHouseType`,
so the Key in the DB must be `wareHouseType`.

### Rule 3 — Always record manual DB changes in __EFMigrationsHistory
If you apply SQL manually (because the API is running and you can't run `dotnet ef`),
always insert a row into `__EFMigrationsHistory` with a unique MigrationId.
Otherwise the next `dotnet ef database update` will fail or try to apply it again.

### Rule 4 — ForeignKeyProperty must be the Id field name, not the navigation property
For a column that shows a resolved name:
- `EntityProperty` = `WareHouseType`  (the navigation property / display concept)
- `ForeignKeyProperty` = `WareHouseTypeId`  (the actual FK column on the table)

### Rule 5 — Metadata cache TTL is 30 minutes
After adding a DB column, a hard browser refresh (Ctrl+Shift+R) is needed to force
the Angular MetadataService to re-fetch from the API instead of returning the cached copy.

### Rule 6 — InsertedBy must come from LoginService, not be hardcoded
In every operation component, inject `LoginService` and set:
```typescript
this.form.insertedBy   = this.login.currentUserId();
this.form.insertedDate = new Date().toISOString();
```
Do this only in the create path (when `!this.isEdit()`).

---

## 6. Quick-Reference Checklist

### Adding a column to an existing table
- [ ] Find TableNameId for the entity in TableName_s
- [ ] Find MAX(Id) in TableMetaData for the next Id
- [ ] Declare Arabic text in a T-SQL variable
- [ ] INSERT the new row into TableMetaData
- [ ] Record the change in __EFMigrationsHistory
- [ ] If FK column: add cellRenderer in the list TS
- [ ] If FilterType=select: add options in searchFields getter
- [ ] If IsFilterable=true: add filter condition in flatFiltered / displayRows
- [ ] Hard refresh the browser

### Registering a new table
- [ ] Create C# model in MerkERP.Core/Models/
- [ ] Add DbSet to MerkDbContext
- [ ] Write and apply CREATE TABLE SQL manually
- [ ] Record table migration in __EFMigrationsHistory
- [ ] Create API Controller with Get/Create/Update/Delete/ToggleActive
- [ ] INSERT into TableName_s (with EntityKey)
- [ ] INSERT column rows into TableMetaData (Arabic in variables)
- [ ] Record metadata migration in __EFMigrationsHistory
- [ ] Add route entries in app.routes.ts
- [ ] Add sidebar link in sidebar.service.ts
- [ ] Create list component (TS + HTML) with cellRenderers and searchFields
- [ ] Create operation component (TS + HTML) with LoginService for InsertedBy
- [ ] Add translation keys in en.json and ar.json
- [ ] Hard refresh the browser
