using System;
using Flexpage.Domain.Entities;
using Flexpage.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using Pluritech.Services;
using Flexpage.Abstract;

namespace Flexpage.Infrastructure
{
    public class RoleAccessAttribute : ActionFilterAttribute
    {
        private IPageAccessProvider _pageAccessProvider
        {
            get
            {
                return DependencyResolver.Current.GetService<IPageAccessProvider>();
            }
        }

        public RoleAccessAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string alias = filterContext.RequestContext.HttpContext.Request.Path.Substring(1);
            var route = ((Route)RouteTable.Routes["Default"]).Defaults;
            alias = alias.TrimEnd('/');
            if (string.IsNullOrEmpty(alias) || alias.Equals(route["controller"].ToString(), StringComparison.OrdinalIgnoreCase))
            {
                alias = $"{route["controller"]}/{route["action"]}";
            }
            if (!_pageAccessProvider.CanViewPage(alias, (role) => { return filterContext.HttpContext.User.IsInRole(role); }))
            {
                filterContext.Result = new RedirectResult("/Account/Login?ReturnUrl=/" + alias);
            }
        }

    }
    
}