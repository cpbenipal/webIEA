using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using webIEA.DataBaseContext;

namespace webIEA.App_Start
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        WebIEAContext2 context = new WebIEAContext2(); // my entity  
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
            filterContext.Result = new HttpUnauthorizedResult();             
        }
    }
}
