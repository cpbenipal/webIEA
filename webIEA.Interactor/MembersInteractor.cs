using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class MembersInteractor 
    { 
        private readonly IRepositoryWrapper repositoryWrapper;

        public MembersInteractor(IRepositoryWrapper _repositoryWrapper)
        { 
            repositoryWrapper = _repositoryWrapper;
        }
         
        public List<MembersDto> GetAllMembers()
        { 
            return repositoryWrapper.MemberManager.GetAllMembers(); 
        }
        public MembersDto GetMemberById(long id)
        { 
            return repositoryWrapper.MemberManager.GetMemberById(id);
        }
        public object AddMember(RequestMemberDto membersDto)
        {            
            return repositoryWrapper.MemberManager.AddMember(membersDto);
        }
        public object UpdateMember(MembersDto membersDto)
        {             
            return repositoryWrapper.MemberManager.UpdateMember(membersDto);
        }
    }
}
