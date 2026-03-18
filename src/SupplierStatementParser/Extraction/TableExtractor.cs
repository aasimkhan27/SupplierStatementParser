using Amazon.Textract.Model;
using SupplierStatementParser.Helpers;
using SupplierStatementParser.Models;

namespace SupplierStatementParser.Extraction;

/// <summary>
/// Extracts structured tables from Textract blocks.
/// </summary>
public sealed class TableExtractor
{
    public IReadOnlyList<ExtractedTable> Extract(BlockIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);

        var tables = new List<ExtractedTable>();
        foreach (var tableBlock in index.GetByType(BlockType.TABLE))
        {
            var cells = index.GetRelatedBlocks(tableBlock, "CHILD")
                .Where(b => b.BlockType == BlockType.CELL)
                .OrderBy(c => c.RowIndex)
                .ThenBy(c => c.ColumnIndex)
                .ToList();

            if (cells.Count == 0)
            {
                continue;
            }

            var rows = cells.GroupBy(c => c.RowIndex ?? 0).OrderBy(g => g.Key).ToList();
            var headerRow = rows.First();
            var headers = headerRow
                .OrderBy(c => c.ColumnIndex)
                .Select(c => TextReconstructionHelper.GetText(c, index) ?? $"Column{c.ColumnIndex}")
                .ToArray();

            var dataRows = rows.Skip(1)
                .Select(row => new ExtractedTableRow
                {
                    RowIndex = row.Key,
                    Values = row.ToDictionary(
                        c => headers.ElementAtOrDefault((c.ColumnIndex ?? 1) - 1) ?? $"Column{c.ColumnIndex}",
                        c => new RawFieldValue(
                            headers.ElementAtOrDefault((c.ColumnIndex ?? 1) - 1) ?? $"Column{c.ColumnIndex}",
                            TextReconstructionHelper.GetText(c, index),
                            c.Confidence,
                            "TABLES",
                            c.Id),
                        StringComparer.OrdinalIgnoreCase)
                })
                .ToList();

            tables.Add(new ExtractedTable
            {
                TableId = tableBlock.Id,
                Headers = headers,
                Rows = dataRows
            });
        }

        return tables;
    }
}
