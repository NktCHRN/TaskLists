namespace TaskLists.Infrastructure.Abstractions;

public interface IDatabaseSeeder
{
    Task Seed();
}