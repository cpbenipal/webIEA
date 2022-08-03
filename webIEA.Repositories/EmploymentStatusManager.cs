﻿
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
    public class EmploymentStatusManager : IEmploymentStatusManager
    {
        private readonly IRepositoryBase<MemberEmploymentStatu> _repositoryBase; 
        private readonly Mapper mapper;

        public EmploymentStatusManager(IRepositoryBase<MemberEmploymentStatu> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        public object Add(EmploymentStatusDto employmentStatusDto)
        {
            var data = new MemberEmploymentStatu
            {
                StatusName = employmentStatusDto.StatusName,
            };
            var result = _repositoryBase.Insert(data);
            return result;
        }
        public object Update(EmploymentStatusDto employmentStatusDto)
        {
            var data = _repositoryBase.GetById(employmentStatusDto.Id);
            data.StatusName = employmentStatusDto.StatusName;
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;
        }
        public EmploymentStatusDto GetById(long Id)
        {
            EmploymentStatusDto employmentStatusDto = new EmploymentStatusDto();
            var model = _repositoryBase.GetById(Id);
            employmentStatusDto.Id = model.Id;
            employmentStatusDto.StatusName = model.StatusName;
            return employmentStatusDto;
        }
        public List<EmploymentStatusDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new EmploymentStatusDto
            {
                Id = x.Id,
                StatusName = x.StatusName,
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
