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
    public interface IMemberManager
    {
        List<MembersDto> GetAllMembers();
        object AddMember(RequestMemberDto requestMemberDto);
        MembersDto GetMemberById(long id);
        object UpdateMember(MembersDto membersDto);
        object UpdateStatus(long Id, string FieldName, bool check);
    }
}
