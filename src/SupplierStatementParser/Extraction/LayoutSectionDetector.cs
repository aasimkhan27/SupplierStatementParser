using Amazon.Textract.Model;
using SupplierStatementParser.Helpers;

namespace SupplierStatementParser.Extraction;

/// <summary>
/// Provides lightweight section detection using Textract layout blocks.
/// </summary>
public sealed class LayoutSectionDetector
{
    public IReadOnlyList<string> DetectSections(BlockIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);

        return index.GetByType(BlockType.LAYOUT_SECTION_HEADER)
            .Select(block => TextReconstructionHelper.GetText(block, index))
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
