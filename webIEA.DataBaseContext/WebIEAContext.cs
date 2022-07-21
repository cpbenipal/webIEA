using Microsoft.Ajax.Utilities;
using System.Data.Entity;

namespace webIEA.DataBaseContext
{
    public partial class WebIEAContext : DbContext
    {
        public WebIEAContext()
            : base("name=WebIEAContext1")
        {
        }

        public virtual DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
