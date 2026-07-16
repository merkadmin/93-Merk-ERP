namespace MerkERP.Core.Models;

public class Company_cs
{
	public long Id { get; set; }
	public string? InternalCode { get; set; }
	public string Name_EN { get; set; } = string.Empty;
	public string? Name_AR { get; set; }
	public string Abbr { get; set; } = string.Empty;
	public long DefaultCurrencyId { get; set; }
	public string? Country { get; set; }
	public string? TaxId { get; set; }
	public string? Phone { get; set; }
	public string? Email { get; set; }
	public string? Website { get; set; }
	public string? Address { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public Currency_s? DefaultCurrency { get; set; }
}
