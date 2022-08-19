using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using webIEA.DataBaseContext;

namespace webIEA.App_Start
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        readonly WebIEAContext2 context = new WebIEAContext2(); // my entity  
        private readonly string[] allowedroles;
         
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;            
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var UserId = Convert.ToString(httpContext.Session["Id"]); 
            if (UserId != null && UserId != "")
            {
                var user = (from u in context.Users
                            join ur in context.UserRoles on u.RoleId equals ur.Id
                            where u.Id == UserId
                            select new { ur.Name }).FirstOrDefault();

                foreach (var role in allowedroles)
                {
                    if (role == user.Name) return true;
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
           filterContext.Result = new RedirectToRouteResult(  
               new RouteValueDictionary  
               {  
                    { "controller", "Home" },  
                    { "action", "UnAuthorized" }  
               });          
        }
    }
     
    public class CustomAuthenticationFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute, System.Web.Mvc.Filters.IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["Id"])))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "Login" },
                     { "action", "Index" }
                });
            }
        }
    }
}
