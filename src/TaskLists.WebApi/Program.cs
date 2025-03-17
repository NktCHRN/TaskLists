using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Scalar.AspNetCore;
using TaskLists.Application;
using TaskLists.Domain;
using TaskLists.Infrastructure;
using TaskLists.Infrastructure.Abstractions;
using TaskLists.WebApi;
using TaskLists.WebApi.OutboundParameterTransformers;
using TaskLists.WebApi.UserContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDomainServices()
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddWebApiServices(builder.Configuration);

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();        // Use http://localhost:5113/scalar/v1 instead of Swagger.
    
    var databaseFacade = app.Services.GetRequiredService<IDatabaseFacade>();
    await databaseFacade.SetupDatabase();

    var seeder = app.Services.GetRequiredService<IDatabaseSeeder>();
    await seeder.Seed();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseMiddleware<UserContextMiddleware>();

app.Run();
