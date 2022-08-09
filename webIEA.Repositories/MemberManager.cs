
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class MemberManager : IMemberManager
    {
        private readonly IRepositoryBase<MemberProfile> _repositoryBase;
        private readonly IRepositoryBase<MemberSpecialization> _speciallizationrepo;
        private readonly IRepositoryBase<MemberTranieeCommission> _commissionrepo;
        private readonly Mapper mapper;

        public MemberManager(IRepositoryBase<MemberProfile> repositoryBase)
        {
            var _mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<MemberProfile, MembersDto>());
            mapper = new Mapper(_mapConfig);
            _repositoryBase = repositoryBase;

        }

        public List<MemberProfile> GetAllMembers()
        {          
            return _repositoryBase.GetAll().ToList();
        }
        public MemberProfile GetMemberById(long id)
        { 
            return _repositoryBase.GetById(id);
        }
        public long AddMember(RequestMemberDto membersDto)
        {

            var data = new MemberProfile
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
                StatusID = 1,
                EmploymentStatusID = membersDto.EmploymentStatusID,
                StatusIDPublic = true,
                SpecializationPublic = true,
                TraineeCommissionPublic=true,
                AddedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };
            var result = _repositoryBase.Insert(data);
            _repositoryBase.Save();
            return result.Id;

        }
        public object UpdateMember(MembersDto membersDto)
        {
            //var specdata = _speciallizationrepo.GetAll().ToList();
            //var commdata = _commissionrepo.GetAll().ToList();
            //_speciallizationrepo.DeleteList(specdata);
            //_commissionrepo.DeleteList(commdata);
            var data = _repositoryBase.GetById(membersDto.Id);
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
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;

        }
        public object UpdateStatus(long Id, string FieldName, bool check)
        {
            var data = _repositoryBase.GetById(Id);
            data.GetType().GetProperty(FieldName).SetValue(data, check, null);
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;

        }
        public object UpdateMemberStatus(long Id, string FieldName, int status)
        {
            var data = _repositoryBase.GetById(Id);
            data.StatusID = data.StatusID==3?2:3;
            //=
            //data.GetType().GetProperty(FieldName).SetValue(data, status, null);
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;

        }
    }
}
