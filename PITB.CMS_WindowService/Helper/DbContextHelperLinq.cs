using System.Configuration;
using PITB.CMS.Models.DB;
using PITB.CMS_WindowService.Models.DB;

namespace PITB.CMS_WindowService.Helper
{
    using System.Data.Entity;
    class DbContextHelperLinq : DbContext
    {
        public DbContextHelperLinq()
            : base("name=PITB.CMS")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        //public virtual DbSet<DbWindowServiceError> DbWindowServiceErrors { get; set; }
    }
}
