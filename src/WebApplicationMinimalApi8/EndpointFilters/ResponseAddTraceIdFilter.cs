using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace WebApplicationMinimalApi8.EndpointFilters;

public class ResponseAddTraceIdFilter : IEndpointFilter
{
    public static JsonNode AddTraceId(object? value, string id)
    {
        var root = JsonSerializer.SerializeToNode(value)!.Root;

        root["traceId"] = id;

        return root;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var id = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        var ret = await next(context);

        return ret switch
        {
            IStatusCodeHttpResult c => c switch
            {
                IValueHttpResult v => Results.Text(AddTraceId(v.Value, id).ToJsonString(), "application/json", statusCode: c.StatusCode),
                { } v => v
            },
            _ => ret
        };
    }


}
