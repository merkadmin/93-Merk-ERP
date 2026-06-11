using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkERP.Core.Models.Enums
{
	public enum BarcodeTypeEnum
	{
		None = 0,
		EAN13 = 1,
		EAN8 = 2,
		UPCA = 3,
		UPCE = 4,
		Code39 = 5,
		Code128 = 6,
		ITF14 = 7,
		GS1128 = 8,
		QRCode = 9,
		DataMatrix = 10,
		Custom = 11,
	}

	public enum InventoryValidationMethodEnum
	{
		None = 0,
		FIFO = 1,
		LIFO = 2,
		MovingAverage = 3,
	}

	public enum WareHouseTypeEnum
	{
		None = 0,
		PharmaceuticalWarehouses = 1,
		ConsumablesAndPPEWarehouses = 2,
		MedicalDeviceAndEquipmentWarehouses = 3,
		SterileSurgicalWarehouses = 4,
	}

	/// <summary>
	/// Maps every physical EF table to a stable integer Id.
	/// The integer values are mirrored in the TableName_s seed data — keep them in sync.
	/// When a new table is added: add an entry here, add it to HasData in MerkDbContext,
	/// then create a migration. The TableRegistryService will catch any table not yet seeded.
	/// </summary>
	public enum TableNameEnum
	{
		None = 0,
		BarcodeType_s               = 1,
		Branch_cs                   = 2,
		InventoryValidationMethod_s = 3,
		Item_cs                     = 4,
		Item_UOM_Barcode_cs         = 5,
		ItemGroup_cs                = 6,
		ItemType_s                  = 7,
		ItemUOMConversion_cs        = 8,
		StockLedgerTransaction      = 9,
		TableName_s                 = 10,
		UOM_cs                      = 11,
		UOMConversionFactor_cs      = 12,
		UOMConversionGroup_cs       = 13,
		WareHouse_cs                = 14,
		WareHouseCategory_cs        = 15,
		WareHouseType_s             = 16,
		WarehouseTransaction        = 17,
		TableMetaData               = 18,
	}
}
