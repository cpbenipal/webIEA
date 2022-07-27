using System.Web.Mvc;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.MemberProfile.Controllers
{
    public class BecomeMemberController : Controller
    {
        // GET: MemberProfile/BecomeMember
        private readonly MembersInteractor _memberManager;

        public BecomeMemberController(MembersInteractor memberManager)
        {
            _memberManager = memberManager;
        }
        public ActionResult Index()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult CreateMember()
        {
            return View(new RequestMemberDto());

        }
        public ActionResult AddMemeber(RequestMemberDto requestMemberDto)
        {
            var result = _memberManager.AddMember(requestMemberDto);
            return RedirectToAction("Index");
        }
        public ActionResult GetAllMembers()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult MemberDetails(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        public ActionResult GetMemberById(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        public ActionResult EditMemeber(MembersDto membersDto)
        {
            var result = _memberManager.UpdateMember(membersDto);
            return RedirectToAction("Index");
        }
        public ActionResult UpdateMember(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        public ActionResult UpdateStatus(long Id, string FieldName, bool check)
        {
            var result = _memberManager.UpdateStatus(Id, FieldName, check);
            return RedirectToAction("Index", result);
        }


    }
}