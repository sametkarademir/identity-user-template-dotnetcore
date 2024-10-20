using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class FeatureToggleAttribute : ActionFilterAttribute
{
    public string FeatureToggleKey { get; }

    public FeatureToggleAttribute(string featureToggleKey)
    {
        FeatureToggleKey = featureToggleKey;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        var isFeatureEnabled = configuration.GetValue<bool>(FeatureToggleKey);

        if (!isFeatureEnabled)
        {
            context.Result = new NotFoundResult(); // 404 Not Found döndür
        }

        base.OnActionExecuting(context);
    }
}