using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;

namespace MerkERP.DAL.Context;

public class MerkDbContext : DbContext
{
	public MerkDbContext(DbContextOptions<MerkDbContext> options) : base(options) { }

	public DbSet<TableName_s> TableName_s { get; set; }
	public DbSet<TableMetaData> TableMetaData { get; set; }
	public DbSet<TableMetaData_FilterType_s> TableMetaData_FilterType_s { get; set; }
	public DbSet<TableMetaData_DataType_s> TableMetaData_DataType_s { get; set; }
	public DbSet<TableMetaData_RenderAs_s> TableMetaData_RenderAs_s { get; set; }
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
	public DbSet<StockTransactionType_s> StockTransactionType_s { get; set; }
	public DbSet<StockTransactionStatus_s> StockTransactionStatus_s { get; set; }
	public DbSet<StockReconciliationTransaction> StockReconciliationTransaction { get; set; }
	public DbSet<StockReconciliationTransactionDetail> StockReconciliationTransactionDetail { get; set; }
	public DbSet<StockReconciliationTransactionDate> StockReconciliationTransactionDate { get; set; }
	public DbSet<StockLedgerTransaction> StockLedgerTransaction { get; set; }
	public DbSet<UserType_s> UserType_s { get; set; }
	public DbSet<User_cs> User_cs { get; set; }

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
		m.Entity<TableMetaData>().HasOne(e => e.FilterType).WithMany().HasForeignKey(e => e.FilterTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<TableMetaData>().HasOne(e => e.DataType).WithMany().HasForeignKey(e => e.DataTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<TableMetaData>().HasOne(e => e.RenderAs).WithMany().HasForeignKey(e => e.RenderAsId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<TableMetaData_FilterType_s>().HasKey(e => e.Id);
		m.Entity<TableMetaData_FilterType_s>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<TableMetaData_FilterType_s>().Property(e => e.Name).HasColumnType("nvarchar(50)").IsRequired();

		m.Entity<TableMetaData_DataType_s>().HasKey(e => e.Id);
		m.Entity<TableMetaData_DataType_s>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<TableMetaData_DataType_s>().Property(e => e.Name).HasColumnType("nvarchar(50)").IsRequired();

		m.Entity<TableMetaData_RenderAs_s>().HasKey(e => e.Id);
		m.Entity<TableMetaData_RenderAs_s>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<TableMetaData_RenderAs_s>().Property(e => e.Name).HasColumnType("nvarchar(50)").IsRequired();
		m.Entity<Branch_cs>().HasKey(e => e.Id);
		m.Entity<WareHouseCategory_cs>().HasKey(e => e.Id);
		m.Entity<WareHouseCategory_cs>()
			.HasOne(c => c.WareHouseType).WithMany().HasForeignKey(c => c.WareHouseTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<WareHouseType_s>().HasKey(e => e.Id);
		m.Entity<UserType_s>().HasKey(e => e.Id);
		m.Entity<User_cs>().HasKey(e => e.Id);
		m.Entity<User_cs>()
			.HasOne(u => u.UserType).WithMany().HasForeignKey(u => u.UserTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<User_cs>()
			.HasOne<User_cs>().WithMany().HasForeignKey(u => u.InsertedBy).OnDelete(DeleteBehavior.NoAction);
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

		// ── Stock tables ──────────────────────────────────────────────────────
		m.Entity<StockTransactionType_s>().HasKey(e => e.Id);
		m.Entity<StockTransactionType_s>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<StockTransactionType_s>().Property(e => e.Name_EN).HasColumnType("nvarchar(200)").IsRequired();
		m.Entity<StockTransactionType_s>().Property(e => e.Name_AR).HasColumnType("nvarchar(200)").IsRequired();

		m.Entity<StockTransactionStatus_s>().HasKey(e => e.Id);
		m.Entity<StockTransactionStatus_s>().Property(e => e.Id).ValueGeneratedNever();
		m.Entity<StockTransactionStatus_s>().Property(e => e.Name_EN).HasColumnType("nvarchar(200)").IsRequired();
		m.Entity<StockTransactionStatus_s>().Property(e => e.Name_AR).HasColumnType("nvarchar(200)").IsRequired();

		m.Entity<StockReconciliationTransaction>().HasKey(e => e.Id);
		m.Entity<StockReconciliationTransaction>().Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<StockReconciliationTransaction>()
			.HasOne(t => t.StockTransactionType).WithMany().HasForeignKey(t => t.StockTransactionTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockReconciliationTransaction>()
			.HasOne(t => t.StockTransactionStatus).WithMany().HasForeignKey(t => t.StockTransactionStatusId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockReconciliationTransaction>()
			.HasOne(t => t.SetWarehouse).WithMany().HasForeignKey(t => t.SetWarehouseId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockReconciliationTransaction>()
			.HasMany(t => t.Details).WithOne(d => d.StockReconciliationTransaction).HasForeignKey(d => d.StockReconciliationTransactionId).OnDelete(DeleteBehavior.Cascade);

		m.Entity<StockReconciliationTransactionDetail>().HasKey(e => e.Id);
		m.Entity<StockReconciliationTransactionDetail>().Property(e => e.Quantity).HasColumnType("decimal(18,4)");
		m.Entity<StockReconciliationTransactionDetail>()
			.HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockReconciliationTransactionDetail>()
			.HasOne(d => d.Warehouse).WithMany().HasForeignKey(d => d.WarehouseId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockReconciliationTransactionDetail>()
			.HasOne(d => d.UOM).WithMany().HasForeignKey(d => d.UOMId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<StockReconciliationTransactionDate>().HasKey(e => e.Id);
		m.Entity<StockReconciliationTransactionDate>()
			.HasOne(d => d.StockReconciliationTransaction).WithMany().HasForeignKey(d => d.StockReconciliationTransactionId).OnDelete(DeleteBehavior.Cascade);
		m.Entity<StockReconciliationTransactionDate>()
			.HasOne(d => d.StockTransactionStatus).WithMany().HasForeignKey(d => d.StockTransactionStatusId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockReconciliationTransactionDate>()
			.HasOne(d => d.StockTransactionType).WithMany().HasForeignKey(d => d.StockTransactionTypeId).OnDelete(DeleteBehavior.Restrict);

		m.Entity<StockLedgerTransaction>().HasKey(e => e.Id);
		m.Entity<StockLedgerTransaction>().Property(e => e.InternalCode).HasColumnType("nvarchar(50)");
		m.Entity<StockLedgerTransaction>().Property(e => e.ActualQuantity).HasColumnType("decimal(18,4)");
		m.Entity<StockLedgerTransaction>().Property(e => e.QuantityAfterTransaction).HasColumnType("decimal(18,4)");
		m.Entity<StockLedgerTransaction>().Property(e => e.ValuationRate).HasColumnType("decimal(18,4)");
		m.Entity<StockLedgerTransaction>()
			.HasOne(l => l.StockTransactionType).WithMany().HasForeignKey(l => l.StockTransactionTypeId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockLedgerTransaction>()
			.HasOne(l => l.Item).WithMany().HasForeignKey(l => l.ItemId).OnDelete(DeleteBehavior.Restrict);
		m.Entity<StockLedgerTransaction>()
			.HasOne(l => l.WareHouse).WithMany().HasForeignKey(l => l.WareHouseId).OnDelete(DeleteBehavior.Restrict);

		// InsertedBy → User_cs (nullable FK, set null on user delete)
		m.Entity<Branch_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<WareHouseCategory_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<WareHouse_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<UOM_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<UOMConversionGroup_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<UOMConversionFactor_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<ItemGroup_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<Item_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<ItemType_s>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<ItemUOMConversion_cs>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<StockReconciliationTransaction>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<StockReconciliationTransactionDetail>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<StockReconciliationTransactionDate>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);
		m.Entity<StockLedgerTransaction>().HasOne<User_cs>().WithMany().HasForeignKey(e => e.InsertedBy).OnDelete(DeleteBehavior.SetNull);

		m.Entity<ItemUOMConversion_cs>()
			.Property(e => e.ConversionFactor).HasColumnType("decimal(18,2)");

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

		m.Entity<UserType_s>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<UserType_s>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");
		m.Entity<User_cs>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<User_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");
		m.Entity<User_cs>()
			.Property(e => e.Login).HasColumnType("nvarchar(100)");
		m.Entity<User_cs>()
			.Property(e => e.Password).HasColumnType("nvarchar(500)");
		m.Entity<User_cs>()
			.Property(e => e.Email).HasColumnType("nvarchar(200)");
		m.Entity<User_cs>()
			.Property(e => e.Description).HasColumnType("nvarchar(max)");

		m.Entity<UserType_s>().HasData(
			new UserType_s { Id = 1, Name_EN = "Root", Name_AR = "روت" },
			new UserType_s { Id = 2, Name_EN = "Merk Admin", Name_AR = "مدير ميرك" },
			new UserType_s { Id = 3, Name_EN = "Admin", Name_AR = "مدير" },
			new UserType_s { Id = 4, Name_EN = "Regular User", Name_AR = "مستخدم عادي" }
		);

		m.Entity<User_cs>().HasData(
			new User_cs { Id = 1, Name_EN = "Admin", Name_AR = "مدير", Login = "admin", Password = "admin", UserTypeId = 2, IsActive = true }
		);

		m.Entity<TableName_s>().HasData(
			new TableName_s { Id = 1, Name = "BarcodeType_s", EntityKey = null },
			new TableName_s { Id = 2, Name = "Branch_cs", EntityKey = "branches" },
			new TableName_s { Id = 3, Name = "InventoryValidationMethod_s", EntityKey = null },
			new TableName_s { Id = 4, Name = "Item_cs", EntityKey = "items" },
			new TableName_s { Id = 5, Name = "Item_UOM_Barcode_cs", EntityKey = null },
			new TableName_s { Id = 6, Name = "ItemGroup_cs", EntityKey = "itemgroups" },
			new TableName_s { Id = 7, Name = "ItemType_s", EntityKey = null },
			new TableName_s { Id = 8, Name = "ItemUOMConversion_cs", EntityKey = null },
			new TableName_s { Id = 10, Name = "TableName_s", EntityKey = null },
			new TableName_s { Id = 11, Name = "UOM_cs", EntityKey = "uoms" },
			new TableName_s { Id = 12, Name = "UOMConversionFactor_cs", EntityKey = "uomconversionfactors" },
			new TableName_s { Id = 13, Name = "UOMConversionGroup_cs", EntityKey = "uomconversiongroups" },
			new TableName_s { Id = 14, Name = "WareHouse_cs", EntityKey = "warehouses" },
			new TableName_s { Id = 15, Name = "WareHouseCategory_cs", EntityKey = "warehousecategories" },
			new TableName_s { Id = 16, Name = "WareHouseType_s", EntityKey = null },
			new TableName_s { Id = 18, Name = "TableMetaData", EntityKey = null },
			new TableName_s { Id = 19, Name = "UserType_s", EntityKey = null },
			new TableName_s { Id = 20, Name = "User_cs", EntityKey = null },
			new TableName_s { Id = 21, Name = "StockTransactionType_s", EntityKey = null },
			new TableName_s { Id = 22, Name = "StockReconciliationTransaction", EntityKey = null },
			new TableName_s { Id = 23, Name = "StockReconciliationTransactionDetail", EntityKey = null },
			new TableName_s { Id = 24, Name = "StockLedgerTransaction", EntityKey = null },
			new TableName_s { Id = 25, Name = "StockTransactionStatus_s", EntityKey = null }
		);

		m.Entity<BarcodeType_s>().HasData(
			new BarcodeType_s { BarcodeTypeId = 1L, Name = "EAN-13" },
			new BarcodeType_s { BarcodeTypeId = 2L, Name = "EAN-8" },
			new BarcodeType_s { BarcodeTypeId = 3L, Name = "UPC-A" },
			new BarcodeType_s { BarcodeTypeId = 4L, Name = "UPC-E" },
			new BarcodeType_s { BarcodeTypeId = 5L, Name = "Code 39" },
			new BarcodeType_s { BarcodeTypeId = 6L, Name = "Code 128" },
			new BarcodeType_s { BarcodeTypeId = 7L, Name = "ITF-14" },
			new BarcodeType_s { BarcodeTypeId = 8L, Name = "GS1-128" },
			new BarcodeType_s { BarcodeTypeId = 9L, Name = "QR Code" },
			new BarcodeType_s { BarcodeTypeId = 10L, Name = "Data Matrix" },
			new BarcodeType_s { BarcodeTypeId = 11L, Name = "Custom" }
		);

		m.Entity<InventoryValidationMethod_s>().HasData(
			new InventoryValidationMethod_s { InventoryValidationMethodId = 1L, Name = "FIFO" },
			new InventoryValidationMethod_s { InventoryValidationMethodId = 2L, Name = "LIFO" },
			new InventoryValidationMethod_s { InventoryValidationMethodId = 3L, Name = "Moving Average" }
		);

		m.Entity<WareHouseType_s>().HasData(
			new WareHouseType_s { Id = 1L, Name_EN = "Pharmaceutical Warehouses", Name_AR = "مستودعات الأدوية" },
			new WareHouseType_s { Id = 2L, Name_EN = "Consumables & PPE Warehouses", Name_AR = "مستودعات المستهلكات ومعدات الوقاية الشخصية" },
			new WareHouseType_s { Id = 3L, Name_EN = "Medical Device & Equipment Warehouses", Name_AR = "مستودعات الأجهزة والمعدات الطبية" },
			new WareHouseType_s { Id = 4L, Name_EN = "Sterile Surgical Warehouses", Name_AR = "مستودعات العمليات الجراحية المعقمة" }
		);

		m.Entity<ItemType_s>().HasData(
			new ItemType_s { ItemTypeId = 1L, Name = "Stock Item", IsActive = true },
			new ItemType_s { ItemTypeId = 2L, Name = "Service", IsActive = true },
			new ItemType_s { ItemTypeId = 3L, Name = "Non-Stock Item", IsActive = true }
		);

		m.Entity<StockTransactionType_s>().HasData(
			new StockTransactionType_s { Id = 1L, Name_EN = "Opening Balance", Name_AR = "رصيد افتتاحي" },
			new StockTransactionType_s { Id = 2L, Name_EN = "Stock Reconciliation", Name_AR = "جرد المخزون" },
			new StockTransactionType_s { Id = 3L, Name_EN = "Stock Receipt", Name_AR = "استلام مخزون" },
			new StockTransactionType_s { Id = 4L, Name_EN = "Stock Issue", Name_AR = "صرف مخزون" },
			new StockTransactionType_s { Id = 5L, Name_EN = "Stock Transfer", Name_AR = "تحويل مخزون" }
		);

		m.Entity<StockTransactionStatus_s>().HasData(
			new StockTransactionStatus_s { Id = 1L, Name_EN = "Draft", Name_AR = "مسودة" },
			new StockTransactionStatus_s { Id = 2L, Name_EN = "Pending", Name_AR = "قيد الانتظار" },
			new StockTransactionStatus_s { Id = 3L, Name_EN = "Submitted", Name_AR = "مُقدَّم" },
			new StockTransactionStatus_s { Id = 4L, Name_EN = "Cancelled", Name_AR = "ملغى" },
			new StockTransactionStatus_s { Id = 5L, Name_EN = "Amended", Name_AR = "معدَّل" }
		);
	}
}
