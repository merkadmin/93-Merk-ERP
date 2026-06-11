using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;

namespace MerkERP.DAL.Context;

public class MerkDbContext : DbContext
{
	public MerkDbContext(DbContextOptions<MerkDbContext> options) : base(options) { }

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
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<WareHouseCategory_cs>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");
		m.Entity<WareHouseCategory_cs>()
			.Property(e => e.Description).HasColumnType("nvarchar(max)");

		m.Entity<WareHouseType_s>()
			.Property(e => e.Name_EN).HasColumnType("nvarchar(200)");
		m.Entity<WareHouseType_s>()
			.Property(e => e.Name_AR).HasColumnType("nvarchar(200)");

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
