//using AutoMapper;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class HistoryChangesInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;        
        public HistoryChangesInteractor(IRepositoryWrapper _repositoryWrapper) 
        { 
            repositoryWrapper = _repositoryWrapper;            
        }
          
        public List<HistoryDataChanx> GetAll()
        {
            return repositoryWrapper.HistoryChangesManager.GetAll();

        }

        public List<HistoryChangesListDto> GetMemberHistoryLogs(long MemberId)
        {
            var history = GetHistory(MemberId, "MemberProfile");

            return history.Select(x=> new HistoryChangesListDto()
            {
                ID = x.ID,
                UpdateDate = ((DateTime)x.UpdateDate).ToShortDateString()
            }).ToList();            
        }
        private List<HistoryDataChanx> GetHistory(long MemberId, string TableName)   
        {
            return repositoryWrapper.HistoryChangesManager.GetAll().Where(x=>x.PK == Convert.ToString(MemberId) && x.TableName == TableName && x.Type == "U").ToList();
        }
    }
}
