using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using webIEA.Entities;

namespace webIEA.DataBaseContext
{
    public partial class WebIEAContext : DbContext
    {
        public WebIEAContext()
            : base("name=MainConnectionString")
        {
        }

       // public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MemberProfile> MemberProfile { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
