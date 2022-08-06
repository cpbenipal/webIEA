using Flexpage.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using webIEA.Dtos;
using webIEA.Entities;
using webIEA.Interactor;
using WebIEA.Repositories;

namespace webIEA.Areas.MemberProfile.Controllers
{
    public class BecomeMemberController : Controller
    {
        // GET: MemberProfile/BecomeMember
        private readonly MembersInteractor _memberManager;
        private readonly MemberStatusInteractor _memberStatusManager;
        private readonly EmploymentStatusInteractor _employmentStatusInteractor;
        private readonly MemberSpecializationInteractor _specializationInteractor;
        private readonly TraineeCourseInteractor _traineeCourseInteractor;
        private readonly CourseMemberInteractor _courseMemberInteractor;
        private readonly UnitOfWorkInteractor<MemberTranieeCommission> _unitOfWork;

        protected readonly ILanguageRepository _ocessor;
        public BecomeMemberController(MembersInteractor memberManager, MemberStatusInteractor memberStatusManager,
            MemberSpecializationInteractor specializationInteractor,
            ILanguageRepository ocessor,EmploymentStatusInteractor employmentStatusInteractor,
            TraineeCourseInteractor traineeCourseInteractor,
            CourseMemberInteractor courseMemberInteractor,
            UnitOfWorkInteractor<MemberTranieeCommission> unitOfWorkInteractor
            )
        {
            _memberManager = memberManager;
            _memberStatusManager = memberStatusManager;
            _ocessor = ocessor;
            _specializationInteractor = specializationInteractor;
            _employmentStatusInteractor = employmentStatusInteractor;
            _traineeCourseInteractor = traineeCourseInteractor;
            _courseMemberInteractor = courseMemberInteractor;
            _unitOfWork = unitOfWorkInteractor;
        }
        public ActionResult Index()
        {
            _unitOfWork.GetAllFiltered();

            var result = _memberManager.GetAllMembers();
            return View(result);
        }
        public ActionResult CreateMember()
        {
            var model = new RequestMemberDto();
            var languages = _ocessor.GetLanguages();
            model.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList();
            var employmentstatus = _employmentStatusInteractor.GetAll();
            model.EmploymentStatuses = employmentstatus.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.StatusName }).ToList();
            var traningcourse = _traineeCourseInteractor.GetAll();
            model.TranieeCommission = traningcourse.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.TrainingName }).ToList();
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
                var result = _memberManager.AddMember(requestMemberDto);
                foreach (var tarnId in requestMemberDto.TraneeComissionId)
                {
                    var cmdt = new CourseMemberDto();
                    cmdt.MemberID = result;
                    cmdt.TrainingCourseId = Convert.ToInt32(tarnId);
                    _courseMemberInteractor.Add(cmdt);
                }
                foreach (var sepname in requestMemberDto.Specialization)
                {
                    var msdt = new MemberSpecializationDto();
                    msdt.MemberId = (int)result;
                    msdt.SpecializationName = sepname;
                    _specializationInteractor.Add(msdt);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateMember", requestMemberDto);

            }

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
            var traineedt = _courseMemberInteractor.GetAllFiltered(membersDto.Id).ToList();
            var result = _memberManager.UpdateMember(membersDto);
            
            
            var tdt = traineedt.Select(x => new MemberTranieeCommission
            {
                Id = (int)x.Id,
                MemberID = x.MemberID,
                TrainingCourseId = x.TrainingCourseId,

            }).ToList();
            
            foreach (var tarnId in membersDto.TraneeComissionId)
            {
                var cmdt = new CourseMemberDto();
                cmdt.MemberID = membersDto.Id;
                cmdt.TrainingCourseId = Convert.ToInt32(tarnId);
                _courseMemberInteractor.Add(cmdt);
            }
            foreach (var sepname in membersDto.Specialization)
            {
                var msdt = new MemberSpecializationDto();
                msdt.MemberId = (int)membersDto.Id;
                msdt.SpecializationName = sepname;
                _specializationInteractor.Add(msdt);
            }
            return RedirectToAction("Index");
        }
        public ActionResult UpdateMember(long id)
        {
            var result = _memberManager.GetMemberById(id);
            result.TraneeComissionId = _courseMemberInteractor.GetAllFiltered(id).Select(x => x.TrainingCourseId).ToList();
            result.Specialization = _specializationInteractor.GetAllFiltered(id).Select(x => x.SpecializationName).ToList();
            var languages = _ocessor.GetLanguages();
            result.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList();
            var employmentstatus = _employmentStatusInteractor.GetAll();
            result.EmploymentStatuses = employmentstatus.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.StatusName }).ToList();
            var traningcourse = _traineeCourseInteractor.GetAll();
            result.TranieeCommission = traningcourse.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.TrainingName }).ToList();
           

            return View(result);
        }
        public ActionResult UpdateStatus(long Id, string FieldName, bool check)
        {
            var result = _memberManager.UpdateStatus(Id, FieldName, check);
            return RedirectToAction("Index", result);
        }


    }
}