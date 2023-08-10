using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace PITB.CMS_Common.Models
{
    public partial class DBContextHelperLinq : DbContext
    {
        //public DBContextHelperLinq() : base("name=PITB.CMS")
        public DBContextHelperLinq() : base(Config.ConnectionString/*Utility.GetDecryptedString(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString)*/)
        {
            Configuration.LazyLoadingEnabled = true;
        }
        public virtual DbSet<DbCampaign> DbCampaigns { get; set; }
        public virtual DbSet<DbComplaintSubType> DbComplaints_SubType { get; set; }

        public virtual DbSet<DbComplaintType> DbComplaints_Type { get; set; }
        public virtual DbSet<DbUsers> DbUsers { get; set; }
        public virtual DbSet<DbStatus> DbStatuses { get; set; }

        public virtual DbSet<DbDistrict> DbDistricts { get; set; }
        public virtual DbSet<DbDivision> DbDivisions { get; set; }
        public virtual DbSet<DbProvince> DbProvinces { get; set; }
        public virtual DbSet<DbTehsil> DbTehsils { get; set; }
        public virtual DbSet<DbUnionCouncils> DbUnionCouncils { get; set; }

        public virtual DbSet<DbHierarchyCampaignGroupMapping> DbHierarchyCampaignGroupMappings { get; set; }

        public virtual DbSet<DbCategoryGroupMapping> DbCategoryGroupMappings { get; set; }

        public virtual DbSet<DbWards> DbWards { get; set; }

        public virtual DbSet<DbComplaint> DbComplaints { get; set; }
        public virtual DbSet<DbComplaintCallLogs> DbComplaint_Call_Logs { get; set; }
        public virtual DbSet<DbPersonInformation> DbPersonalInfo { get; set; }

        public virtual DbSet<DbAssignmentMatrix> DbAssignmentMatrix { get; set; }

        public virtual DbSet<DbDynamicCategories> DbDynamicCategories { get; set; }
        public virtual DbSet<DbDynamicFormControls> DbDynamicForm { get; set; }

        public virtual DbSet<DbDynamicComplaintFields> DbDynamicComplaintFields { get; set; }

        public virtual DbSet<DbDynamicCategoriesMapping> DbDynamicCategoriesMapping { get; set; }

        public virtual DbSet<DbHierarchy> DbHierarchy { get; set; }

        public virtual DbSet<DbComplaintTransferLog> DbComplaintTransferLog { get; set; }

        public virtual DbSet<DbPermissions> DbPermissions { get; set; }
        public virtual DbSet<DbPermissionsAssignment> DbPermissionsAssignments { get; set; }

        public virtual DbSet<DbAttachments> DbAttachments { get; set; }

        public virtual DbSet<DbComplaintStatusChangeLog> DbComplaintStatusChangeLog { get; set; }

        public virtual DbSet<DbComplaintFollowupLogs> DbComplaintFollowupLogs { get; set; }

        public virtual DbSet<DbDepartmentCategory> DbDepartmentCategory { get; set; }

        public virtual DbSet<DbDepartmentSubCategory> DbDepartmentSubCategory { get; set; }

        public virtual DbSet<DbCallTagging> DbCallTagging { get; set; }

        public virtual DbSet<DbDepartment> DbDepartment { get; set; }

        public virtual DbSet<DbIncomingMessages> DbIncomingMessages { get; set; }

        public virtual DbSet<DbNotificationLogs> DbNotificationLogs { get; set; }

        public virtual DbSet<DbUserWiseNotification> DbUserWiseNotification { get; set; }

        public virtual DbSet<DbReplyMessages> DbReplyMessages { get; set; }

        public virtual DbSet<DbSchoolsMapping> DbSchoolsMapping { get; set; }
        public virtual DbSet<DbComplaintVote> DbComplaintVotes { get; set; }

        public virtual DbSet<DbUniqueIncrementor> DbUniqueIncrementor { get; set; }

        public virtual DbSet<DbSchoolEducationHeadMapping> DbSchoolEducationHeadMapping { get; set; }

        //public virtual DbSet<DbUserCategory> DbUserCategory { get; set; }

        public virtual DbSet<DbUserCategory> DbUserCategory { get; set; }

        public virtual DbSet<UserCategory> UserCategory { get; set; }

        public virtual DbSet<DbComplaintsOriginLog> DbComplaintsOriginLog { get; set; }

        public virtual DbSet<DbUserWiseComplaints> DbUserWiseComplaints { get; set; }


        public virtual DbSet<DbWindowServiceError> DbWindowServiceError { get; set; }

        public virtual DbSet<DbCrmIdsMappingToOtherSystem> DbCrmIdsMappingToOtherSystem { get; set; }

        public virtual DbSet<DbUserWiseSupervisorMapping> DbUserWiseSupervisorMapping { get; set; }


        //public virtual DbSet<DbFormControls> DbFormControls { get; set; }

        public virtual DbSet<DbFormControl> DbFormControl { get; set; }

        public virtual DbSet<DbFormPermissionsAssignment> DbFormPermissionAssignment { get; set; }

        public virtual DbSet<DbCategory> DbCategory { get; set; }

        public virtual DbSet<DbCategoryMapping> DbCategoryMapping { get; set; }

        public virtual DbSet<DbHealthDistricts> DbHealthDistricts { get; set; }
        public virtual DbSet<DbHealthTehsil> DbHealthTehsil { get; set; }
        public virtual DbSet<DbHealthComplaintCategories> DbHealthComplaintCategories { get; set; }
        public virtual DbSet<DbHealthComplaintSubCategories> DbHealthComplaintSubCategories { get; set; }
        public virtual DbSet<DbHealthDepartments> DbHealthDepartments { get; set; }

        public virtual DbSet<DbMobileRequest> DbMobileRequest { get; set; }

        public virtual DbSet<DbPoliceAction> DbPoliceAction { get; set; }
        public virtual DbSet<DbPoliceActionReportLogs> DbPoliceActionReportLogs { get; set; }

        public virtual DbSet<DbUserWiseLogs> DbUserWiseLogs { get; set; }

        public virtual DbSet<DbCampaignMessages> DbCampaignMessages { get; set; }

        public virtual DbSet<DbComplainantFeedbackLog> DbComplainantFeedbackLog { get; set; }

        public virtual DbSet<DbMobileNotificationLogs> DbMobileNotificationLogs { get; set; }

        public virtual DbSet<DbUserWiseDevices> DbUserWiseDevices { get; set; }

        public virtual DbSet<DbCampaignWiseCallLogs> DbCampaignWiseCallLogs { get; set; }

        public virtual DbSet<DbConfiguration_Assignment> DbConfiguration_Assignment { get; set; }

        public virtual DbSet<DbTranslationMapping> DbTranslationMapping { get; set; }

        public virtual DbSet<DbTagLookup> DbTagLookup { get; set; }
        public virtual DbSet<DbPublicUserOTP> DbPublicUserOTP { get; set; }
        public virtual DbSet<DbCampaignDepartment> DbCampaignDepartment { get; set; }

        public virtual DbSet<DbSearchCampaign> DbSearchCampaign { get; set; }

        public virtual DbSet<DbClientSystem> DbClientSystem { get; set; }



        #region from API
        public virtual DbSet<DbApiRequestLogs> DbApiRequestLogs { get; set; }
        public virtual DbSet<DbLinearIncrement> DbLinearIncrement { get; set; }
        public virtual DbSet<DbSchoolCategoryUserMapping> DbSchoolCategoryUserMapping { get; set; }
        public virtual DbSet<DbSocialSharing> DbSocialSharings { get; set; }
        public virtual DbSet<DbUsersVersionMapping> DbUsersVersionMapping { get; set; }
        public virtual DbSet<DbVersion> DbVersion { get; set; }
        #endregion
        public static void UpdateEntity<TEntity>(TEntity entity, DBContextHelperLinq db = null, List<string> listEntities = null /*, params Expression<Func<TEntity, TProperty>>[] properties*/) where TEntity : class //where TProperty:class 
        {
            try
            {
                bool alreadyInDB = db.ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);

                if (!alreadyInDB)
                {
                    db.Set<TEntity>().Attach(entity);
                }


                if (listEntities.Count > 0)
                {
                    foreach (string val in listEntities)
                    {
                        //Expression<Antlr.Runtime.Misc.Func<DbComplaint, string>> getProperty = n => n.Person_Cnic;
                        //DbComplaint dbComplaint = null;
                        db.Entry(entity).Property(val).IsModified = true;
                    }
                }
                else
                {
                    db.Entry(entity).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {

            }

        }


        //public static void UpdateEntity<TEntity,TProperty>(TEntity entity, DBContextHelperLinq db = null, params Expression<Func<TEntity, TProperty>>[] properties) where TEntity : class where TProperty:class 
        //{

        //    db.Set<TEntity>().Attach(entity);

        //    if (properties.Length > 0)
        //    {
        //        foreach (var val in properties)
        //        {
        //            Expression<Func<DbComplaint, string>> getProperty = n => n.Person_Cnic;
        //            DbComplaint dbComplaint = null;
        //            //db.Entry(entity).Property(getProperty);

        //            //var entityEntry = db.Entry(entity);
        //            //var propertyEntry = entityEntry.Property(getProperty);
        //            //propertyEntry.IsModified = false;


        //            db.Entry(dbComplaint).Reference(getProperty).IsModified = true;
        //            //db.Entry(entity).Property(n=>n).IsModified = true;
        //            db.Entry(dbComplaint).Property (n => n.Person_Cnic).IsModified = true;
        //        }
        //    }
        //    else
        //    {
        //        db.Entry(entity).State = EntityState.Modified;
        //    }

        //}

        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUsers>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<DbUsers>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }*/
    }
}
