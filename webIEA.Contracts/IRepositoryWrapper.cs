﻿namespace webIEA.Contracts
{
    public interface IRepositoryWrapper
    {
       IMemberManager MemberManager { get; }
       IMemberStatusManager MemberStatusManager { get; } 
        IMemberSpecializationManager MemberSpecializationManager { get; }
        ICourseMemberManager CourseMemberManager { get; }
        ITraineeCourseManager TraineeCourseManager { get; }
        ICourseTypeManager CourseTypeManager { get; }
        IEmploymentStatusManager EmploymentStatusManager { get; }
    }
}
