﻿using System;
using System.Web;
using System.Web.Mvc;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.Members.Controllers 
{
    public class BecomeMemberController : Controller
    {
        // GET: Members/BecomeMember
        private readonly MembersInteractor _memberManager; 
        private readonly MemberDocumentInteractor _memberDocumentInteractor; 
        public BecomeMemberController(MembersInteractor memberManager,MemberDocumentInteractor memberDocumentInteractor)
        {
            _memberManager = memberManager;            
            _memberDocumentInteractor = memberDocumentInteractor;
        }       
        public ActionResult Register() 
        {
            var model = _memberManager.GetProfileInitialData(); 
            return View(model);
        } 
        //public ActionResult GetSpecialization()
        //{
        //   // var specialization = _specializationInteractor.GetAllSpecialization();
        //    //var data = specialization.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.Name }).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult AddMemeber(RequestMemberDto requestMemberDto)
        {
            try
            {
                _memberManager.AddMember(requestMemberDto);                 
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateMember", requestMemberDto);

            } 

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
        public ActionResult Update(long id) 
        {
            var result = _memberManager.GetMemberById(id);
            return View(result);
        }
        public ActionResult UpdateStatus(long Id, string FieldName, bool check)
        {
            var result = _memberManager.UpdateStatus(Id, FieldName, check);
            return RedirectToAction("Index", result);
        }
        public ActionResult UpdateMemberStatus(long Id)
        {
            var result = _memberManager.UpdateMemberStatus(Id, "StatusID", 0);
            return RedirectToAction("Index", result);
        }
        public ActionResult AddMemberDocument(long Id, HttpPostedFileBase file)
        { 
            var result = _memberDocumentInteractor.UploadDocument(Id,file);
            return RedirectToAction("GetMemberDocument",Id);
        }
        public ActionResult Documents(long Id)
        {
            var result = _memberDocumentInteractor.GetAllFiltered(Id);
            ViewBag.MemberId = Id;
            return View("GetMemberDocument",result);
        } 
        public ActionResult DeleteMemberDocument(int Id)
        {
            var result = _memberDocumentInteractor.Delete(Id);
            return RedirectToAction("GetMemberDocument",Id);
        }
    }
}