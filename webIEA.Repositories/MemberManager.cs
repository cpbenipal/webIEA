
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class MemberManager : IMemberManager
    {
        private readonly IRepositoryBase<MemberProfile> _repositoryBase;

        public MemberManager(IRepositoryBase<MemberProfile> repositoryBase) 
        {
            _repositoryBase = repositoryBase;
        }

        public List<MemberProfile> GetAllMembers()
        {
            return _repositoryBase.GetAll().ToList();
        }
    }
}
