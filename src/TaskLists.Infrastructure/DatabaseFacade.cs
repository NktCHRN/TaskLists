using System.Collections.Frozen;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using TaskLists.Domain.Models;
using TaskLists.Infrastructure.Abstractions;

namespace TaskLists.Infrastructure;

public sealed class DatabaseFacade : IDatabaseFacade
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly ILogger<DatabaseFacade> _logger;
    public IMongoCollection<TaskList> TaskLists => GetCollection<TaskList>();
    public IMongoCollection<Domain.Models.Task> Tasks => GetCollection<Domain.Models.Task>();
    public IMongoCollection<User> Users => GetCollection<User>();

    private readonly IReadOnlyDictionary<Type, string> _collectionNames = new Dictionary<Type, string>()
    {
        {typeof(TaskList), "TaskLists"},
        {typeof(Domain.Models.Task), "Tasks"},
        {typeof(User), "Users"}
    };

    public DatabaseFacade(IMongoClient client, IConfiguration configuration, ILogger<DatabaseFacade> logger)
    {
        _client = client;
        _logger = logger;
        _database = _client.GetDatabase(configuration["DatabaseName"]);
    }
    
    public IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : class
    {
        return _database.GetCollection<TEntity>(_collectionNames[typeof(TEntity)]);
    }

    public async System.Threading.Tasks.Task SetupDatabase()
    {
        List<System.Threading.Tasks.Task> tasks = [];
        foreach (var collectionName in _collectionNames.Values)
        {
            tasks.Add(_database.CreateCollectionAsync(collectionName));
        }
        await System.Threading.Tasks.Task.WhenAll(tasks);
        
        var connectedUsersIndexModel = new CreateIndexModel<TaskList>(
            Builders<TaskList>.IndexKeys
                .Ascending(x => x.OwnerId)
                .Ascending(x => x.ConnectedUsersIds),
            new CreateIndexOptions(){Name = "OwnerIdAscConnectedUsersIdsAsc"});
        await TaskLists.Indexes.CreateOneAsync(connectedUsersIndexModel);

        var taskListIdIndexModel = new CreateIndexModel<Domain.Models.Task>(
            Builders<Domain.Models.Task>.IndexKeys
                .Ascending(x => x.TaskListId),
            new CreateIndexOptions(){Name  = "TaskListIdAsc"});
        await Tasks.Indexes.CreateOneAsync(taskListIdIndexModel);
    }

    public static void RegisterSettings()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        
        var defaultConventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };
        ConventionRegistry.Register("defaultConventions", defaultConventionPack, _ => true);
        
        BsonClassMap.RegisterClassMap<Domain.Models.Task>(classMap =>
        {
            classMap.AutoMap();
            classMap.MapCreator(t => new Domain.Models.Task(t.Id, t.Name, t.Description, t.DueDate, t.CreationDateTime, t.IsCompleted, t.TaskListId));
        });
        BsonClassMap.RegisterClassMap<TaskList>(classMap =>
        {
            classMap.AutoMap();
            classMap.MapMember(t => t.ConnectedUsersIds).SetElementName("ConnectedUsersIds");
            classMap.MapCreator(t => new TaskList(t.Id, t.Name, t.OwnerId, t.ConnectedUsersIds, t.CreationDateTime));
        });
        BsonClassMap.RegisterClassMap<User>(classMap =>
        {
            classMap.AutoMap();
            classMap.MapCreator(u => new User(u.Id, u.Name));
        });
    }
}
