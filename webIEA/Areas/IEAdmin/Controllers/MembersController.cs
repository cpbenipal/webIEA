using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.App_Start;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.IEAdmin.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersInteractor _memberManager; 
        public MembersController(MembersInteractor memberManager)
        {
            _memberManager = memberManager;            
        }
        // GET: IEAdmin/Members
        [CustomAuthorizeAttribute("Admin")]
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
            var result = _memberManager.UpdateMemberStatus(Id, "StatusID", (int)MemberStatusEnum.Active);  
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
        public ActionResult UnAuthorized()
        {
            ViewBag.Message = "Un Authorized Page!";

            return View();
        }
    }
}