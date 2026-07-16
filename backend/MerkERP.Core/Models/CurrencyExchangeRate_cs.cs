namespace MerkERP.Core.Models;

public class CurrencyExchangeRate_cs
{
	public long Id { get; set; }
	public long FromCurrencyId { get; set; }
	public long ToCurrencyId { get; set; }
	public decimal Rate { get; set; }
	public DateTime EffectiveDate { get; set; }
	public bool ForBuying { get; set; } = true;
	public bool ForSelling { get; set; } = true;
	public bool IsActive { get; set; } = true;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public Currency_s? FromCurrency { get; set; }
	public Currency_s? ToCurrency { get; set; }
}
