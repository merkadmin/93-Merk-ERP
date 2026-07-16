namespace MerkERP.Core.Models;

public class Currency_s
{
	public long Id { get; set; }
	public string Code { get; set; } = string.Empty;
	public string Name_EN { get; set; } = string.Empty;
	public string? Name_AR { get; set; }
	public string? Symbol { get; set; }
	public bool IsActive { get; set; } = true;
}
