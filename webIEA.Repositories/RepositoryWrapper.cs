
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly WebIEAContext2 _context;
        private readonly IHashManager _hashManager;
        public RepositoryWrapper(WebIEAContext2 context, IHashManager hashManager)
        {
            _context = context; _hashManager = hashManager;
        }


        private IUnitOfWork _unitOfWork;

        public IUnitOfWork UnitOfWorkManager
        {
            get
            {

                if (_unitOfWork == null)
                {
                    ////var repositoryBase = new RepositoryBase<T>(_context);
                    //_unitOfWork.GetRepository<T>();
                }
                return _unitOfWork;
            }
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
        private IStatusManager _statusmanager;

        public IStatusManager StatusManager
        {
            get
            {
                if (_statusmanager == null)
                {
                    var repositoryBase = new RepositoryBase<mStatu>(_context);
                    _statusmanager = new StatusManager(repositoryBase);
                }
                return _statusmanager;
            }
        }
        private IMemberSpecializationManager _specialmanager;

        public IMemberSpecializationManager MemberSpecializationManager

        {
            get
            {
                if (_specialmanager == null)
                {
                    var repositoryBase = new RepositoryBase<MemberSpecialization>(_context);
                    _specialmanager = new MemberSpecializationManager(repositoryBase);
                }
                return _specialmanager;
            }
        }
        private ICourseMemberManager _CourseMembermanager;

        public ICourseMemberManager CourseMemberManager
        {
            get
            {
                if (_CourseMembermanager == null)
                {
                    var repositoryBase = new RepositoryBase<MemberTranieeCommission>(_context);
                    _CourseMembermanager = new CourseMemberManager(repositoryBase);
                }
                return _CourseMembermanager;
            }
        }
        private ITraineeCourseManager _TraineeCourseManager;

        public ITraineeCourseManager TraineeCourseManager
        {
            get
            {
                if (_TraineeCourseManager == null)
                {
                    var repositoryBase = new RepositoryBase<TrainingCours>(_context);
                    _TraineeCourseManager = new TraineeCourseManager(repositoryBase);
                }
                return _TraineeCourseManager;
            }
        }
        private IEmploymentStatusManager _EmploymentStatusManager;

        public IEmploymentStatusManager EmploymentStatusManager
        {
            get
            {
                if (_EmploymentStatusManager == null)
                {
                    var repositoryBase = new RepositoryBase<mEmploymentStatu>(_context);
                    _EmploymentStatusManager = new EmploymentStatusManager(repositoryBase);
                }
                return _EmploymentStatusManager;
            }
        }
        private ICourseTypeManager _CourseType;

        public ICourseTypeManager CourseTypeManager
        {
            get
            {
                if (_CourseType == null)
                {
                    var repositoryBase = new RepositoryBase<mCourseType>(_context);
                    _CourseType = new CourseTypeManager(repositoryBase);
                }
                return _CourseType;
            }
        }
        private IMemberDocumentManager _memberDocumentManager;

        public IMemberDocumentManager MemberDocumentManager
        {
            get
            {
                if (_memberDocumentManager == null)
                {
                    var repositoryBase = new RepositoryBase<MemberDocument>(_context);
                    _memberDocumentManager = new MemberDocumentManager(repositoryBase);
                }
                return _memberDocumentManager;
            }
        }
        private IAccountManager _accountManager;

        public IAccountManager AccountManager
        {
            get
            {
                if (_accountManager == null)
                {
                    var repositoryBase = new RepositoryBase<User>(_context);
                    _accountManager = new AccountManager(repositoryBase, _hashManager);
                }
                return _accountManager;
            }
        }

        private IHistoryChangesManager _historyManager; 

        public IHistoryChangesManager HistoryChangesManager 
        {
            get
            {
                if (_historyManager == null)
                {
                    var repositoryBase = new RepositoryBase<HistoryDataChanx>(_context);
                    _historyManager = new HistoryChangesManager(repositoryBase); 
                }
                return _historyManager;
            }
        }

    }
}
