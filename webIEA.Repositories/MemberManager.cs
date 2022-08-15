
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
        private readonly Mapper mapper;

        public MemberManager(IRepositoryBase<MemberProfile> repositoryBase)
        {
            var _mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<MemberProfile, MembersDto>());
            mapper= new Mapper(_mapConfig);
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
        public long AddMember(MemberProfile memberProfile)
        {           
            var result = _repositoryBase.Insert(memberProfile);
            _repositoryBase.Save();
            return result.Id;

        }
        public object UpdateMember(MemberProfile memberProfile)
        {            
            var result = _repositoryBase.Update(memberProfile);
            _repositoryBase.Save();
            return result;

        }
        public MemberProfile UpdateStatus(MemberProfile memberProfile)
        { 
            var result = _repositoryBase.Update(memberProfile);
            _repositoryBase.Save();
            return result;

        }
        //public object UpdateMemberStatus(long Id, string FieldName, int status)
        //{
        //    var data = _repositoryBase.GetById(Id);
        //    data.StatusID = status;
        //    var result = _repositoryBase.Update(data);
        //    _repositoryBase.Save();
        //    return result;

        //}
    }
}
