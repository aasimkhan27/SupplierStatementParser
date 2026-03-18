using Amazon.Textract.Model;
using SupplierStatementParser.Helpers;
using SupplierStatementParser.Models;

namespace SupplierStatementParser.Extraction;

/// <summary>
/// Extracts query answers from Textract QUERY and QUERY_RESULT blocks.
/// </summary>
public sealed class QueryExtractor
{
    public IReadOnlyDictionary<string, RawFieldValue> Extract(BlockIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);

        var results = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);
        foreach (var queryBlock in index.GetByType(BlockType.QUERY))
        {
            var query = queryBlock.Query;
            if (query is null)
            {
                continue;
            }

            var key = query.Alias ?? query.Text;
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var answer = index.GetRelatedBlocks(queryBlock, "ANSWER").FirstOrDefault();
            var answerText = TextReconstructionHelper.GetText(answer, index);
            results[key] = new RawFieldValue(key, answerText, answer?.Confidence ?? queryBlock.Confidence, "QUERIES", answer?.Id ?? queryBlock.Id, query.Alias);
        }

        return results;
    }
}
