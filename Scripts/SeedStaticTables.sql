-- ============================================================
-- SeedStaticTables.sql
-- Idempotent seed for all *_s (static / lookup) tables.
-- Safe to run any number of times — inserts only missing rows.
-- Add new rows here whenever a new lookup value is needed,
-- then run this script against the target database.
-- ============================================================

SET NOCOUNT ON;

-- ── BarcodeType_s ─────────────────────────────────────────────────────────────
--    PK: BarcodeTypeId (IDENTITY) | Enum: BarcodeTypeEnum
SET IDENTITY_INSERT BarcodeType_s ON;
MERGE BarcodeType_s AS target
USING (VALUES
    ( 1, 'EAN-13'     ),
    ( 2, 'EAN-8'      ),
    ( 3, 'UPC-A'      ),
    ( 4, 'UPC-E'      ),
    ( 5, 'Code 39'    ),
    ( 6, 'Code 128'   ),
    ( 7, 'ITF-14'     ),
    ( 8, 'GS1-128'    ),
    ( 9, 'QR Code'    ),
    (10, 'Data Matrix'),
    (11, 'Custom'     )
) AS src (BarcodeTypeId, Name)
ON  target.BarcodeTypeId = src.BarcodeTypeId
WHEN NOT MATCHED THEN
    INSERT (BarcodeTypeId, Name)
    VALUES (src.BarcodeTypeId, src.Name);
SET IDENTITY_INSERT BarcodeType_s OFF;

-- ── InventoryValidationMethod_s ───────────────────────────────────────────────
--    PK: InventoryValidationMethodId (IDENTITY) | Enum: InventoryValidationMethodEnum
SET IDENTITY_INSERT InventoryValidationMethod_s ON;
MERGE InventoryValidationMethod_s AS target
USING (VALUES
    (1, 'FIFO'          ),
    (2, 'LIFO'          ),
    (3, 'Moving Average')
) AS src (InventoryValidationMethodId, Name)
ON  target.InventoryValidationMethodId = src.InventoryValidationMethodId
WHEN NOT MATCHED THEN
    INSERT (InventoryValidationMethodId, Name)
    VALUES (src.InventoryValidationMethodId, src.Name);
SET IDENTITY_INSERT InventoryValidationMethod_s OFF;

-- ── ItemType_s ────────────────────────────────────────────────────────────────
--    PK: ItemTypeId (IDENTITY)
SET IDENTITY_INSERT ItemType_s ON;
MERGE ItemType_s AS target
USING (VALUES
    (1, 'Stock Item'    ),
    (2, 'Service'       ),
    (3, 'Non-Stock Item')
) AS src (ItemTypeId, Name)
ON  target.ItemTypeId = src.ItemTypeId
WHEN NOT MATCHED THEN
    INSERT (ItemTypeId, Name, IsActive, IsFavorite)
    VALUES (src.ItemTypeId, src.Name, 1, 0);
SET IDENTITY_INSERT ItemType_s OFF;

-- ── WareHouseType_s ───────────────────────────────────────────────────────────
--    PK: Id (IDENTITY) | Enum: WareHouseTypeEnum
SET IDENTITY_INSERT WareHouseType_s ON;
MERGE WareHouseType_s AS target
USING (VALUES
    (1, 'Pharmaceutical Warehouses',             N'مستودعات الأدوية'                            ),
    (2, 'Consumables & PPE Warehouses',          N'مستودعات المستهلكات ومعدات الوقاية الشخصية' ),
    (3, 'Medical Device & Equipment Warehouses', N'مستودعات الأجهزة والمعدات الطبية'           ),
    (4, 'Sterile Surgical Warehouses',           N'مستودعات العمليات الجراحية المعقمة'         )
) AS src (Id, Name_EN, Name_AR)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name_EN, Name_AR)
    VALUES (src.Id, src.Name_EN, src.Name_AR);
SET IDENTITY_INSERT WareHouseType_s OFF;

-- ── StockTransactionType_s ────────────────────────────────────────────────────
--    PK: Id (no IDENTITY — ValueGeneratedNever)
MERGE StockTransactionType_s AS target
USING (VALUES
    (1, 'Opening Balance',      N'رصيد افتتاحي'),
    (2, 'Stock Reconciliation', N'جرد المخزون' ),
    (3, 'Stock Receipt',        N'استلام مخزون'),
    (4, 'Stock Issue',          N'صرف مخزون'   ),
    (5, 'Stock Transfer',       N'تحويل مخزون' )
) AS src (Id, Name_EN, Name_AR)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name_EN, Name_AR)
    VALUES (src.Id, src.Name_EN, src.Name_AR);

-- ── StockTransactionStatus_s ──────────────────────────────────────────────────
--    PK: Id (no IDENTITY — ValueGeneratedNever) | Enum: StockTransactionStatusEnum
MERGE StockTransactionStatus_s AS target
USING (VALUES
    (1, 'Draft',     N'مسودة'       ),
    (2, 'Pending',   N'قيد الانتظار'),
    (3, 'Submitted', N'إعتماد'     ),
    (4, 'Cancelled', N'ملغى'        ),
    (5, 'Amended',   N'معدَّل'      ),
	(6, 'Reissued',   N'إعادة إصدار'      )
) AS src (Id, Name_EN, Name_AR)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name_EN, Name_AR)
    VALUES (src.Id, src.Name_EN, src.Name_AR);

-- ── UserType_s ────────────────────────────────────────────────────────────────
--    PK: Id (IDENTITY) | Enum: UserTypeEnum
SET IDENTITY_INSERT UserType_s ON;
MERGE UserType_s AS target
USING (VALUES
    (1, 'Root',         N'روت'        ),
    (2, 'Merk Admin',   N'مدير ميرك'  ),
    (3, 'Admin',        N'مدير'       ),
    (4, 'Regular User', N'مستخدم عادي')
) AS src (Id, Name_EN, Name_AR)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name_EN, Name_AR)
    VALUES (src.Id, src.Name_EN, src.Name_AR);
SET IDENTITY_INSERT UserType_s OFF;

-- ── TableMetaData_FilterType_s ────────────────────────────────────────────────
--    PK: Id (no IDENTITY — ValueGeneratedNever) | Enum: TableMetaDataFilterTypeEnum
MERGE TableMetaData_FilterType_s AS target
USING (VALUES
    (1, 'text'   ),
    (2, 'number' ),
    (3, 'boolean'),
    (4, 'select' )
) AS src (Id, Name)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name) VALUES (src.Id, src.Name);

-- ── TableMetaData_DataType_s ──────────────────────────────────────────────────
--    PK: Id (no IDENTITY — ValueGeneratedNever) | Enum: TableMetaDataDataTypeEnum
MERGE TableMetaData_DataType_s AS target
USING (VALUES
    (1, 'string' ),
    (2, 'number' ),
    (3, 'boolean'),
    (4, 'date'   )
) AS src (Id, Name)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name) VALUES (src.Id, src.Name);

-- ── TableMetaData_RenderAs_s ──────────────────────────────────────────────────
--    PK: Id (no IDENTITY — ValueGeneratedNever) | Enum: TableMetaDataRenderAsEnum
MERGE TableMetaData_RenderAs_s AS target
USING (VALUES
    (1, 'text' ),
    (2, 'badge'),
    (3, 'yesno'),
    (4, 'tree' )
) AS src (Id, Name)
ON  target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Name) VALUES (src.Id, src.Name);

-- ── Done ──────────────────────────────────────────────────────────────────────
PRINT 'Static tables seeded successfully.';
