using Flexpage.Domain.Abstract;
using Flexpage.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FlexPage2.Areas.Flexpage.Infrastructure
{
    public class FormsAuthProvider 
    {
        public static string Encrypt(string plaintext)
        {
            return plaintext;
        }
        public static string Decrypt(string plaintext)
        {
            return plaintext;
        }
        public static string GeneratePassword()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int len = 8;
            StringBuilder sb = new StringBuilder(len);
            Random rand = new Random();
            for (int i = 0; i < len; i++)
            {
                sb.Append(chars[rand.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }

    public class FlexpageAdminAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return FlexpageSettings.Instance.IsCmsAdminContext(httpContext.ApplicationInstance.Context);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { redirectTo = FormsAuthentication.LoginUrl }
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }

    public class FlexpageAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.IsAuthenticated;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if(filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { redirectTo = FormsAuthentication.LoginUrl }
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}