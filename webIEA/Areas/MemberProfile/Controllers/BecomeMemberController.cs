using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.Contracts;
using webIEA.Dtos;

namespace webIEA.Areas.MemberProfile.Controllers
{
    public class BecomeMemberController : Controller
    {
        // GET: MemberProfile/BecomeMember
        private readonly IMemberManager _memberManager;

        public BecomeMemberController(IMemberManager memberManager)
        {
            _memberManager = memberManager;
        }
        public ActionResult Index()
        {
            return View(new RequestMemberDto());
        }
        public  ActionResult AddMemeber(RequestMemberDto requestMemberDto)
        {
            var result = _memberManager.AddMember(requestMemberDto);
            return View(result);
        }
        public ActionResult GetAllMembers()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult GetMemberById(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }


    }
}