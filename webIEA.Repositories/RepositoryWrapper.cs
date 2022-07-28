
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly WebIEAContext2 _context;

        public RepositoryWrapper(WebIEAContext2 context)
        {
            _context = context;
        }

        private IMemberManager _manager;

        public IMemberManager MemberManager 
        {
            get
            {

                if (_manager == null)
                {
                    var repositoryBase = new RepositoryBase<MemberProfile>(_context);
                    _manager = new MemberManager(repositoryBase);
                }
                return _manager;
            }
        }
        private IMemberStatusManager _statusmanager; 

        public IMemberStatusManager MemberStatusManager 
        {
            get
            {
                if (_manager == null)
                {
                    var repositoryBase = new RepositoryBase<MemberStatu>(_context);
                    _statusmanager = new MemberStatusManager(repositoryBase);
                }
                return _statusmanager;
            }
        }
    }
}
