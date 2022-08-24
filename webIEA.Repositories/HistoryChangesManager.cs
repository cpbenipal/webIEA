using System;
using System.Collections.Generic;
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
            return repositoryBase.GetAllFiltered(x=>x.PK==pk && x.Type == "U" && x.UpdateDate.Value.Year==date.Year && x.UpdateDate.Value.Month == date.Month && x.UpdateDate.Value.Day == date.Day).ToList();
        }
        public List<int?> GetMemberTranieeHistory(string pk, DateTime date)
        {
            return repositoryBase.GetAllFiltered(x => x.PK == pk && x.FieldName== "TrainingCourseId" && x.TableName== "MemberTranieeCommission" && x.Type=="I" && x.UpdateDate.Value.Year == date.Year && x.UpdateDate.Value.Month == date.Month && x.UpdateDate.Value.Day == date.Day).Select(x=>x.NewValue).Select(s => Int32.TryParse(s, out int n) ? n : (int?)null).ToList();
        }
        public List<string> GetMemberSpecializationtHistory(string pk, DateTime date)
        {
            return repositoryBase.GetAllFiltered(x => x.PK == pk && x.FieldName == "SpecializationName" && x.TableName == "MemberSpecialization" && x.Type == "I" && x.UpdateDate.Value.Year == date.Year && x.UpdateDate.Value.Month == date.Month && x.UpdateDate.Value.Day == date.Day).Select(x => x.NewValue).ToList();
        }
    }
}