## Minimal API 흐름

1. MVC와 달리 Model Binder가 없다.


## .Net 8에서 GlobalErrorHandling이 추가됨
* https://juliocasal.com/blog/Global-Error-Handling-In-AspNet-Core-APIs.html*
* https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-8
* https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#produce-a-problemdetails-payload-for-exceptions

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
```

## Minimal API에서 FluentValidation 사용하기
* https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http
* `dotnet add package ForEvolve.FluentValidation.AspNetCore.Http`
```csharp

```

## Swagger Request Exapmle 작성
* https://medium.com/@niteshsinghal85/multiple-request-response-examples-for-swagger-ui-in-asp-net-core-864c0bdc6619

## Request Body에서 null을 명시할 경우 무시할 수 없음
* JsonIgnoreCondition은 현재 null 쓰기무시만 있고 null 읽기무시는 없음
* https://github.com/dotnet/runtime/issues/66490#issuecomment-1142804662
