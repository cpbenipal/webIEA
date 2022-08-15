using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Contracts
{
    public interface IMemberDocumentManager
    {
        object Add(MemberDocument model);
        object Update(MemberDocument model);
        MemberDocumentDto GetById(long id);
        MemberDocument GetFirstById(long Id);
        List<MemberDocumentDto> GetAll();
        List<MemberDocumentDto> GetAllFiltered(long Id);
        object Delete(int Id);
    }
}
