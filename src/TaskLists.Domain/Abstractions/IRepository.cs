namespace TaskLists.Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<Result> Create(TEntity entity);
    Task<TEntity?> Get(Guid id);
    Task<PagedEntities<TEntity>> GetPaged(int pageNumber, int pageSize, bool isAscending = true);
    Task<Result> Update(TEntity entity);
    Task<Result> Delete(Guid id);
}
