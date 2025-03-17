using FluentResults;
using MongoDB.Driver;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Errors;
using TaskLists.Infrastructure.Abstractions;

namespace TaskLists.Infrastructure.Repositories;

public abstract class Repository<TEntity>  : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly IDatabaseFacade DatabaseFacade;
    protected readonly IMongoCollection<TEntity> Collection;

    protected Repository(IDatabaseFacade databaseFacade)
    {
        DatabaseFacade = databaseFacade;
        Collection = databaseFacade.GetCollection<TEntity>();
    }

    public virtual async Task<Result> Create(TEntity entity)
    {
        await Collection.InsertOneAsync(entity);
        return Result.Ok();
    }

    public virtual async Task<TEntity?> Get(Guid id)
    {
        return await Collection.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public virtual async Task<PagedEntities<TEntity>> GetPaged(int pageNumber, int pageSize, bool isAscending = true)
    {
        var query = Collection.Find(Builders<TEntity>.Filter.Empty);
        
        var count = await query.CountDocumentsAsync();
        
        IReadOnlyList<TEntity> entities = await GetPagedItems(
            query,
            pageNumber,
            pageSize,
            isAscending);
        
        return new PagedEntities<TEntity>(entities, count);
    }
    
    protected async Task<IReadOnlyList<TEntity>> GetPagedItems(IFindFluent<TEntity, TEntity> query, int pageNumber, int pageSize, bool isAscending = true)
    {
        query = isAscending
            ? query.SortBy(x => x.Id)
            : query.SortByDescending(x => x.Id);

        query = query
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize);
        
        return await query.ToListAsync();
    }

    public virtual async Task<Result> Update(TEntity entity)
    {
        var result = await Collection.ReplaceOneAsync(t => t.Id == entity.Id, entity);
        return Result.FailIf(
            result.ModifiedCount == 0, 
            () => new NotFoundError("Entity was not found "));
    }

    public virtual async Task<Result> Delete(Guid id)
    {
        var result = await Collection.DeleteOneAsync(t => t.Id == id);
        return Result.FailIf(
            result.DeletedCount == 0, 
            () => new NotFoundError("Entity was not found "));
    }
}
