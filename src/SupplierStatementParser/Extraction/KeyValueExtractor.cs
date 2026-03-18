using Amazon.Textract.Model;
using SupplierStatementParser.Helpers;
using SupplierStatementParser.Models;

namespace SupplierStatementParser.Extraction;

/// <summary>
/// Extracts key-value pairs from Textract form blocks.
/// </summary>
public sealed class KeyValueExtractor
{
    public IReadOnlyDictionary<string, RawFieldValue> Extract(BlockIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);

        var results = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);
        var keyBlocks = index.GetByType(BlockType.KEY_VALUE_SET)
            .Where(b => string.Equals(b.EntityTypes?.FirstOrDefault()?.Value, "KEY", StringComparison.OrdinalIgnoreCase));

        foreach (var keyBlock in keyBlocks)
        {
            var key = TextReconstructionHelper.GetText(keyBlock, index);
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var valueBlock = index.GetRelatedBlocks(keyBlock, "VALUE").FirstOrDefault();
            var valueText = TextReconstructionHelper.GetText(valueBlock, index);
            results[key] = new RawFieldValue(key, valueText, valueBlock?.Confidence ?? keyBlock.Confidence, "FORMS", valueBlock?.Id ?? keyBlock.Id);
        }

        return results;
    }
}
