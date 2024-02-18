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
            IValueHttpResult v => Results.Text
            (
                AddTraceId(v.Value).ToJsonString(serializerOptions),
                contentType: v switch
                {
                    IContentTypeHttpResult c => c.ContentType,
                    _ => MediaTypeNames.Application.Json
                },
                statusCode: v switch
                {
                    IStatusCodeHttpResult c => c.StatusCode,
                    _ => 200
                }
            ),
            ContentHttpResult { ResponseContent: not null } v => Results.Text
            (
                AddTraceStringId(v.ResponseContent).ToJsonString(serializerOptions),
                contentType: v.ContentType ?? MediaTypeNames.Application.Json,
                statusCode: v.StatusCode
            ),
            string v => Results.Text
            (
                AddTraceStringId(v).ToJsonString(serializerOptions),
                contentType: MediaTypeNames.Application.Json,
                statusCode: 200
            ),
            Ok v => Results.Text
            (
                $$"""{"traceId":"{{id}}"}""",
                contentType: MediaTypeNames.Application.Json,
                statusCode: v switch
                {
                    IStatusCodeHttpResult c => c.StatusCode,
                    _ => 200
                }
            ),
            IResult v => v,
            { } v => Results.Text
            (
                AddTraceId(v).ToJsonString(serializerOptions),
                MediaTypeNames.Application.Json,
                statusCode: 200
            ),
            _ => ret
        };

        JsonNode AddTraceStringId(string value)
        {
            var root = JsonNode.Parse(value)!.Root;
            root["traceId"] = id;

            return root;
        }

        JsonNode AddTraceId(object? value)
        {
            var root = JsonSerializer.SerializeToNode(value, serializerOptions)!.Root;
            root["traceId"] = id;

            return root;
        }
    }
}
