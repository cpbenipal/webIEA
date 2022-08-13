using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using webIEA.Entities;

namespace webIEA.DataBaseContext
{    
    public partial class WebIEAContext2 : DbContext
    {
        public WebIEAContext2()
            : base("name=webIEAEntities")
        {
        }
         
        public virtual DbSet<MemberProfile> MemberProfile { get; set; }
        public virtual DbSet<MemberStatu> MemberStatus { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
