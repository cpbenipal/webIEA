using System;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class SessionController : Controller
    {
        public EmptyResult RenewSession()
        {
            return new EmptyResult();
        }

        public Boolean CheckAuth()
        {
            return Request.RequestContext.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}