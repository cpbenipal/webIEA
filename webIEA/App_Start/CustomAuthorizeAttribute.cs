using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using webIEA.DataBaseContext;

namespace webIEA.App_Start
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        WebIEAContext2 context = new WebIEAContext2(); // my entity  
        private readonly string[] allowedroles;
        private readonly string UserId;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
            this.UserId = (string)HttpContext.Current.Session["Id"];
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (UserId != null && UserId != "")
            {
                foreach (var role in allowedroles)
                {
                    var user = (from u in context.Users join ur in context.UserRoles on u.RoleId equals ur.Id
                               where u.Id == UserId && ur.Name == role
                               select new { u }).ToList();
                    if (user.Count>4)
                    {
                        authorize = true;
                    }
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
