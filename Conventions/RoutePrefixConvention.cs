using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Hoofd;

public class RoutePrixConvention(string prefix) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _prefix = new(new RouteAttribute(prefix));

    public void Apply(ApplicationModel application)
    {
        foreach(var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            if (selector.AttributeRouteModel is not null)
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                    _prefix,
                    selector.AttributeRouteModel
                );
            }
        }
    }
}