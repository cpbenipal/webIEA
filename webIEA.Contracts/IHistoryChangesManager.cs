using System.Collections.Generic;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Contracts
{
    public interface IHistoryChangesManager
    {
        List<HistoryDataChanx> GetAll();
    }
}