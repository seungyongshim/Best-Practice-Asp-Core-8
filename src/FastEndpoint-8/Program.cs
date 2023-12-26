using FastEndpoint_8.Results;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultExceptionHandler()
   .UseFastEndpoints(config =>
{
    config.Errors.ResponseBuilder = (failures, httpContext, statusCode) =>
        new CustomErrorResponse(failures, httpContext, statusCode);
});
app.Run();
