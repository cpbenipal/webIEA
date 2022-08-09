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
        List<MemberProfile> GetAllMembers();
        long AddMember(RequestMemberDto requestMemberDto);
        MemberProfile GetMemberById(long id);
        object UpdateMember(MembersDto membersDto);
        object UpdateStatus(long Id, string FieldName, bool check);
        object UpdateMemberStatus(long Id, string FieldName, int status);
    }
}
