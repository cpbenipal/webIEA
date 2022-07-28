
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
    public class SpecializationManager : IMemberSpecialization
    {
        private readonly IRepositoryBase<Specialization> _repositoryBase; 
        private readonly Mapper mapper;

        public SpecializationManager(IRepositoryBase<Specialization> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        public List<SpecializationDto> GetAllSpecialization()
        {
            var data = _repositoryBase.GetAll().ToList();
            var a = data.Select(x => new SpecializationDto()
            {
                Id = x.Id,
                Name = x.Name,                
            }).ToList();            
            return a;
        }
        public SpecializationDto GetSpecializationById(int id)
        { 
            var model = _repositoryBase.GetById(id);  
            return new SpecializationDto()
            {
                Id = model.Id,
                Name = model.Name,
            };
        }  
    }
}
