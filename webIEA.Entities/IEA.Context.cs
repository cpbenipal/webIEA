﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webIEA.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class webIEAEntities : DbContext
    {
        public webIEAEntities()
            : base("name=webIEAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CourseLanguage> CourseLanguages { get; set; }
        public DbSet<HistoryDataChanx> HistoryDataChanges { get; set; }
        public DbSet<mCourseType> mCourseTypes { get; set; }
        public DbSet<MemberDocument> MemberDocuments { get; set; }
        public DbSet<MemberProfile> MemberProfiles { get; set; }
        public DbSet<MemberSpecialization> MemberSpecializations { get; set; }
        public DbSet<MemberTranieeCommission> MemberTranieeCommissions { get; set; }
        public DbSet<mEmploymentStatu> mEmploymentStatus { get; set; }
        public DbSet<mStatu> mStatus { get; set; }
        public DbSet<TrainingCours> TrainingCourses { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
