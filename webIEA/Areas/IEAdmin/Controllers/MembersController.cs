using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.IEAdmin.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersInteractor _memberManager;
        private readonly MemberDocumentInteractor _memberDocumentInteractor;
        public MembersController(MembersInteractor memberManager, MemberDocumentInteractor memberDocumentInteractor)
        {
            _memberManager = memberManager;
            _memberDocumentInteractor = memberDocumentInteractor;
        }
        // GET: IEAdmin/Members
        public ActionResult Index()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult GetAllMembers()
        {
            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult Details(long id)
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
            _memberManager.UpdateMember(membersDto);
            return RedirectToAction("Index");
        }
        public ActionResult UpdateStatus(long Id, string FieldName, bool check)
        {
            var result = _memberManager.UpdateStatus(Id, FieldName, check);
            return RedirectToAction("Index");
        }
        public ActionResult UpdateMemberStatus(long Id)
        {
            var result = _memberManager.UpdateMemberStatus(Id, "StatusID", 0);
            return RedirectToAction("Index");
        }
        public ActionResult Update(long id)
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        public ActionResult UpdatePassword(UpdatePasswordDto dto)
        {
            var result = _memberManager.UpdatePassword(dto);
            return View(result);
        }
    }
}