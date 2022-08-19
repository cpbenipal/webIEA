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
    }
}