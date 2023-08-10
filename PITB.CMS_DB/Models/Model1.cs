namespace PITB.CMS.Models.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<DbCampaignWiseCallLogs> Campaign_Wise_Call_Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCampaignWiseCallLogs>()
                .Property(e => e.Phone_No)
                .IsUnicode(false);
        }
    }
}
