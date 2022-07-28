
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
    public class MemberStatusManager : IMemberStatusManager
    {
        private readonly IRepositoryBase<MemberStatu> _repositoryBase; 
        private readonly Mapper mapper;

        public MemberStatusManager(IRepositoryBase<MemberStatu> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        public List<StatusDto> GetAllStatus()
        {
            var data = _repositoryBase.GetAll().ToList();
            var a = data.Select(x => new StatusDto()
            {
                ID = x.ID,
                StatusName = x.StatusName,                
            }).ToList();            
            return a;
        }
        public StatusDto GetStatusById(int id)
        { 
            var model = _repositoryBase.GetById(id);  
            return new StatusDto()
            {
                ID = model.ID,
                StatusName = model.StatusName,
            };
        }  
    }
}
