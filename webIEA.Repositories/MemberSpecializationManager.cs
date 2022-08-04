
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
    public class MemberSpecializationManager : IMemberSpecializationManager
    {
        private readonly IRepositoryBase<MemberSpecialization> _repositoryBase; 
        private readonly Mapper mapper;

        public MemberSpecializationManager(IRepositoryBase<MemberSpecialization> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        public object Add(MemberSpecializationDto memberSpecializationDto)
        {
            var data = new MemberSpecialization
            {
                MemberID = memberSpecializationDto.MemberId,
                SpecializationName = memberSpecializationDto.SpecializationName
            };
            var result = _repositoryBase.Insert(data);
            return result;
        }
        public object Update(MemberSpecializationDto memberSpecializationDto)
        {
            var data = _repositoryBase.GetById(memberSpecializationDto.Id);
            data.SpecializationName = memberSpecializationDto.SpecializationName;
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;
        }
        public MemberSpecializationDto GetById(long Id)
        {
            MemberSpecializationDto memberSpecializationDto = new MemberSpecializationDto();
            var model = _repositoryBase.GetById(Id);
            memberSpecializationDto.Id = model.Id;
            memberSpecializationDto.SpecializationName = model.SpecializationName;
            return memberSpecializationDto;
        }
        public List<MemberSpecializationDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new MemberSpecializationDto
            {
                Id = x.Id,
                SpecializationName = x.SpecializationName,
            }).ToList();
            return data;
        }
        public object Delete(int Id)
        {
            var model = _repositoryBase.GetById(Id);
            _repositoryBase.Delete(model);
            _repositoryBase.Save();
            return "";
        }
    }
}
