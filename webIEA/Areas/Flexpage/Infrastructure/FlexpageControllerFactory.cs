using Flexpage.Controllers;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Flexpage.Infrastructure
{
    public class FlexpageControllerFactory : DefaultControllerFactory
    {
        public FlexpageControllerFactory()
        {
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                requestContext.RouteData.Values["alias"] = requestContext.HttpContext.Request.Path.TrimStart('/');
                requestContext.RouteData.Values["action"] = "CmsPage";
                requestContext.RouteData.Values["Request.QueryString"] = requestContext.HttpContext.Request.QueryString;
                requestContext.RouteData.Values["Request.Url"] = requestContext.HttpContext.Request.Url;

                return (IController)DependencyResolver.Current.GetService(typeof(FlexpageController));
            }
            return base.GetControllerInstance(requestContext, controllerType);
            //return (IController)DependencyResolver.Current.GetService(controllerType);
        }
    }
}
