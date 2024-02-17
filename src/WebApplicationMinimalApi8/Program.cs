using Microsoft.Extensions.Hosting;
using WebApplicationMinimalApi8.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    

app.MapGet("/500", () =>
{
    throw new Exception("이런 저런 에러가 났군요");
})
.WithName("InternalException")6c
.WithOpenApi();

app.Run();

