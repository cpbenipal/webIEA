using Flexpage.Domain.Abstract;
using System.Linq;
using System.Web.Mvc;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.MemberProfile.Controllers
{
    public class BecomeMemberController : Controller
    {
        // GET: MemberProfile/BecomeMember
        private readonly MembersInteractor _memberManager;
        private readonly MemberStatusInteractor _memberStatusManager;
        private readonly SpecializationInteractor _specializationInteractor;

        protected readonly ILanguageRepository _ocessor;
        public BecomeMemberController(MembersInteractor memberManager, MemberStatusInteractor memberStatusManager,
            SpecializationInteractor specializationInteractor, ILanguageRepository ocessor)
        {
            _memberManager = memberManager;
            _memberStatusManager = memberStatusManager;
            _ocessor = ocessor;
            _specializationInteractor = specializationInteractor;
        }
        public ActionResult Index()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult CreateMember()
        {
            var model = new RequestMemberDto();
            var languages = _ocessor.GetLanguages();
            model.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList(); 
            var specialization = _specializationInteractor.GetAllSpecialization();
            model.Specialization = specialization.Select(x => new ListCollectionDto() { Id = (int)x.Id,  Value = x.Name }).ToList();
            model.Statuses = _memberStatusManager.GetAllStatus();
            return View(model);
        }
        public ActionResult GetSpecialization()
        {
            var specialization = _specializationInteractor.GetAllSpecialization();
            var data = specialization.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

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