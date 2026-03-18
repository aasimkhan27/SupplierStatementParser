using Amazon.Textract.Model;

namespace SupplierStatementParser.Helpers;

/// <summary>
/// Provides efficient lookup helpers over Textract blocks.
/// </summary>
public sealed class BlockIndex
{
    private readonly Dictionary<string, Block> _byId;
    private readonly Dictionary<BlockType, List<Block>> _byType;

    public BlockIndex(IReadOnlyCollection<Block> blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        Blocks = blocks.ToArray();
        _byId = Blocks.Where(b => !string.IsNullOrWhiteSpace(b.Id)).ToDictionary(b => b.Id, StringComparer.Ordinal);
        _byType = Blocks.GroupBy(b => b.BlockType).ToDictionary(g => g.Key, g => g.ToList());
    }

    public IReadOnlyList<Block> Blocks { get; }

    public Block? GetById(string? id)
        => !string.IsNullOrWhiteSpace(id) && _byId.TryGetValue(id, out var block) ? block : null;

    public IReadOnlyList<Block> GetByType(BlockType type)
        => _byType.TryGetValue(type, out var blocks) ? blocks : Array.Empty<Block>();

    public IEnumerable<Block> GetRelatedBlocks(Block? block, params string[] relationshipTypes)
    {
        if (block?.Relationships is null || relationshipTypes.Length == 0)
        {
            yield break;
        }

        foreach (var relationship in block.Relationships.Where(r => relationshipTypes.Contains(r.Type?.Value, StringComparer.OrdinalIgnoreCase)))
        {
            if (relationship.Ids is null)
            {
                continue;
            }

            foreach (var id in relationship.Ids)
            {
                var related = GetById(id);
                if (related is not null)
                {
                    yield return related;
                }
            }
        }
    }
}
