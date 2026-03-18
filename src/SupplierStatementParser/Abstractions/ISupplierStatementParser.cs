using Amazon.Textract.Model;
using SupplierStatementParser.Configuration;
using SupplierStatementParser.Models;

namespace SupplierStatementParser.Abstractions;

/// <summary>
/// Parses AWS Textract blocks into a supplier statement model.
/// </summary>
public interface ISupplierStatementParser
{
    /// <summary>
    /// Parses a Textract block collection.
    /// </summary>
    SupplierStatementParseResult Parse(IReadOnlyCollection<Block> blocks, SupplierStatementParserOptions? options = null);
}
