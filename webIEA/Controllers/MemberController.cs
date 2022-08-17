using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.App_Start;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Controllers
{
    public class MemberController : Controller
    {
        private readonly MembersInteractor _memberManager;
        private readonly MemberDocumentInteractor _memberDocumentInteractor;
        public MemberController(MembersInteractor memberManager, MemberDocumentInteractor memberDocumentInteractor)
        {
            _memberManager = memberManager;
            _memberDocumentInteractor = memberDocumentInteractor;

        }
        // GET: IEAdmin/Members
        //   [CustomAuthorizeAttribute("Admin")]
        public ActionResult Index()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new RequestMemberDto();
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult AddMemeber(RequestMemberDto requestMemberDto)
        {
            try
            {
                _memberManager.AddMember(requestMemberDto);
                return View("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateMember", requestMemberDto);

            }

        }
        [CustomAuthorizeAttribute("Admin")]
        public ActionResult GetAllMembers()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult Details(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        [CustomAuthorizeAttribute("Admin")]
        public ActionResult GetMemberById(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        [CustomAuthorizeAttribute("Admin")]
        public ActionResult EditMemeber(MembersDto membersDto)
        {
            _memberManager.UpdateMember(membersDto);
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute("Admin")]
        public ActionResult UpdateStatus(long Id, string FieldName, bool check)
        {
            var result = _memberManager.UpdateStatus(Id, FieldName, check);
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute("Admin")]
        public ActionResult UpdateMemberStatus(long Id)
        {
            var result = _memberManager.UpdateMemberStatus(Id, "StatusID", (int)MemberStatusEnum.Active);
            return RedirectToAction("Index", "Member");

        }
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult Update(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult UpdatePassword(UpdatePasswordDto dto)
        {
            var result = _memberManager.UpdatePassword(dto);
            return View(result);
        }
        public ActionResult UnAuthorized()
        {
            ViewBag.Message = "Un Authorized Page!";

            return View();
        }

        [CustomAuthorizeAttribute("Admin", "Member")]

        public ActionResult AddMemberDocument(long Id, HttpPostedFileBase file)
        {
            var result = _memberDocumentInteractor.UploadDocument(Id, file);
            return RedirectToAction("GetMemberDocument", Id);
        }
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult Documents(long Id)
        {
            var result = _memberDocumentInteractor.GetAllFiltered(Id);
            ViewBag.MemberId = Id;
            return View("GetMemberDocument", result);
        }
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult DeleteMemberDocument(int Id)
        {
            var result = _memberDocumentInteractor.Delete(Id);
            return RedirectToAction("GetMemberDocument", Id);
        }
    }
}