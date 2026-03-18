using Amazon.Textract.Model;

namespace SupplierStatementParser.Helpers;

/// <summary>
/// Rebuilds readable text from Textract blocks.
/// </summary>
public static class TextReconstructionHelper
{
    public static string? GetText(Block? block, BlockIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);

        if (block is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(block.Text))
        {
            return NormalizeWhitespace(block.Text);
        }

        var childWords = index
            .GetRelatedBlocks(block, "CHILD")
            .Where(b => b.BlockType == BlockType.WORD || b.BlockType == BlockType.SELECTION_ELEMENT)
            .Select(GetLeafText)
            .Where(t => !string.IsNullOrWhiteSpace(t));

        var combined = string.Join(' ', childWords);
        return NormalizeWhitespace(combined);
    }

    private static string? GetLeafText(Block block)
        => block.BlockType == BlockType.SELECTION_ELEMENT
            ? string.Equals(block.SelectionStatus?.Value, "SELECTED", StringComparison.OrdinalIgnoreCase) ? "[X]" : "[ ]"
            : block.Text;

    public static string? NormalizeWhitespace(string? input)
        => string.IsNullOrWhiteSpace(input)
            ? null
            : string.Join(' ', input.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
