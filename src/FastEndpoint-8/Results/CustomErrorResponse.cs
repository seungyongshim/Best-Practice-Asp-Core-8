
using System.ComponentModel;
using System.Reflection;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.Metadata;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Net.Mime;
using System.Diagnostics;



#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;
#endif

namespace FastEndpoint_8.Results;




public class CustomErrorResponse : IResult, IEndpointMetadataProvider
{
    static readonly string[] _item = { "application/problem+json" };

    public static void PopulateMetadata(MethodInfo _, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata
        (
            400,
            typeof(ProblemDetails),
            _item
        ));
    }

    [DefaultValue(400)]
    public int StatusCode { get; set; }

    [DefaultValue("One or more errors occurred!")]
    public string Message { get; set; } = "One or more errors occurred!";

    public Dictionary<string, List<string>> Errors { get; set; } = new();

    public CustomErrorResponse() { }

    public string TraceId { get; init; } = Activity.Current?.Id ?? default;

    public CustomErrorResponse(List<ValidationFailure> failures, HttpContext httpContext, int statusCode = 400)
    {
        StatusCode = statusCode;
        Errors = failures.GroupToDictionary(
            f => f.PropertyName,
            v => v.ErrorMessage);
        
    }

    public Task ExecuteAsync(HttpContext httpContext) =>
        httpContext.Response.SendAsync(this, StatusCode);
}
file static class MiscExtensions
{
    internal static Dictionary<TKey, List<TValue>> GroupToDictionary<TItem, TKey, TValue>(this List<TItem> items,
                                                                                          Func<TItem, TKey> keySelector,
                                                                                          Func<TItem, TValue> valueSelector) where TKey : notnull
    {
        var dict = new Dictionary<TKey, List<TValue>>();

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var key = keySelector(item);

            if (!dict.TryGetValue(key, out var group))
            {
                group = new(1);
                dict.Add(key, group);
            }
            group.Add(valueSelector(item));
        }

        return dict;
    }
}
