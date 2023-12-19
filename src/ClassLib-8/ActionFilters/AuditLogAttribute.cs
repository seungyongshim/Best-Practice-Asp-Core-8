using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib_8.AuditConverter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClassLib_8.ActionFilters;

public class AuditLogAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await base.OnActionExecutionAsync(context, next);

        foreach (var src in context.ActionDescriptor.Parameters)
        {
            if (src.BindingInfo?.BindingSource == BindingSource.Body)
            {
                var converter = context.HttpContext.RequestServices.GetRequiredService(typeof(IDtoAuditConverter<>).MakeGenericType(src.ParameterType));

                context.HttpContext.Items["bodyType"] = converter.ToConvert(context.ActionArguments[src.Name], src.ParameterType);
            }
        }
    }
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuditLogAttribute>>();

        await base.OnResultExecutionAsync(context, next);

        logger.LogInformation("{@body}", "here");
    }
        
}
