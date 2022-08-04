
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
        private IMemberSpecializationManager _specialmanager;

        public IMemberSpecializationManager MemberSpecializationManager

        {
            get
            {
                if (_manager == null)
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
                if (_manager == null)
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
                if (_manager == null)
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
                if (_manager == null)
                {
                    var repositoryBase = new RepositoryBase<MemberEmploymentStatu>(_context);
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
                if (_manager == null)
                {
                    var repositoryBase = new RepositoryBase<CourseType>(_context);
                    _CourseType = new CourseTypeManager(repositoryBase);
                }
                return _CourseType;
            }
        }


       
    }
}
