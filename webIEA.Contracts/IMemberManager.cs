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
        long AddMember(MemberProfile memberProfile);
        MemberProfile GetMemberById(long id);
        MemberProfile UpdateMember(MemberProfile membersDto);         
        MemberProfile GetMemberByEmail(string Email);
        void DeleteMember(long Id); 
    }
}
