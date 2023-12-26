using System.Diagnostics;
using FastEndpoints;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseFastEndpoints(config =>
{
    //config.Errors.UseProblemDetails();
});
app.Run();
