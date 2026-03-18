using Amazon.Textract.Model;
using SupplierStatementParser.Abstractions;
using SupplierStatementParser.Configuration;
using SupplierStatementParser.Extraction;
using SupplierStatementParser.Helpers;
using SupplierStatementParser.Mapping;
using SupplierStatementParser.Models;

namespace SupplierStatementParser;

/// <summary>
/// Production-oriented parser for AWS Textract supplier statement documents.
/// </summary>
public sealed class SupplierStatementParser : ISupplierStatementParser
{
    private readonly KeyValueExtractor _keyValueExtractor = new();
    private readonly TableExtractor _tableExtractor = new();
    private readonly QueryExtractor _queryExtractor = new();
    private readonly LayoutSectionDetector _layoutSectionDetector = new();
    private readonly SupplierStatementMapper _mapper = new();

    /// <inheritdoc />
    public SupplierStatementParseResult Parse(IReadOnlyCollection<Block> blocks, SupplierStatementParserOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        options ??= new SupplierStatementParserOptions();

        var issues = new List<ParseIssue>();
        var index = new BlockIndex(blocks);
        var formFields = _keyValueExtractor.Extract(index);
        var queryFields = _queryExtractor.Extract(index);
        var tables = _tableExtractor.Extract(index);
        var sections = _layoutSectionDetector.DetectSections(index);
        var data = _mapper.Map(formFields, queryFields, tables, options, issues);

        if (sections.Count > 0)
        {
            issues.Add(new ParseIssue("layout.sections_detected", $"Detected layout sections: {string.Join(", ", sections)}.", ParseSeverity.Info, "LAYOUT"));
        }

        var rawFields = formFields.Values.Concat(queryFields.Values)
            .ToDictionary(v => v.Name, v => v, StringComparer.OrdinalIgnoreCase);

        return new SupplierStatementParseResult
        {
            Data = data,
            Issues = issues,
            RawFields = rawFields,
            SourceBlocks = blocks.ToArray()
        };
    }
}
