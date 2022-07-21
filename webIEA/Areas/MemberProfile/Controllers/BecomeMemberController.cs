using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.Dtos;

namespace webIEA.Areas.MemberProfile.Controllers
{
    public class BecomeMemberController : Controller
    {
        // GET: MemberProfile/BecomeMember
        public ActionResult Index()
        {
            return View(new RequestMemberDto());
        }
    }
}