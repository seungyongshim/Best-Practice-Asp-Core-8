using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebApplicationMinimalApi8.Dto;
using WebApplicationMinimalApi8.EndpointFilters;
using WebApplicationMinimalApi8.ExceptionHandlers;
using System.Linq;
using System.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.ExampleFilters());
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate;
    options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { InterceptNullSetter }
    };
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.AddFluentValidationEndpointFilter();


builder.Services.AddHttpClient("echo", client =>
{
    client.BaseAddress = new("https://httpbin.org");
});

if (builder.Environment.IsEnvironment("Best"))
{
    builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);
    builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails(options =>
        options.CustomizeProblemDetails = context =>
            context.ProblemDetails.Extensions["traceId"] =
                Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (builder.Environment.IsEnvironment("Best"))
{
    app.UseExceptionHandler();
}
var root = app.MapGroup("/")
              .AddEndpointFilter<ResponseAddTraceIdFilter>()
              .AddFluentValidationFilter();
              

root.MapPost("/validate", (MessageDto message) => Results.Ok())
    .WithDescription("메시지를 검증합니다.")
    .WithOpenApi();

root.MapGet("/500", () =>
    {
        throw new Exception("이런 저런 에러가 났군요");
    })
    .WithName("InternalException")
    .WithOpenApi();

root.MapPost("/Persons", (PersonDto person) => Results.Created("/Persons/1", new
    {
        Id = 1,
        person.Name,
        person.Age,
        person.Emails
    }))
    .WithDescription("사람을 생성합니다.")
    .WithOpenApi();

root.MapPut("/Persons/{id}", (PersonDto person, int id) => Results.Ok(new
    {
        Id = id,
        person.Name,
        person.Age,
        person.Emails
    }))
    .WithDescription("사람을 생성합니다.")
    .WithOpenApi();

app.Run();

static void InterceptNullSetter(JsonTypeInfo typeInfo)
{
    foreach (var (propertyInfo, setProperty) in from propertyInfo in typeInfo.Properties
                                                let setProperty = propertyInfo.Set
                                                where setProperty is not null
                                                select (propertyInfo, setProperty))
    {
        propertyInfo.Set = (obj, value) =>
        {
            if (value != null)
            {
                setProperty(obj, value);
            }
        };
    }
}
