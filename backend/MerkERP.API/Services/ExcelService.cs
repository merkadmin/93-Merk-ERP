using ClosedXML.Excel;

namespace MerkERP.API.Services;

/// <summary>
/// Describes a read-only reference sheet appended to a template workbook.
/// </summary>
public record ReferenceSheet(string Name, string[] Headers, IEnumerable<string[]> Rows);

/// <summary>
/// Shared Excel helper used by all controllers that support template export / data import.
/// </summary>
public class ExcelService
{
	private static readonly XLColor HeaderBg   = XLColor.FromHtml("#1E3A5F");
	private static readonly XLColor HeaderFg   = XLColor.White;

	// ── Export ───────────────────────────────────────────────────────────────

	/// <summary>
	/// Builds a styled .xlsx template and returns its bytes.
	/// </summary>
	/// <param name="columns">Tuples of (header label, column width).</param>
	/// <param name="referenceSheets">Optional read-only reference sheets for user convenience.</param>
	public byte[] BuildTemplate(
		IEnumerable<(string Label, int Width)> columns,
		IEnumerable<ReferenceSheet>? referenceSheets = null)
	{
		using var wb   = new XLWorkbook();
		var cols       = columns.ToList();

		// ── Data-entry sheet ─────────────────────────────────────────────────
		var ws = wb.AddWorksheet("Template");
		for (int i = 0; i < cols.Count; i++)
		{
			ws.Cell(1, i + 1).Value = cols[i].Label;
			ws.Column(i + 1).Width  = cols[i].Width;
		}
		ApplyHeaderStyle(ws.Range(1, 1, 1, cols.Count));

		// ── Reference sheets ─────────────────────────────────────────────────
		if (referenceSheets != null)
		{
			foreach (var refSheet in referenceSheets)
			{
				var wsRef = wb.AddWorksheet(refSheet.Name);
				for (int c = 0; c < refSheet.Headers.Length; c++)
					wsRef.Cell(1, c + 1).Value = refSheet.Headers[c];
				ApplyHeaderStyle(wsRef.Range(1, 1, 1, refSheet.Headers.Length));
				wsRef.Columns(1, refSheet.Headers.Length).ForEach(col => col.Width = 28);

				int row = 2;
				foreach (var r in refSheet.Rows)
				{
					for (int c = 0; c < r.Length; c++)
						wsRef.Cell(row, c + 1).Value = r[c];
					row++;
				}
			}
		}

		using var ms = new MemoryStream();
		wb.SaveAs(ms);
		return ms.ToArray();
	}

	// ── Import ────────────────────────────────────────────────────────────────

	/// <summary>
	/// Reads all data rows from the first worksheet of an uploaded .xlsx file.
	/// Row 1 (header) is skipped automatically. Empty trailing rows are ignored.
	/// Each element of the returned array corresponds to one cell value (trimmed string).
	/// </summary>
	public IReadOnlyList<string[]> ReadRows(Stream stream)
	{
		using var wb = new XLWorkbook(stream);
		var ws       = wb.Worksheet(1);
		var lastRow  = ws.LastRowUsed()?.RowNumber() ?? 1;
		var lastCol  = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

		var result = new List<string[]>();
		for (int row = 2; row <= lastRow; row++)
		{
			var cells = new string[lastCol];
			for (int col = 1; col <= lastCol; col++)
				cells[col - 1] = ws.Cell(row, col).GetValue<string>().Trim();
			result.Add(cells);
		}
		return result;
	}

	// ── Helpers ──────────────────────────────────────────────────────────────

	private static void ApplyHeaderStyle(IXLRange range)
	{
		range.Style.Font.Bold            = true;
		range.Style.Fill.BackgroundColor = HeaderBg;
		range.Style.Font.FontColor       = HeaderFg;
	}
}
