using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Helper.Database
{
    using System.Data.Entity;

    public class DBContextHelperLinq : DbContext
    {
        public DBContextHelperLinq()
            : base("name=PITB.CMSApi")
        {
            Configuration.LazyLoadingEnabled = true;
        }
        
        //public virtual DbSet<DbCampaign> DbCampaigns { get; set; }
        public virtual DbSet<DbComplaintSubType> DbComplaints_SubType { get; set; }
        public virtual DbSet<DbComplaintType> DbComplaints_Type { get; set; }

        public virtual DbSet<DbComplaintStatusChangeLog> DbComplaintStatusChangeLog { get; set; }

        public virtual DbSet<DbUsers> DbUsers { get; set; }
        //public virtual DbSet<DbUsers> DbUsers { get; set; }
        //public virtual DbSet<DbStatus> DbStatuses { get; set; }

        public virtual DbSet<DbCampaign> DbCampaigns { get; set; }

        public virtual DbSet<DbStatuses> DbStatuses { get; set; }

        public virtual DbSet<DbDistrict> DbDistricts { get; set; }
        public virtual DbSet<DbDivision> DbDivisions { get; set; }
        public virtual DbSet<DbProvince> DbProvinces { get; set; }
        public virtual DbSet<DbTehsil> DbTehsils { get; set; }
        public virtual DbSet<DbUnionCouncils> DbUnionCouncils { get; set; }

        public virtual DbSet<DbWards> DbWards { get; set; }

        public virtual DbSet<DbHierarchyCampaignGroupMapping> DbHierarchyCampaignGroupMappings { get; set; }

        public virtual DbSet<DbComplaint> DbComplaints { get; set; }

        public virtual DbSet<DbPersonInformation> DbPersonalInfo { get; set; }

        public virtual DbSet<DbAssignmentMatrix> DbAssignmentMatrix { get; set; }

        public virtual DbSet<DbAttachments> DbAttachments { get; set; }

        public virtual DbSet<DbMobileRequest> DbMobileRequest { get; set; }

        public virtual DbSet<DbApiRequestLogs> DbApiRequestLogs { get; set; }

        public virtual DbSet<DbIncomingMessages> DbIncomingMessages { get; set; }

        public virtual DbSet<DbReplyMessages> DbReplyMessages { get; set; }

        public virtual DbSet<DbLinearIncrement> DbLinearIncrement { get; set; }

        public virtual DbSet<DbTranslationMapping> DbTranslationMapping { get; set; }

        public virtual DbSet<DbVersion> DbVersion { get; set; }

        public virtual DbSet<DbUsersVersionMapping> DbUsersVersionMapping { get; set; }
        public virtual DbSet<DbComplaintVote> DbComplaintVotes { get; set; }
        public virtual DbSet<DbSocialSharing> DbSocialSharings { get; set; }

        public virtual DbSet<DbSchoolCategoryUserMapping> DbSchoolCategoryUserMapping { get; set; }

        //public virtual DbSet<DbSchoolsMapping> DbSchoolsMapping { get; set; }
        //public virtual DbSet<DbPermissions> DbPermissionses { get; set; }

        public virtual DbSet<DbPermissionsAssignment> DbPermissionsAssignments { get; set; }

        public virtual DbSet<DbPermissions> DbPermissions { get; set; }
        
        /*
        public virtual DbSet<DbPersonInformation> DbPersonalInfo { get; set; }

        public virtual DbSet<DbAssignmentMatrix> DbAssignmentMatrix { get; set; }

        public virtual DbSet<DbDynamicCategories> DbDynamicCategories { get; set; }
        public virtual DbSet<DbDynamicFormControls> DbDynamicForm { get; set; }

        public virtual DbSet<DbDynamicComplaintFields> DbDynamicComplaintFields { get; set; }

        public virtual DbSet<DbHierarchy> DbHierarchy { get; set; }

        public virtual DbSet<DbComplaintTransferLog> DbComplaintTransferLog { get; set; }

        public virtual DbSet<DbPermissions> DbPermissions { get; set; }
        public virtual DbSet<DbPermissionsAssignment> DbPermissionsAssignments { get; set; }
        */
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<DbUsers>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<DbUsers>()
                .Property(e => e.Password)
                .IsUnicode(false);
            */

           /* modelBuilder.Entity<DbAttachments>()
                .HasRequired<DbComplaintStatusChangeLog>(s => s.DbComplaintStatusChangeLog)
                .WithMany(g => g.ListDbAttachments)
                .HasForeignKey<int?>(s => s.ReferenceTypeId);*/
        }
    }
}
