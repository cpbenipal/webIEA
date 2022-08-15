//using AutoMapper;
using Flexpage.Domain.Abstract;
using FlexPage.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly IHashManager _hashManager;

        public MembersInteractor(IRepositoryWrapper _repositoryWrapper, ILanguageRepository ocessor, IHashManager hashManager) 
        {
            repositoryWrapper = _repositoryWrapper; _ocessor = ocessor;
            this._hashManager = hashManager;
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
            var statuses = repositoryWrapper.MemberStatusManager.GetAll();
            var userLogins = repositoryWrapper.AccountManager.GetAll();
            var model = repositoryWrapper.MemberManager.GetAllMembers().Select(x => new MembersDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                Email = x.Email,
                DOB = x.DOB,
                Phone = x.Phone,
                StatusID = (int)x.StatusID,
                Password = (int)x.StatusID == 2 ? _hashManager.DecryptCipherText("WlZuQ0lQJEM=") : "",
                StatusName = statuses.FirstOrDefault(xx => xx.ID == x.StatusID).StatusName,
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
            m.EmploymentStatusIDPublic = data.EmploymentStatusIDPublic ?? false;
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
        public long AddMember(RequestMemberDto membersDto)
        { 
            var memberProfile = new MemberProfile
            {
                Id = membersDto.Id,
                FirstName = membersDto.FirstName,
                FirstNamePublic = true,
                LastName = membersDto.LastName,
                LastNamePublic = true,
                DOB = membersDto.DOB,
                DOBPublic = true,
                Email = membersDto.Email,
                EmailPublic = true,
                BirthPlace = membersDto.BirthPlace,
                BirthPlacePublic = true,
                Nationality = membersDto.Nationality,
                NationalityPublic = true,
                LanguageID = membersDto.LanguageID,
                LanguageIDPublic = true,
                Phone = membersDto.Phone,
                PhonePublic = true,
                GSM = membersDto.GSM,
                GSMPublic = true,
                Street = membersDto.Street,
                StreetPublic = true,
                PostalCode = membersDto.PostalCode,
                PostalCodePublic = true,
                Commune = membersDto.Commune,
                CommunePublic = true,
                PrivateAddress = membersDto.PrivateAddress,
                PrivateAddressPublic = true,
                PrivatePostalCode = membersDto.PrivatePostalCode,
                PrivatePostalCodePublic = true,
                StatusID = (int)MemberStatusEnum.Pending,
                EmploymentStatusID = membersDto.EmploymentStatusID,
                EmploymentStatusIDPublic = true,
                SpecializationPublic = true,
                TraineeCommissionPublic = true,
                AddedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };
            var result = repositoryWrapper.MemberManager.AddMember(memberProfile);

            if (membersDto.TraneeComissionId.Count > 0) 
            {
                foreach (var tarnId in membersDto.TraneeComissionId)
                {
                    var cmdt = new CourseMemberDto();
                    cmdt.MemberID = result;
                    cmdt.TrainingCourseId = Convert.ToInt32(tarnId);
                    repositoryWrapper.CourseMemberManager.Add(cmdt);
                }
            }
            if (membersDto.Specialization.Count > 0)
            {
                foreach (var sepname in membersDto.Specialization)
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
            //var specdata = _speciallizationrepo.GetAll().ToList();
            //var commdata = _commissionrepo.GetAll().ToList();
            //_speciallizationrepo.DeleteList(specdata);
            //_commissionrepo.DeleteList(commdata);
            var data = repositoryWrapper.MemberManager.GetMemberById(membersDto.Id);
            data.FirstName = membersDto.FirstName;
            //FirstNamePublic = membersDto.FirstNamePublic,
            data.LastName = membersDto.LastName;
            //data.LastNamePublic = membersDto.LastNamePublic,
            data.DOB = membersDto.DOB;
            // DOBPublic = membersDto.DOBPublic,
            data.Email = membersDto.Email;
            //EmailPublic = membersDto.EmailPublic,
            data.BirthPlace = membersDto.BirthPlace;
            //data.BirthPlacePublic = membersDto.BirthPlacePublic;
            data.Nationality = membersDto.Nationality;
            // data.NationalityPublic = membersDto.NationalityPublic,
            data.LanguageID = membersDto.LanguageID;
            //  data.LanguageIDPublic = membersDto.LanguageIDPublic,
            data.Phone = membersDto.Phone;
            // data.PhonePublic = membersDto.PhonePublic,
            data.GSM = membersDto.GSM;
            // data.GSMPublic = membersDto.GSMPublic,
            data.Street = membersDto.Street;
            // data.StreetPublic = membersDto.StreetPublic,
            data.PostalCode = membersDto.PostalCode;
            // data.PostalCodePublic = membersDto.PostalCodePublic,
            data.Commune = membersDto.Commune;
            //data.CommunePublic = membersDto.CommunePublic,
            data.PrivateAddress = membersDto.PrivateAddress;
            //data.PrivateAddressPublic = membersDto.PrivateAddressPublic,
            data.PrivatePostalCode = membersDto.PrivatePostalCode;
            // PrivatePostalCodePublic = membersDto.PrivateAddressPublic,
            data.EmploymentStatusID = membersDto.EmploymentStatusID;
            // data.StatusIDPublic = membersDto.StatusIDPublic,

            var result = repositoryWrapper.MemberManager.UpdateMember(data);

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
            var data = repositoryWrapper.MemberManager.GetMemberById(Id);
            data.GetType().GetProperty(FieldName).SetValue(data, check, null);
            return repositoryWrapper.MemberManager.UpdateStatus(data);
        }
        public object UpdateMemberStatus(long Id, string FieldName, int Status)
        {
            MemberProfile memberProfile = new MemberProfile();
            var dt = repositoryWrapper.MemberManager.GetMemberById(Id);
            if (dt.StatusID == (int)MemberStatusEnum.Pending)
            {
                var model = new AccountDto
                {
                    Email = dt.Email,
                    TableName = "MemberProfile",
                    loginUserId = dt.Id,
                    RoleId = (int)Roles.Member,

                };                

                var password = CommonUtils.GenratePassword();
                var hashed = _hashManager.HashWithSalt(password);
                
                var data = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    Password = _hashManager.EncryptPlainText(password),
                    PasswordHash = hashed[0],
                    PasswordSalt = hashed[1],
                    RoleId = model.RoleId,
                    loginUserId = model.loginUserId,
                    TableName = model.TableName,
                };

                var response = repositoryWrapper.AccountManager.Register(data);
                if(response != null)
                {
                    dt.StatusID = Status;
                    dt.ModifiedBy = 1;
                    dt.ModifiedOn = DateTime.Now;
                    memberProfile = repositoryWrapper.MemberManager.UpdateStatus(dt);
                    var mailHelper = new MailHelper();
                    bool resultNotifing = mailHelper.SendMail(ConfigurationManager.AppSettings["FromAddress"], model.Email, "IAE - IEA - Account Approved", $"Your account has been approved. <br/><br/> Here's login email is {model.Email} and password is {password}.");
                    if (resultNotifing)
                    {
                         
                    }
                }
            }
            
            return memberProfile;
        }
        public object UpdatePassword(UpdatePasswordDto dto)
        {
            return repositoryWrapper.AccountManager.UpdatePassword(dto);
        }
    }
}
