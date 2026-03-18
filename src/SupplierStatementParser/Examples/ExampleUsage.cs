using Amazon.Textract.Model;
using SupplierStatementParser.Configuration;
using SupplierStatementParser.Models;

namespace SupplierStatementParser.Examples;

/// <summary>
/// Demonstrates how to use the parser with Textract blocks.
/// </summary>
public static class ExampleUsage
{
    /// <summary>
    /// Parses a supplier statement using optional alias customization.
    /// </summary>
    public static SupplierStatementParseResult ParseStatement(IReadOnlyCollection<Block> textractBlocks)
    {
        ArgumentNullException.ThrowIfNull(textractBlocks);

        var options = new SupplierStatementParserOptions()
            .AddAliases(SupplierStatementField.SupplierAccountNumber, "supplier id", "creditor account")
            .AddAliases(SupplierStatementField.StatementNumber, "statement id");

        var parser = new SupplierStatementParser();
        return parser.Parse(textractBlocks, options);
    }
}
