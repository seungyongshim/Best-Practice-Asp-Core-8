using System.Reflection.Metadata.Ecma335;
using FluentValidation;
using Microsoft.OpenApi.Models;
using WebApplicationMinimalApi8.Dto;
using WebApplicationMinimalApi8.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.AddFluentValidationEndpointFilter();

if (builder.Environment.IsEnvironment("Best"))
{
    builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (builder.Environment.IsEnvironment("Best"))
{
    app.UseExceptionHandler();
}

app.MapPost("/validate", (MessageDto message) => message)
   .WithDescription("메시지를 검증합니다.")
   
   .AddFluentValidationFilter()
   .WithOpenApi(x =>
   {
       x.RequestBody = new OpenApiRequestBody
       {
           Content =
           {
               ["application/json"] = new OpenApiMediaType
               {
                   Schema = new OpenApiSchema
                   {
                       Reference = new OpenApiReference
                       {
                           Id = "MessageDto",
                           Type = ReferenceType.Schema
                       }
                   }
               }
           }
       };
       return x;
   });

app.MapGet("/500", () =>
{
    throw new Exception("이런 저런 에러가 났군요");
})
.WithName("InternalException")
.WithOpenApi();

app.Run();

