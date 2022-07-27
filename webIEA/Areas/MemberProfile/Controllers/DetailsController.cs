using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.Interactor;

namespace webIEA.Areas.MemberProfile.Controllers
{
    public class DetailsController : Controller
    {
        // GET: MemberProfile/Details
        private readonly MembersInteractor _memberManager;
        public DetailsController(MembersInteractor memberManager)
        {
            _memberManager = memberManager;
        }
        public ActionResult Index(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
    }
}