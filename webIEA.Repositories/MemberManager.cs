
using AutoMapper;
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
            var result = Mapper.Map<List<MemberProfile>, List<MembersDto>>(data);
            return result;

        }
        public MembersDto GetMemberById(long id)
        {
            var data = _repositoryBase.GetById(id);
            var result = Mapper.Map<MembersDto>(data);
            return result;

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
                MemberSpecializations= (ICollection<MemberSpecialization>)membersDto.Specialization,
                MemberTranieeCommissions= (ICollection<MemberTranieeCommission>)membersDto.TranieeCommission,
            };
           var result= _repositoryBase.Insert(data);
            _repositoryBase.Save();
            return result;

        }
        public object UpdateMember(MembersDto membersDto)
        {
            var data = new MemberProfile
            {
                Id = membersDto.Id,
                FirstName = membersDto.FirstName,
                FirstNamePublic = membersDto.FirstNamePublic,
                LastName = membersDto.LastName,
                LastNamePublic = membersDto.LastNamePublic,
                DOB = membersDto.DOB,
                DOBPublic = membersDto.DOBPublic,
                Email = membersDto.Email,
                EmailPublic = membersDto.EmailPublic,
                BirthPlace = membersDto.BirthPlace,
                BirthPlacePublic = membersDto.BirthPlacePublic,
                Nationality = membersDto.Nationality,
                NationalityPublic = membersDto.NationalityPublic,
                LanguageID = membersDto.LanguageID,
                LanguageIDPublic = membersDto.LanguageIDPublic,
                Phone = membersDto.Phone,
                PhonePublic = membersDto.PhonePublic,
                GSM = membersDto.GSM,
                GSMPublic = membersDto.GSMPublic,
                Street = membersDto.Street,
                StreetPublic = membersDto.StreetPublic,
                PostalCode = membersDto.PostalCode,
                PostalCodePublic = membersDto.PostalCodePublic,
                Commune = membersDto.Commune,
                CommunePublic = membersDto.CommunePublic,
                PrivateAddress = membersDto.PrivateAddress,
                PrivateAddressPublic = membersDto.PrivateAddressPublic,
                PrivatePostalCode = membersDto.PrivatePostalCode,
                PrivatePostalCodePublic = membersDto.PrivateAddressPublic,
                StatusID = membersDto.StatusID,
                StatusIDPublic = membersDto.StatusIDPublic,
                MemberSpecializations = (ICollection<MemberSpecialization>)membersDto.Specialization,
                MemberTranieeCommissions = (ICollection<MemberTranieeCommission>)membersDto.TranieeCommission,
            };
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;

        }
    }
}
