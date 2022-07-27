
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

        public List<MembersDto> GetAllMembers()
        {
            var data = _repositoryBase.GetAll().ToList();
            var a = data.Select(x => new MembersDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                Email = x.Email,
                DOB = x.DOB,
                Phone = x.Phone
            }).ToList();
            // var result = Mapper.Map<List<MemberProfile>, List<MembersDto>>(data);
            return a;

        }
        public MembersDto GetMemberById(long id)
        {
            MembersDto m = new MembersDto();
            var data = _repositoryBase.GetById(id);
            m.Id = data.Id;
            m.FirstName = data.FirstName;            
            m.LastName = data.LastName;
            m.DOB = data.DOB;
            m.Email = data.Email;
            m.BirthPlace = data.BirthPlace;
            m.Nationality = data.Nationality;
            m.LanguageID = (int)data.LanguageID;
            m.Phone = data.Phone;
            m.GSM = data.GSM;
            m.Street = data.Street;
            m.PostalCode = data.PostalCode;
            m.Commune = data.Commune;
            m.PrivateAddress = data.PrivateAddress;
            m.PrivatePostalCode = data.PrivatePostalCode;
            m.StatusID = (int)data.StatusID;


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
            return m;

        }
        public object AddMember(RequestMemberDto membersDto)
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
                StatusID = membersDto.StatusID,
                StatusIDPublic = true,
                AddedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                MemberSpecializations = (ICollection<MemberSpecialization>)membersDto.Specialization,
                MemberTranieeCommissions = (ICollection<MemberTranieeCommission>)membersDto.TranieeCommission,
            };
            var result = _repositoryBase.Insert(data);
            _repositoryBase.Save();
            return result;

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
            data.StatusID = membersDto.StatusID;
            // data.StatusIDPublic = membersDto.StatusIDPublic,
            data.MemberSpecializations = (ICollection<MemberSpecialization>)membersDto.Specialization;
            data.MemberTranieeCommissions = (ICollection<MemberTranieeCommission>)membersDto.TranieeCommission;
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
    }
}
