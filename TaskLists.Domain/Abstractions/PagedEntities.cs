namespace TaskLists.Domain.Abstractions;

public sealed class PagedEntities<TEntity>
{
    public IReadOnlyList<TEntity> Items { get; }
    public long Count { get; }

    public PagedEntities(IReadOnlyList<TEntity> items, long count)
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = items;
        Count = count;
    }
}
