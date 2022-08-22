using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class HistoryChangesManager: IHistoryChangesManager
    {
        private RepositoryBase<HistoryDataChanx> repositoryBase;  

        public HistoryChangesManager(RepositoryBase<HistoryDataChanx> repositoryBase)
        {
            this.repositoryBase = repositoryBase; 
        }

        public List<HistoryDataChanx> GetAll()
        {
            return repositoryBase.GetAll().ToList(); 
        }
        public List<HistoryDataChanx> GetHistoryDetail(string pk,DateTime date)
        {
            return repositoryBase.GetAllFiltered(x=>x.PK==pk && x.UpdateDate.Year==date.Year && x.UpdateDate.Month == date.Month && x.UpdateDate.Day == date.Day).ToList();
        }
        //public List<HistoryDataChanx> GetTranieeHistory(string pk, DateTime date)
        //{
        //    return repositoryBase.GetAllFiltered(x => x.PK == pk && x.UpdateDate.Year == date.Year && x.UpdateDate.Month == date.Month && x.UpdateDate.Day == date.Day).ToList();
        //}
        //public List<HistoryDataChanx> GeSpecializationtHistory(string pk, DateTime date)
        //{
        //    return repositoryBase.GetAllFiltered(x => x.NewValue == pk && x.UpdateDate.Year == date.Year && x.UpdateDate.Month == date.Month && x.UpdateDate.Day == date.Day).ToList();
        //}
    }
}