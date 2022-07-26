
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly WebIEAContext _context;

        public RepositoryWrapper(WebIEAContext context)
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
    }
}
