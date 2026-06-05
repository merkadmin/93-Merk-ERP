using Microsoft.EntityFrameworkCore;
using MerkERP.Core.DTOs;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.DAL.Repositories;

public class StockService(MerkDbContext db)
{
	public async Task<StockLedgerTransaction> PostMovementAsync(StockMovementDto dto)
	{
		var bin = await db.WarehouseTransaction
			.FirstOrDefaultAsync(b => b.ItemId == dto.ItemId && b.WarehouseId == dto.WarehouseId);

		if (bin is null)
		{
			bin = new WarehouseTransaction { ItemId = dto.ItemId, WarehouseId = dto.WarehouseId };
			db.WarehouseTransaction.Add(bin);
		}

		var qtyBefore = bin.ActualQty;
		bin.ActualQty += dto.Qty;

		if (dto.Qty > 0 && dto.ValuationRate > 0)
		{
			var totalValue = (qtyBefore * bin.ValuationRate) + (dto.Qty * dto.ValuationRate);
			bin.ValuationRate = bin.ActualQty > 0 ? totalValue / bin.ActualQty : dto.ValuationRate;
		}

		var slt = new StockLedgerTransaction
		{
			ItemId = dto.ItemId,
			WarehouseId = dto.WarehouseId,
			PostingDate = dto.PostingDate,
			PostingTime = TimeOnly.FromDateTime(DateTime.Now),
			ActualQty = dto.Qty,
			QtyAfterTransaction = bin.ActualQty,
			ValuationRate = dto.ValuationRate > 0 ? dto.ValuationRate : bin.ValuationRate,
			StockValue = bin.ActualQty * bin.ValuationRate,
			VoucherType = dto.VoucherType,
			VoucherNo = dto.VoucherNo,
			BatchNo = dto.BatchNo,
		};

		db.StockLedgerTransaction.Add(slt);
		await db.SaveChangesAsync();
		return slt;
	}
}
