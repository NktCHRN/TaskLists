using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TaskLists.Domain.Models;
using TaskLists.Infrastructure.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace TaskLists.Infrastructure;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IDatabaseFacade _databaseFacade;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IDatabaseFacade databaseFacade, ILogger<DatabaseSeeder> logger)
    {
        _databaseFacade = databaseFacade;
        _logger = logger;
    }

    public async Task Seed()
    {
        List<System.Threading.Tasks.Task> threadTasks = new List<System.Threading.Tasks.Task>();
        
        _logger.LogInformation("Seeding database started {DateTime}.", DateTimeOffset.UtcNow);
        var users = new List<User>
        {
            new User(Guid.Parse("0195a49a-0b22-70ae-a522-518da9ebadb9"), "John"),
            new User(Guid.Parse("0195a49d-7c46-79bc-b5e7-5704f7b97ee3"), "Mary"),
            new User(Guid.Parse("0195a49d-a733-7b00-84f8-89cac50eabe6"), "Bob"),
        };
        foreach (var user in users)
        {
            threadTasks.Add(_databaseFacade.Users.ReplaceOneAsync(
                MongoDB.Driver.Builders<User>.Filter.Eq(u => u.Id, user.Id), user,
                new ReplaceOptions { IsUpsert = true }));
        }
        await Task.WhenAll(threadTasks);
        threadTasks.Clear();
        _logger.LogInformation("Users seeded successfully.");

        var taskLists = new List<TaskList>()
        {
            new TaskList(
                Guid.Parse("0195a49f-3fdf-72b4-bada-83f60e5c7881"), 
                "J List 1", 
                Guid.Parse("0195a49a-0b22-70ae-a522-518da9ebadb9"),
                [Guid.Parse("0195a49d-7c46-79bc-b5e7-5704f7b97ee3"), Guid.Parse("0195a49d-a733-7b00-84f8-89cac50eabe6")],
                DateTimeOffset.UtcNow),
            new TaskList(
                Guid.Parse("0195a4a4-7078-7f59-af9e-30d9f49d78e0"), 
                "J List 2", 
                Guid.Parse("0195a49a-0b22-70ae-a522-518da9ebadb9"),
                [],
                DateTimeOffset.UtcNow),
            new TaskList(
                Guid.Parse("0195a4a4-fb9a-72f5-ab5e-cfac8d5c0eff"), 
                "J List 3", 
                Guid.Parse("0195a49a-0b22-70ae-a522-518da9ebadb9"),
                [Guid.Parse("0195a49d-a733-7b00-84f8-89cac50eabe6")],
                DateTimeOffset.UtcNow),
            new TaskList(
                Guid.Parse("0195a547-c68c-7d63-96d5-c2d3fa1a386c"), 
                "M List 1", 
                Guid.Parse("0195a49d-7c46-79bc-b5e7-5704f7b97ee3"),
                [Guid.Parse("0195a49d-a733-7b00-84f8-89cac50eabe6"), Guid.Parse("0195a49a-0b22-70ae-a522-518da9ebadb9")],
                DateTimeOffset.UtcNow),
            new TaskList(
                Guid.Parse("0195a549-4940-7c40-9170-92223a679f20"), 
                "B List 1", 
                Guid.Parse("0195a49d-a733-7b00-84f8-89cac50eabe6"),
                [],
                DateTimeOffset.UtcNow),
        };
        foreach (var taskList in taskLists)
        {
            threadTasks.Add(_databaseFacade.TaskLists.ReplaceOneAsync(MongoDB.Driver.Builders<TaskList>.Filter.Eq(u => u.Id, taskList.Id), taskList, new ReplaceOptions {IsUpsert = true}));
        }
        await Task.WhenAll(threadTasks);
        threadTasks.Clear();
        _logger.LogInformation("Task lists seeded successfully.");

        var tasks = new List<Domain.Models.Task>()
        {
            new Domain.Models.Task(
                Guid.Parse("0195a4a6-f1b0-7d3c-bb6c-a9f63c71b705"), 
                "Task 1",
                "Description description",
                DateTimeOffset.UtcNow.AddDays(3),
                DateTimeOffset.UtcNow,
                false,
                Guid.Parse("0195a49f-3fdf-72b4-bada-83f60e5c7881")),
            new Domain.Models.Task(
                Guid.Parse("0195a4ae-5522-7c0e-8b12-9d3475c6a49e"), 
                "Task 2",
                "Description description",
                DateTimeOffset.UtcNow.AddDays(1),
                DateTimeOffset.UtcNow,
                true,
                Guid.Parse("0195a49f-3fdf-72b4-bada-83f60e5c7881")),
            new Domain.Models.Task(
                Guid.Parse("0195a4ae-8497-75b0-a410-4ae4f3e24002"), 
                "Task 3",
                "Description description",
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow,
                false,
                Guid.Parse("0195a49f-3fdf-72b4-bada-83f60e5c7881")),
            new Domain.Models.Task(
                Guid.Parse("0195a4b4-5549-7f44-afa6-2f55cd259bf9"), 
                "Task 4",
                "Description description",
                DateTimeOffset.UtcNow.AddDays(1),
                DateTimeOffset.UtcNow,
                true,
                Guid.Parse("0195a4a4-fb9a-72f5-ab5e-cfac8d5c0eff")),
        };
        foreach (var task in tasks)
        {
            threadTasks.Add(_databaseFacade.Tasks.ReplaceOneAsync(MongoDB.Driver.Builders<Domain.Models.Task>.Filter.Eq(u => u.Id, task.Id), task, new ReplaceOptions {IsUpsert = true}));
        }
        await Task.WhenAll(threadTasks);
        threadTasks.Clear();
        _logger.LogInformation("Tasks seeded successfully.");
        _logger.LogInformation("Seeding database finished {DateTime}.", DateTimeOffset.UtcNow);
    }
}
