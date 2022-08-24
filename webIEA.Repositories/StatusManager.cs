﻿using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class StatusManager : IStatusManager
    {
        private readonly IRepositoryBase<mStatu> _repositoryBase; 

        public StatusManager(IRepositoryBase<mStatu> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        public object Add(StatusDto model)
        {
            var data = new mStatu
            {
                StatusName = model.StatusName,
            };
            var result = _repositoryBase.Insert(data);
            return result;
        }
        public object Update(StatusDto model)
        {
            var data = _repositoryBase.GetById(model.ID);
            data.StatusName = model.StatusName;
            var result = _repositoryBase.Update(data);
            _repositoryBase.Save();
            return result;
        }
        public StatusDto GetById(long Id)
        {
            StatusDto statusDto = new StatusDto();
            var model = _repositoryBase.GetById(Id);
            statusDto.ID = model.ID;
            statusDto.StatusName = model.StatusName;
            return statusDto;
        }
        public List<StatusDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new StatusDto
            {
                ID = x.ID,
                StatusName = x.StatusName,
            }).ToList();
            return data;
        }
        public object Delete(int Id)
        {
            _repositoryBase.Delete(Id);
            _repositoryBase.Save();
            return "";
        }
    }
}
