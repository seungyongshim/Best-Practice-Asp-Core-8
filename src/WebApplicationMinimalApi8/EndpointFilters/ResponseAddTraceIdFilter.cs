using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Json;
using System.Net.Mime;

namespace WebApplicationMinimalApi8.EndpointFilters;

public class ResponseAddTraceIdFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var id = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        var serializerOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions;
        var ret = await next(context);

        return ret switch
        {
            //ValidationProblem c => AddTraceIdForFluentValidation(c),
            IValueHttpResult v => Results.Text
            (
                AddTraceId(v.Value).ToJsonString(serializerOptions),
                contentType: v switch
                {
                    IContentTypeHttpResult c => c.ContentType,
                    _ => "application/json"
                },
                statusCode: v switch
                {
                    IStatusCodeHttpResult c => c.StatusCode,
                    _ => 200
                }
            ),
            IResult v => Results.Text
            (
                $$"""{"traceId":"{{id}}"}""",
                contentType: "application/json",
                statusCode: v switch
                {
                    IStatusCodeHttpResult c => c.StatusCode,
                    _ => 200
                }
            ),
            { } v => Results.Text(AddTraceId(v).ToJsonString(serializerOptions), "application/json", statusCode: 200),
            _ => ret
        };

        ValidationProblem AddTraceIdForFluentValidation(ValidationProblem v)
        {
            v.ProblemDetails.Extensions["traceId"] = id;
            return v;
        }

        JsonNode AddTraceId(object? value)
        {
            var root = JsonSerializer.SerializeToNode(value, serializerOptions)!.Root;
            root["traceId"] = id;

            return root;
        }
    }
}
