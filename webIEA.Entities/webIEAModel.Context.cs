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
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<MemberDocument> MemberDocuments { get; set; }
        public DbSet<MemberProfile> MemberProfiles { get; set; }
        public DbSet<MemberSpecialization> MemberSpecializations { get; set; }
        public DbSet<MemberStatu> MemberStatus { get; set; }
        public DbSet<MemberTranieeCommission> MemberTranieeCommissions { get; set; }
        public DbSet<TrainingCours> TrainingCourses { get; set; }
    }
}
