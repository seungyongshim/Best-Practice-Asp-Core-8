## Minimal API 흐름

1. MVC와 달리 Model Binder가 없다.


## .Net 8에서 GlobalErrorHandling이 추가됨
* https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-8
* https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#produce-a-problemdetails-payload-for-exceptions

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
```
