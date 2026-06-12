using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;

namespace MerkERP.DAL.Context;

public class MerkDbContext : DbContext
{
	public MerkDbContext(DbContextOptions<MerkDbContext> options) : base(options) { }

	public DbSet<TableName_s>  TableName_s  { get; set; }
	public DbSet<TableMetaData> TableMetaData { get; set; }
	public DbSet<Branch_cs> Branch_cs { get; set; }
	public DbSet<WareHouseCategory_cs> WareHouseCategory_cs { get; set; }
	public DbSet<WareHouseType_s> WareHouseType_s { get; set; }
	public DbSet<BarcodeType_s> BarcodeType_s { get; set; }
	public DbSet<InventoryValidationMethod_s> InventoryValidationMethod_s { get; set; }
	public DbSet<ItemType_s> ItemType_s { get; set; }
	public DbSet<ItemGroup_cs> ItemGroup_cs { get; set; }
	public DbSet<UOM_cs> UOM_cs { get; set; }
	public DbSet<Item_cs> Item_cs { get; set; }
	public DbSet<Item_UOM_Barcode_cs> Item_UOM_Barcode_cs { get; set; }
	public DbSet<ItemUOMConversion_cs> ItemUOMConversion_cs { get; set; }
	public DbSet<UOMConversionFactor_cs> UOMConversionFactor_cs { get; set; }
	public DbSet<UOMConversionGroup_cs> UOMConversionGroup_cs { get; set; }
	public DbSet<WareHouse_cs> WareHouse_cs { get; set; }
	public DbSet<WarehouseTransaction> WarehouseTransaction { get; set; }
	public DbSet<StockLedgerTransaction> StockLedgerTransaction { get; set; }

	protected override void OnModelCreating(ModelBuilder m)
	{
		// Explicit PKs (class names contain underscores so EF convention doesn't match)
		m.Entity<TableName_s>().HasKey(e => e.Id);
		m.Entity<TableName_s>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<TableName_s>().Property(e => e.Name).HasColumnType("nvarchar(200)").IsRequired();
		m.Entity<TableName_s>().Property(e => e.EntityKey).HasColumnType("nvarchar(100)");
		m.Entity<TableName_s>().HasIndex(e => e.Name).IsUnique();

		m.Entity<TableMetaData>().HasKey(e => e.Id);
		m.Entity<TableMetaData>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<TableMetaData>().HasOne(e => e.TableName).WithMany().HasForeignKey(e => e.TableNameId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<TableMetaData>().HasIndex(e => new { e.TableNameId, e.Key }).IsUnique();
		m.Entity<TableMetaData>().Property(e => e.Key).HasColumnType("nvarchar(100)").IsRequired();
		m.Entity<TableMetaData>().Property(e => e.LabelEN).HasColumnType("nvarchar(200)").IsRequired();
		m.Entity<TableMetaData>().Property(e => e.LabelAR).HasColumnType("nvarchar(200)").IsRequired();
		m.Entity<TableMetaData>().Property(e => e.EntityProperty).HasColumnType("nvarchar(100)").IsRequired();
		m.Entity<TableMetaData>().Property(e => e.ForeignKeyProperty).HasColumnType("nvarchar(100)");
		m.Entity<TableMetaData>().Property(e => e.FilterType).HasColumnType("nvarchar(20)");
		m.Entity<TableMetaData>().Property(e => e.DataType).HasColumnType("nvarchar(20)");
		m.Entity<TableMetaData>().Property(e => e.RenderAs).HasColumnType("nvarchar(20)");
		m.Entity<Branch_cs>().HasKey(e => e.Id);
		m.Entity<WareHouseCategory_cs>().HasKey(e => e.Id);
		m.Entity<WareHouseType_s>().HasKey(e => e.Id);
		m.Entity<BarcodeType_s>().HasKey(e => e.BarcodeTypeId);
		m.Entity<InventoryValidationMethod_s>().HasKey(e => e.InventoryValidationMethodId);
		m.Entity<Item_UOM_Barcode_cs>().HasKey(e => e.Id);
		m.Entity<ItemType_s>().HasKey(e => e.ItemTypeId);
		m.Entity<ItemGroup_cs>().HasKey(e => e.ItemGroupId);
		m.Entity<UOM_cs>().HasKey(e => e.Id);
		m.Entity<Item_cs>().HasKey(e => e.Id);
		m.Entity<ItemUOMConversion_cs>().HasKey(e => e.Id);
		m.Entity<UOMConversionFactor_cs>().HasKey(e => e.Id);
		m.Entity<UOMConversionGroup_cs>().HasKey(e => e.Id);
		m.Entity<WareHouse_cs>().HasKey(e => e.Id);
		m.Entity<WarehouseTransaction>().HasKey(e => e.BinId);
		m.Entity<StockLedgerTransaction>().HasKey(e => e.SLTId);

		m.Entity<ItemGroup_cs>()
			.HasOne(g => g.ParentItemGroup)
			.WithMany(g => g.Children)
			.HasForeignKey(g => g.ParentItemGroupId)
			.OnDelete(DeleteBehavior.Restrict);

		m.Entity<WareHouse_cs>()
			.HasOne(w => w.ParentWarehouse)
			.WithMany(w => w.Children)
			.HasForeignKey(w => w.ParentWarehouseId)
			.OnDelete(DeleteBehavior.Restrict);

		m.Entity<WareHouse_cs>()
			.HasOne(w => w.WareHouseType).WithMany().HasForeignKey(w => w.WareHouseTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<WareHouse_cs>()
			.HasOne(w => w.WareHouseCategory).WithMany().HasForeignKey(w => w.WareHouseCategoryId).OnDelete(DeleteBehavior.Restrict);

		// Prevent multiple cascade paths to same table
		m.Entity<Item_cs>()
			.HasOne(i => i.ItemGroup).WithMany(g => g.Items).HasForeignKey(i => i.ItemGroupId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_cs>()
			.HasOne(i => i.ItemType).WithMany(t => t.Items).HasForeignKey(i => i.ItemTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_cs>()
			.HasOne(i => i.DefaultUOM).WithMany(u => u.DefaultForItems).HasForeignKey(i => i.DefaultUOMId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_cs>()
			.HasOne(i => i.DefaultPurchaseUOM).WithMany().HasForeignKey(i => i.DefaultPurchaseUOMId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_cs>()
			.HasOne(i => i.DefaultSellingUOM).WithMany().HasForeignKey(i => i.DefaultSellingUOMId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_cs>()
			.HasOne(i => i.InventoryValidationMethod).WithMany().HasForeignKey(i => i.InventoryValidationMethodId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_cs>()
			.HasOne(i => i.DefaultWarehouse).WithMany().HasForeignKey(i => i.DefaultWarehouseId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<Item_UOM_Barcode_cs>()
			.HasOne(b => b.Item).WithMany(i => i.Barcodes).HasForeignKey(b => b.ItemId).OnDelete(DeleteBehavior.Cascade);
		m.Entity<Item_UOM_Barcode_cs>()
			.HasOne(b => b.BarcodeType).WithMany().HasForeignKey(b => b.BarcodeTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<Item_UOM_Barcode_cs>()
			.HasOne(b => b.UOM).WithMany().HasForeignKey(b => b.UOMId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<ItemUOMConversion_cs>()
			.HasOne(c => c.Item).WithMany(i => i.UOMConversions).HasForeignKey(c => c.ItemId).OnDelete(DeleteBehavior.Cascade);
		m.Entity<ItemUOMConversion_cs>()
			.HasOne(c => c.UOM).WithMany(u => u.Conversions).HasForeignKey(c => c.UOMId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<UOMConversionFactor_cs>()
			.HasOne(g => g.UOMFrom).WithMany().HasForeignKey(g => g.UOMFromId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<UOMConversionFactor_cs>()
			.HasOne(g => g.UOMTo).WithMany().HasForeignKey(g => g.UOMToId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<UOMConversionFactor_cs>()
			.HasOne(g => g.UOMConversionGroup).WithMany().HasForeignKey(g => g.UOMConversionGroupId).OnDelete(DeleteBehavior.SetNull);

		m.Entity<WarehouseTransaction>()
			.HasOne(b => b.Item).WithMany(i => i.WarehouseTransactions).HasForeignKey(b => b.ItemId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<WarehouseTransaction>()
			.HasOne(b => b.Warehouse).WithMany(w => w.Transactions).HasForeignKey(b => b.WarehouseId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<StockLedgerTransaction>()
			.HasOne(s => s.Item).WithMany(i => i.StockLedgerTransactions).HasForeignKey(s => s.ItemId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockLedgerTransaction>()
			.HasOne(s => s.Warehouse).WithMany(w => w.StockLedgerTransactions).HasForeignKey(s => s.WarehouseId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<WarehouseTransaction>()
			.HasIndex(b => new { b.ItemId, b.WarehouseId })
			.IsUnique();

		m.Entity<Item_cs>()
			.HasIndex(i => i.InternalCode)
			.IsUnique();

		m.Entity<Item_cs>()
			.Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<Item_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<Item_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");

		m.Entity<UOM_cs>()
			.Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<UOM_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<UOM_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");

		m.Entity<UOMConversionGroup_cs>()
			.Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<UOMConversionGroup_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(100)");
		m.Entity<UOMConversionGroup_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(100)");

		m.Entity<WareHouse_cs>()
			.Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<WareHouse_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<WareHouse_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");
		m.Entity<WareHouse_cs>()
			.Property(e => e.Description).HasColumnType("nvarchar(max)");

		m.Entity<Branch_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<Branch_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");
		m.Entity<Branch_cs>()
			.Property(e => e.Description).HasColumnType("nvarchar(max)");

		m.Entity<WareHouseCategory_cs>()
			.Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<WareHouseCategory_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<WareHouseCategory_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");
		m.Entity<WareHouseCategory_cs>()
			.Property(e => e.Description).HasColumnType("nvarchar(max)");

		m.Entity<WareHouseType_s>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<WareHouseType_s>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");

		m.Entity<TableName_s>().HasData(
			new TableName_s { Id = 1,  Name = "BarcodeType_s",               EntityKey = null                    },
			new TableName_s { Id = 2,  Name = "Branch_cs",                   EntityKey = "branches"              },
			new TableName_s { Id = 3,  Name = "InventoryValidationMethod_s", EntityKey = null                    },
			new TableName_s { Id = 4,  Name = "Item_cs",                     EntityKey = "items"                 },
			new TableName_s { Id = 5,  Name = "Item_UOM_Barcode_cs",         EntityKey = null                    },
			new TableName_s { Id = 6,  Name = "ItemGroup_cs",                EntityKey = "itemgroups"            },
			new TableName_s { Id = 7,  Name = "ItemType_s",                  EntityKey = null                    },
			new TableName_s { Id = 8,  Name = "ItemUOMConversion_cs",        EntityKey = null                    },
			new TableName_s { Id = 9,  Name = "StockLedgerTransaction",      EntityKey = null                    },
			new TableName_s { Id = 10, Name = "TableName_s",                 EntityKey = null                    },
			new TableName_s { Id = 11, Name = "UOM_cs",                      EntityKey = "uoms"                  },
			new TableName_s { Id = 12, Name = "UOMConversionFactor_cs",      EntityKey = "uomconversionfactors"  },
			new TableName_s { Id = 13, Name = "UOMConversionGroup_cs",       EntityKey = "uomconversiongroups"   },
			new TableName_s { Id = 14, Name = "WareHouse_cs",                EntityKey = "warehouses"            },
			new TableName_s { Id = 15, Name = "WareHouseCategory_cs",        EntityKey = "warehousecategories"   },
			new TableName_s { Id = 16, Name = "WareHouseType_s",             EntityKey = null                    },
			new TableName_s { Id = 17, Name = "WarehouseTransaction",        EntityKey = null                    },
			new TableName_s { Id = 18, Name = "TableMetaData",               EntityKey = null                    }
		);

		// ── TableMetaData seed ── (mirrors MetadataController column definitions)
		m.Entity<TableMetaData>().HasData(

			// items  (TableNameId = 4)
			new TableMetaData { Id = 1,  TableNameId = 4,  ColumnOrder =1, Key = "internalCode",      LabelEN = "Code",             LabelAR = "الكود",             EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 2,  TableNameId = 4,  ColumnOrder =2, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 3,  TableNameId = 4,  ColumnOrder =3, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN"                                                                                 },
			new TableMetaData { Id = 4,  TableNameId = 4,  ColumnOrder =4, Key = "itemGroup",         LabelEN = "Item Group",       LabelAR = "مجموعة الصنف",      EntityProperty = "ItemGroup",  ForeignKeyProperty = "ItemGroupId",  FilterType = "select"                  },
			new TableMetaData { Id = 5,  TableNameId = 4,  ColumnOrder =5, Key = "itemType",          LabelEN = "Item Type",        LabelAR = "نوع الصنف",         EntityProperty = "ItemType",   ForeignKeyProperty = "ItemTypeId",   FilterType = "select"                  },
			new TableMetaData { Id = 6,  TableNameId = 4,  ColumnOrder =6, Key = "defaultUOM",        LabelEN = "Default UOM",      LabelAR = "وحدة القياس",       EntityProperty = "DefaultUOM", ForeignKeyProperty = "DefaultUOMId", FilterType = "select"                  },
			new TableMetaData { Id = 7,  TableNameId = 4,  ColumnOrder =7, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// itemgroups  (TableNameId = 6)
			new TableMetaData { Id = 8,  TableNameId = 6,  ColumnOrder =1, Key = "internalCode",      LabelEN = "Internal Code",    LabelAR = "الكود الداخلي",     EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 9,  TableNameId = 6,  ColumnOrder =2, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 10, TableNameId = 6,  ColumnOrder =3, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN"                                                                                 },
			new TableMetaData { Id = 11, TableNameId = 6,  ColumnOrder =4, Key = "parentItemGroup",   LabelEN = "Parent Group",     LabelAR = "المجموعة الأصل",    EntityProperty = "ParentItemGroup", ForeignKeyProperty = "ParentItemGroupId", FilterType = "select"       },
			new TableMetaData { Id = 12, TableNameId = 6,  ColumnOrder =5, Key = "isMain",            LabelEN = "Is Main",          LabelAR = "أصل",               EntityProperty = "IsMain",                                          FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },
			new TableMetaData { Id = 13, TableNameId = 6,  ColumnOrder =6, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// uoms  (TableNameId = 11)
			new TableMetaData { Id = 14, TableNameId = 11, ColumnOrder =1, Key = "internalCode",      LabelEN = "Internal Code",    LabelAR = "الكود الداخلي",     EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 15, TableNameId = 11, ColumnOrder =2, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 16, TableNameId = 11, ColumnOrder =3, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN"                                                                                 },
			new TableMetaData { Id = 17, TableNameId = 11, ColumnOrder =4, Key = "mustBeWholeNumber", LabelEN = "Must Be Whole No.", LabelAR = "يجب أن يكون صحيحاً", EntityProperty = "MustBeWholeNumber",                             FilterType = "boolean", DataType = "boolean", RenderAs = "yesno" },
			new TableMetaData { Id = 18, TableNameId = 11, ColumnOrder =5, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// uomconversiongroups  (TableNameId = 13)
			new TableMetaData { Id = 19, TableNameId = 13, ColumnOrder =1, Key = "internalCode",      LabelEN = "Internal Code",    LabelAR = "الكود الداخلي",     EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 20, TableNameId = 13, ColumnOrder =2, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 21, TableNameId = 13, ColumnOrder =3, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN"                                                                                 },
			new TableMetaData { Id = 22, TableNameId = 13, ColumnOrder =4, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// uomconversionfactors  (TableNameId = 12)
			new TableMetaData { Id = 23, TableNameId = 12, ColumnOrder =1, Key = "internalCode",       LabelEN = "Internal Code",    LabelAR = "الكود الداخلي",    EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 24, TableNameId = 12, ColumnOrder =2, Key = "uomFrom",            LabelEN = "From UOM",         LabelAR = "من وحدة القياس",    EntityProperty = "UOMFrom",            ForeignKeyProperty = "UOMFromId",            FilterType = "select" },
			new TableMetaData { Id = 25, TableNameId = 12, ColumnOrder =3, Key = "uomTo",              LabelEN = "To UOM",           LabelAR = "إلى وحدة القياس",   EntityProperty = "UOMTo",              ForeignKeyProperty = "UOMToId",              FilterType = "select" },
			new TableMetaData { Id = 26, TableNameId = 12, ColumnOrder =4, Key = "value",              LabelEN = "Factor",           LabelAR = "معامل التحويل",     EntityProperty = "Value",                                           FilterType = "number",  DataType = "number"  },
			new TableMetaData { Id = 27, TableNameId = 12, ColumnOrder =5, Key = "uomConversionGroup", LabelEN = "Conversion Group", LabelAR = "مجموعة التحويل",    EntityProperty = "UOMConversionGroup", ForeignKeyProperty = "UOMConversionGroupId", FilterType = "select" },
			new TableMetaData { Id = 28, TableNameId = 12, ColumnOrder =6, Key = "isActive",           LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// warehouses  (TableNameId = 14)
			new TableMetaData { Id = 29, TableNameId = 14, ColumnOrder =1, Key = "internalCode",      LabelEN = "Internal Code",    LabelAR = "الكود الداخلي",     EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 30, TableNameId = 14, ColumnOrder =2, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 31, TableNameId = 14, ColumnOrder =3, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN",                                         RenderAs = "tree",      IsSortable = false },
			new TableMetaData { Id = 32, TableNameId = 14, ColumnOrder =4, Key = "wareHouseType",     LabelEN = "Type",             LabelAR = "النوع",             EntityProperty = "WareHouseType",     ForeignKeyProperty = "WareHouseTypeId",     FilterType = "select"  },
			new TableMetaData { Id = 33, TableNameId = 14, ColumnOrder =5, Key = "wareHouseCategory", LabelEN = "Category",         LabelAR = "الفئة",             EntityProperty = "WareHouseCategory", ForeignKeyProperty = "WareHouseCategoryId", FilterType = "select"  },
			new TableMetaData { Id = 34, TableNameId = 14, ColumnOrder =6, Key = "isParent",          LabelEN = "Is Parent",        LabelAR = "أصل",               EntityProperty = "IsParent",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },
			new TableMetaData { Id = 35, TableNameId = 14, ColumnOrder =7, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// warehousecategories  (TableNameId = 15)
			new TableMetaData { Id = 36, TableNameId = 15, ColumnOrder =1, Key = "internalCode",      LabelEN = "Internal Code",    LabelAR = "الكود الداخلي",     EntityProperty = "InternalCode"                                                                            },
			new TableMetaData { Id = 37, TableNameId = 15, ColumnOrder =2, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 38, TableNameId = 15, ColumnOrder =3, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN"                                                                                 },
			new TableMetaData { Id = 39, TableNameId = 15, ColumnOrder =4, Key = "description",       LabelEN = "Description",      LabelAR = "الوصف",             EntityProperty = "Description",                                     IsSortable = false                     },
			new TableMetaData { Id = 40, TableNameId = 15, ColumnOrder =5, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" },

			// branches  (TableNameId = 2)
			new TableMetaData { Id = 41, TableNameId = 2,  ColumnOrder =1, Key = "name_AR",           LabelEN = "Name (AR)",        LabelAR = "الاسم (AR)",        EntityProperty = "Name_AR"                                                                                 },
			new TableMetaData { Id = 42, TableNameId = 2,  ColumnOrder =2, Key = "name_EN",           LabelEN = "Name (EN)",        LabelAR = "الاسم (EN)",        EntityProperty = "Name_EN"                                                                                 },
			new TableMetaData { Id = 43, TableNameId = 2,  ColumnOrder =3, Key = "description",       LabelEN = "Description",      LabelAR = "الوصف",             EntityProperty = "Description",                                     IsSortable = false                     },
			new TableMetaData { Id = 44, TableNameId = 2,  ColumnOrder =4, Key = "isActive",          LabelEN = "Active",           LabelAR = "نشط",               EntityProperty = "IsActive",                                        FilterType = "boolean", DataType = "boolean", RenderAs = "badge" }
		);

		m.Entity<BarcodeType_s>().HasData(
			new BarcodeType_s { BarcodeTypeId = 1L,  Name = "EAN-13"      },
			new BarcodeType_s { BarcodeTypeId = 2L,  Name = "EAN-8"       },
			new BarcodeType_s { BarcodeTypeId = 3L,  Name = "UPC-A"       },
			new BarcodeType_s { BarcodeTypeId = 4L,  Name = "UPC-E"       },
			new BarcodeType_s { BarcodeTypeId = 5L,  Name = "Code 39"     },
			new BarcodeType_s { BarcodeTypeId = 6L,  Name = "Code 128"    },
			new BarcodeType_s { BarcodeTypeId = 7L,  Name = "ITF-14"      },
			new BarcodeType_s { BarcodeTypeId = 8L,  Name = "GS1-128"     },
			new BarcodeType_s { BarcodeTypeId = 9L,  Name = "QR Code"     },
			new BarcodeType_s { BarcodeTypeId = 10L, Name = "Data Matrix" },
			new BarcodeType_s { BarcodeTypeId = 11L, Name = "Custom"      }
		);

		m.Entity<InventoryValidationMethod_s>().HasData(
			new InventoryValidationMethod_s { InventoryValidationMethodId = 1L, Name = "FIFO"           },
			new InventoryValidationMethod_s { InventoryValidationMethodId = 2L, Name = "LIFO"           },
			new InventoryValidationMethod_s { InventoryValidationMethodId = 3L, Name = "Moving Average" }
		);

		m.Entity<WareHouseType_s>().HasData(
			new WareHouseType_s { Id = 1L, Name_EN = "Pharmaceutical Warehouses",                Name_AR = "مستودعات الأدوية"                              },
			new WareHouseType_s { Id = 2L, Name_EN = "Consumables & PPE Warehouses",             Name_AR = "مستودعات المستهلكات ومعدات الوقاية الشخصية"    },
			new WareHouseType_s { Id = 3L, Name_EN = "Medical Device & Equipment Warehouses",    Name_AR = "مستودعات الأجهزة والمعدات الطبية"               },
			new WareHouseType_s { Id = 4L, Name_EN = "Sterile Surgical Warehouses",             Name_AR = "مستودعات العمليات الجراحية المعقمة"             }
		);

		m.Entity<ItemType_s>().HasData(
			new ItemType_s { ItemTypeId = 1L, Name = "Stock Item",     IsActive = true },
			new ItemType_s { ItemTypeId = 2L, Name = "Service",        IsActive = true },
			new ItemType_s { ItemTypeId = 3L, Name = "Non-Stock Item", IsActive = true }
		);
	}
}
