using System;
using System.Collections.Generic;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Contracts
{
    public interface IHistoryChangesManager
    {
        List<HistoryDataChanx> GetAll();
        List<HistoryDataChanx> GetHistoryDetail(string pk, DateTime date);
        List<string> GetMemberSpecializationtHistory(string pk, DateTime date);
        List<int?> GetMemberTranieeHistory(string pk, DateTime date);
        void DeleteMemberHistory(long id);
    }
}