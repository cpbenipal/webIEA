//using AutoMapper;
using Flexpage.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class MembersInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        protected readonly ILanguageRepository _ocessor;
        public MembersInteractor(IRepositoryWrapper _repositoryWrapper, ILanguageRepository ocessor)
        {
            repositoryWrapper = _repositoryWrapper; _ocessor = ocessor;
        }

        public RequestMemberDto GetProfileInitialData()
        {
            var model = new RequestMemberDto();
            var languages = _ocessor.GetLanguages();
            model.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList();
            var employmentstatus = repositoryWrapper.EmploymentStatusManager.GetAll();
            model.EmploymentStatuses = employmentstatus.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.StatusName }).ToList();
            var traningcourse = repositoryWrapper.TraineeCourseManager.GetAll();
            model.TranieeCommission = traningcourse.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.TrainingName }).ToList();
            return model;
        }

        public List<MembersDto> GetAllMembers()
        {
            var model = repositoryWrapper.MemberManager.GetAllMembers().Select(x => new MembersDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                Email = x.Email,
                DOB = x.DOB,
                Phone = x.Phone,
                StatusID = (int)x.StatusID,
            }).ToList();
            return model;
        }
        public MembersDto GetMemberById(long id)
        {
            var data = repositoryWrapper.MemberManager.GetMemberById(id);
            MembersDto m = new MembersDto();
            m.Id = data.Id;
            m.FirstName = data.FirstName;
            m.LastName = data.LastName;
            m.DOB = data.DOB;
            m.Email = data.Email;
            m.BirthPlace = data.BirthPlace;
            m.Nationality = data.Nationality;
            m.LanguageID = data.LanguageID;
            m.Phone = data.Phone;
            m.GSM = data.GSM;
            m.Street = data.Street;
            m.PostalCode = data.PostalCode;
            m.Commune = data.Commune;
            m.PrivateAddress = data.PrivateAddress;
            m.PrivatePostalCode = data.PrivatePostalCode;
            // m.StatusID = (int)data.StatusID; 

            m.FirstNamePublic = data.FirstNamePublic;
            m.LastNamePublic = data.LastNamePublic;
            m.DOBPublic = data.DOBPublic;
            m.EmailPublic = data.EmailPublic;
            m.BirthPlacePublic = data.BirthPlacePublic;
            m.NationalityPublic = data.NationalityPublic;
            m.LanguageIDPublic = data.LanguageIDPublic;
            m.PhonePublic = data.PhonePublic;
            m.GSMPublic = data.GSMPublic;
            m.StreetPublic = data.StreetPublic;
            m.PostalCodePublic = data.PostalCodePublic;
            m.CommunePublic = data.CommunePublic;
            m.PrivateAddressPublic = data.PrivateAddressPublic;
            m.PrivatePostalCodePublic = data.PrivatePostalCodePublic;
            m.StatusIDPublic = data.StatusIDPublic;
            m.EmploymentStatusID = (int)data.EmploymentStatusID;
            m.SpecializationPublic = data.SpecializationPublic;
            m.TraineeCommissionPublic = data.TraineeCommissionPublic;

            m.TraneeComissionId = repositoryWrapper.CourseMemberManager.GetAllFiltered(id).Select(x => x.TrainingCourseId).ToList();
            m.Specialization = repositoryWrapper.MemberSpecializationManager.GetAllFiltered(id).Select(x => x.SpecializationName).ToList();
            var languages = _ocessor.GetLanguages();
            m.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList();
            var employmentstatus = repositoryWrapper.MemberStatusManager.GetAll();
            m.EmploymentStatuses = employmentstatus.Select(x => new ListCollectionDto() { Id = (int)x.ID, Value = x.StatusName }).ToList();
            var traningcourse = repositoryWrapper.TraineeCourseManager.GetAll();
            m.TranieeCommission = traningcourse.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.TrainingName }).ToList();

            return m;
        }
        public long AddMember(RequestMemberDto requestMemberDto)
        {
            var result = repositoryWrapper.MemberManager.AddMember(requestMemberDto);

            if (requestMemberDto.TraneeComissionId.Count > 0)
            {
                foreach (var tarnId in requestMemberDto.TraneeComissionId)
                {
                    var cmdt = new CourseMemberDto();
                    cmdt.MemberID = result;
                    cmdt.TrainingCourseId = Convert.ToInt32(tarnId);
                    repositoryWrapper.CourseMemberManager.Add(cmdt);
                }
            }
            if (requestMemberDto.Specialization.Count > 0)
            {
                foreach (var sepname in requestMemberDto.Specialization)
                {
                    if (sepname != "")
                    {
                        var msdt = new MemberSpecializationDto();
                        msdt.MemberId = (int)result;
                        msdt.SpecializationName = sepname;
                        repositoryWrapper.MemberSpecializationManager.Add(msdt);
                    }
                }
            }
            return result;
        }
        public object UpdateMember(MembersDto membersDto)
        {
            var result = repositoryWrapper.MemberManager.UpdateMember(membersDto);
            repositoryWrapper.CourseMemberManager.DeleteList(membersDto.Id);
            if (membersDto.TraneeComissionId.Count > 0)
            {
                foreach (var tarnId in membersDto.TraneeComissionId)
                {
                    var cmdt = new CourseMemberDto();
                    cmdt.MemberID = membersDto.Id;
                    cmdt.TrainingCourseId = Convert.ToInt32(tarnId);
                    repositoryWrapper.CourseMemberManager.Add(cmdt);
                }
            }
            repositoryWrapper.MemberSpecializationManager.DeleteList(membersDto.Id);
            if (membersDto.Specialization.Count > 0)
            {
                foreach (var sepname in membersDto.Specialization)
                {
                    if (sepname != "")
                    {
                        var msdt = new MemberSpecializationDto();
                        msdt.MemberId = (int)membersDto.Id;
                        msdt.SpecializationName = sepname;
                        repositoryWrapper.MemberSpecializationManager.Add(msdt);
                    }
                }
            }
            return result;
        }
        public object UpdateStatus(long Id, string FieldName, bool check)
        {
            return repositoryWrapper.MemberManager.UpdateStatus(Id, FieldName, check);
        }
        public object UpdateMemberStatus(long Id, string FieldName, int Status)
        {
            return repositoryWrapper.MemberManager.UpdateMemberStatus(Id, FieldName, Status);
        }
        public object UpdatePassword(UpdatePasswordDto dto)
        {
            return repositoryWrapper.AccountManager.UpdatePassword(dto);
        }
    }
}
