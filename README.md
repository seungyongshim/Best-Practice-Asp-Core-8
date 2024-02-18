## Minimal API 흐름

1. MVC와 달리 Model Binder가 없다.


## .Net 8에서 GlobalErrorHandling이 추가됨
* https://juliocasal.com/blog/Global-Error-Handling-In-AspNet-Core-APIs.html*
* https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-8
* https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#produce-a-problemdetails-payload-for-exceptions

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);

var app = builder.Build();

app.UseExceptionHandler();
```

## Minimal API에서 FluentValidation 사용하기
* https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http
* `dotnet add package ForEvolve.FluentValidation.AspNetCore.Http`
```csharp
var root = app.MapGroup("/")
              .AddFluentValidationFilter();

root.MapPost(...)
```

## Swagger Request Exapmle 작성
* https://medium.com/@niteshsinghal85/multiple-request-response-examples-for-swagger-ui-in-asp-net-core-864c0bdc6619

## Request Body에서 null을 명시할 경우 무시할 수 없음
* `JsonIgnoreCondition.WhenReadingNull`이 추가될 예정도 없음
* https://stackoverflow.com/questions/77516935/ignore-json-null-value-in-system-text-json-deserialize

```csharp
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
    options.SerializerOptions.PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate;
    options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { InterceptNullSetter }
    };
});
```

## ProblemDetails에 TraceId 추가하기
* https://stackoverflow.com/questions/76669903/problemdetails-response-does-not-contain-traceid-property
```csharp
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = (context) =>
    {
        if (!context.ProblemDetails.Extensions.ContainsKey("traceId"))
        { 
            string? traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            context.ProblemDetails.Extensions.Add(new KeyValuePair<string, object?>("traceId", traceId));
        }
    }
);
```

## CRUD API 작성하기
* https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html
```
Create
---------------------------------------------------------------------
Success - 201 Created - Return created object
Failure - 400 Invalid request - Return details about the failure
Async fire and forget operation - 202 Accepted - Optionally return url for polling status

Update
---------------------------------------------------------------------
Success - 200 Ok - Return the updated object
Success - 204 NoContent
Failure - 404 NotFound - The targeted entity identifier does not exist
Failure - 400 Invalid request - Return details about the failure
Async fire and forget operation - 202 Accepted - Optionally return url for polling status

Patch
---------------------------------------------------------------------
Success - 200 Ok - Return the patched object
Success - 204 NoContent
Failure - 404 NotFound - The targeted entity identifier does not exist
Failure - 400 Invalid request - Return details about the failure
Async fire and forget operation - 202 Accepted - Optionally return url for polling status

Delete
---------------------------------------------------------------------
Success - 200 Ok - No content
Success - 200 Ok - When element attempting to be deleted does not exist
Async fire and forget operation - 202 Accepted - Optionally return url for polling status

Get
---------------------------------------------------------------------
Success - 200 Ok - With the list of resulting entities matching the search criteria
Success - 200 Ok - With an empty array

Get specific
---------------------------------------------------------------------
Success - 200 Ok - The entity matching the identifier specified is returned as content
Failure - 404 NotFound - No content

Action
---------------------------------------------------------------------
Success - 200 Ok - Return content where appropriate
Success - 204 NoContent
Failure - 400 - Return details about the failure
Async fire and forget operation - 202 Accepted - Optionally return url for polling status

Generic results
---------------------------------------------------------------------
Authorization error 401 Unauthorized
Authentication error 403 Forbidden
For methods not supported 405
Generic server error 500
```

## HttpClient의 올바른 사용방법
* https://devblogs.microsoft.com/dotnet/author/martintomka/
* https://learn.microsoft.com/ko-kr/dotnet/core/resilience/http-resilience
* `Microsoft.Extensions.Http.Resilience` 패키지 사용(암시적으로 polly를 사용함)
* `Chaos Engineering`
```

```

## 개인정보 로깅
* https://andrewlock.net/redacting-sensitive-data-with-microsoft-extensions-compliance/
