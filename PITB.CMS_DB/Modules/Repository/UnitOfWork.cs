using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using PITB.CMS_Common.Models;
//using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common;
using PITB.CMS_DB.Models;
using PITB.CMS_DB.Modules.Repository.Db;

namespace PITB.CMS_DB.Modules.Repository
{
    public class UnitOfWork : /*DbContext,*/ IDisposable
    {
        private CMSDbContext dbContext;
        //private GenericRepository<DbCampaign> dbCampaign;
        //private RepDbCampaign<DbCampaign> dbCampaign2;
        private dynamic d;
        private DynamicObject asd;

        public RepoDbCampaign DbCampaign
        {
            get 
            {
                if(d.DbCampaign == null) 
                {
                    d.DbCampaign = new RepoDbCampaign(dbContext, this);
                }
                return (RepoDbCampaign) d.DbCampaign;
            }
            set { d.DbCampaign = value; }
        }
        public RepoDbPermissionsAssignment DbPermissionsAssignment
        {
            get
            {
                if (d.RepoDbPermissionsAssignment == null)
                {
                    d.RepoDbPermissionsAssignment = new RepoDbPermissionsAssignment(dbContext);
                }
                return (RepoDbPermissionsAssignment)d.RepoDbPermissionsAssignment;
            }
            set { d.RepoDbPermissionsAssignment = value; }
        }


        private DbContextTransaction dbContextTransaction;

        //public virtual DbSet<DbCampaign> DbCampaign { get; set; }
        //public virtual DbSet<DbComplaintSubType> DbComplaints_SubType { get; set; }

        //public virtual DbSet<DbComplaintType> DbComplaints_Type { get; set; }
        //public virtual DbSet<DbUsers> DbUsers { get; set; }
        //public virtual DbSet<DbStatus> DbStatuses { get; set; }

        //public virtual DbSet<DbDistrict> DbDistricts { get; set; }
        //public virtual DbSet<DbDivision> DbDivisions { get; set; }
        //public virtual DbSet<DbProvince> DbProvinces { get; set; }
        //public virtual DbSet<DbTehsil> DbTehsils { get; set; }
        //public virtual DbSet<DbUnionCouncils> DbUnionCouncils { get; set; }

        //public virtual DbSet<DbHierarchyCampaignGroupMapping> DbHierarchyCampaignGroupMappings { get; set; }

        //public virtual DbSet<DbCategoryGroupMapping> DbCategoryGroupMappings { get; set; }

        //public virtual DbSet<DbWards> DbWards { get; set; }

        //public virtual DbSet<DbComplaint> DbComplaints { get; set; }
        //public virtual DbSet<DbComplaintCallLogs> DbComplaint_Call_Logs { get; set; }
        //public virtual DbSet<DbPersonInformation> DbPersonalInfo { get; set; }

        //public virtual DbSet<DbAssignmentMatrix> DbAssignmentMatrix { get; set; }

        //public virtual DbSet<DbDynamicCategories> DbDynamicCategories { get; set; }
        //public virtual DbSet<DbDynamicFormControls> DbDynamicForm { get; set; }

        //public virtual DbSet<DbDynamicComplaintFields> DbDynamicComplaintFields { get; set; }

        //public virtual DbSet<DbDynamicCategoriesMapping> DbDynamicCategoriesMapping { get; set; }

        //public virtual DbSet<DbHierarchy> DbHierarchy { get; set; }

        //public virtual DbSet<DbComplaintTransferLog> DbComplaintTransferLog { get; set; }

        //public virtual DbSet<DbPermissions> DbPermissions { get; set; }
        //public virtual DbSet<DbPermissionsAssignment> DbPermissionsAssignments { get; set; }

        //public virtual DbSet<DbAttachments> DbAttachments { get; set; }

        //public virtual DbSet<DbComplaintStatusChangeLog> DbComplaintStatusChangeLog { get; set; }

        //public virtual DbSet<DbComplaintFollowupLogs> DbComplaintFollowupLogs { get; set; }

        //public virtual DbSet<DbDepartmentCategory> DbDepartmentCategory { get; set; }

        //public virtual DbSet<DbDepartmentSubCategory> DbDepartmentSubCategory { get; set; }

        //public virtual DbSet<DbCallTagging> DbCallTagging { get; set; }

        //public virtual DbSet<DbDepartment> DbDepartment { get; set; }

        //public virtual DbSet<DbIncomingMessages> DbIncomingMessages { get; set; }

        //public virtual DbSet<DbNotificationLogs> DbNotificationLogs { get; set; }

        //public virtual DbSet<DbUserWiseNotification> DbUserWiseNotification { get; set; }

        //public virtual DbSet<DbReplyMessages> DbReplyMessages { get; set; }

        //public virtual DbSet<DbSchoolsMapping> DbSchoolsMapping { get; set; }
        //public virtual DbSet<DbComplaintVote> DbComplaintVotes { get; set; }

        //public virtual DbSet<DbUniqueIncrementor> DbUniqueIncrementor { get; set; }

        //public virtual DbSet<DbSchoolEducationHeadMapping> DbSchoolEducationHeadMapping { get; set; }

        ////public virtual DbSet<DbUserCategory> DbUserCategory { get; set; }

        //public virtual DbSet<DbUserCategory> DbUserCategory { get; set; }

        //public virtual DbSet<UserCategory> UserCategory { get; set; }

        //public virtual DbSet<DbComplaintsOriginLog> DbComplaintsOriginLog { get; set; }

        //public virtual DbSet<DbUserWiseComplaints> DbUserWiseComplaints { get; set; }


        //public virtual DbSet<DbWindowServiceError> DbWindowServiceError { get; set; }

        //public virtual DbSet<DbCrmIdsMappingToOtherSystem> DbCrmIdsMappingToOtherSystem { get; set; }

        //public virtual DbSet<DbUserWiseSupervisorMapping> DbUserWiseSupervisorMapping { get; set; }


        ////public virtual DbSet<DbFormControls> DbFormControls { get; set; }

        //public virtual DbSet<DbFormControl> DbFormControl { get; set; }

        //public virtual DbSet<DbFormPermissionsAssignment> DbFormPermissionAssignment { get; set; }

        //public virtual DbSet<DbCategory> DbCategory { get; set; }

        //public virtual DbSet<DbCategoryMapping> DbCategoryMapping { get; set; }

        //public virtual DbSet<DbHealthDistricts> DbHealthDistricts { get; set; }
        //public virtual DbSet<DbHealthTehsil> DbHealthTehsil { get; set; }
        //public virtual DbSet<DbHealthComplaintCategories> DbHealthComplaintCategories { get; set; }
        //public virtual DbSet<DbHealthComplaintSubCategories> DbHealthComplaintSubCategories { get; set; }
        //public virtual DbSet<DbHealthDepartments> DbHealthDepartments { get; set; }

        //public virtual DbSet<DbMobileRequest> DbMobileRequest { get; set; }

        //public virtual DbSet<DbPoliceAction> DbPoliceAction { get; set; }
        //public virtual DbSet<DbPoliceActionReportLogs> DbPoliceActionReportLogs { get; set; }

        //public virtual DbSet<DbUserWiseLogs> DbUserWiseLogs { get; set; }

        //public virtual DbSet<DbCampaignMessages> DbCampaignMessages { get; set; }

        //public virtual DbSet<DbComplainantFeedbackLog> DbComplainantFeedbackLog { get; set; }

        //public virtual DbSet<DbMobileNotificationLogs> DbMobileNotificationLogs { get; set; }

        //public virtual DbSet<DbUserWiseDevices> DbUserWiseDevices { get; set; }

        //public virtual DbSet<DbCampaignWiseCallLogs> DbCampaignWiseCallLogs { get; set; }

        //public virtual DbSet<DbConfiguration_Assignment> DbConfiguration_Assignment { get; set; }

        //public virtual DbSet<DbTranslationMapping> DbTranslationMapping { get; set; }

        public UnitOfWork()
        {
            dbContext = new CMSDbContext();
            d = new DynamicDictionary();
            //Configuration.LazyLoadingEnabled = true;
        }

        //public string GetRepository<T>() where T:class
        //{
        //    //T t = (T)Activator.CreateInstance(typeof(T));
        //    Type t = typeof(T);
        //    Dictionary<string,PropertyInfo> dictAttr = t.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance).ToDictionary(x=>x.GetType,y=>y);
            
        //    if (null != prop && prop.CanWrite)
        //    {
        //        prop.SetValue(obj, defaultValue.Value, null);
        //    }
        //    return null;
        //}
        //public GenericRepository<DbCampaign> DbCampaign
        //{
        //    get
        //    {
        //        if (this.dbCampaign == null)
        //        {
        //            this.dbCampaign = new GenericRepository<DbCampaign>(dbContext);
        //            //dbContext.Add(this.dbCampaign);
        //        }
        //        return dbCampaign;
        //    }
        //}

        public void CreateTransaction()
        {
            dbContextTransaction = dbContext.Database.BeginTransaction();
        }
        //If all the Transactions are completed successfuly then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            dbContextTransaction.Commit();
        }
        //If atleast one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            dbContextTransaction.Rollback();
            dbContextTransaction.Dispose();
        }

        

        private bool isDisposed = false;
        protected void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    dbContext.Dispose();
                }
            }
            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
        }
    }
}
