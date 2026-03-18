using Amazon.Textract.Model;

namespace SupplierStatementParser.Models;

/// <summary>
/// Encapsulates parsed statement data, source fields, and issues.
/// </summary>
public sealed class SupplierStatementParseResult
{
    public SupplierStatementData Data { get; init; } = new();

    public IReadOnlyList<ParseIssue> Issues { get; init; } = Array.Empty<ParseIssue>();

    public IReadOnlyDictionary<string, RawFieldValue> RawFields { get; init; } = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<Block> SourceBlocks { get; init; } = Array.Empty<Block>();

    public bool HasErrors => Issues.Any(i => i.Severity == ParseSeverity.Error);
}
