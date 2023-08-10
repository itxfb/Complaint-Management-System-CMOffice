using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Handler.Business;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    [Table("PITB.Complaints_Origin_Log")]
    public class DbComplaintsOriginLog
    {
        public int Id { get; set; }

        public int? Complaint_Id { get; set; }

        public int? Origin_HierarchyId { get; set; }

        public int? Origin_UserHierarchyId { get; set; }

        public int? Origin_UserCategoryId1 { get; set; }

        public int? Origin_UserCategoryId2 { get; set; }

        public int? Source_Id { get; set; }

        public int? Event_Id { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public bool? Is_AssignedToOrigin { get; set; }

        public bool? IsCurrentlyActive { get; set; }



        public static void SaveComplaintsOriginHistoryLog(DbComplaintsOriginLog dbComplaintsOriginLog, DBContextHelperLinq db)
        {
                //DBContextHelperLinq db = new DBContextHelperLinq();

                MakeLastLogOfComplaintStatusInactive(dbComplaintsOriginLog, db);
                DbComplaintsOriginLog dbStatusChangeLog = SaveComplaintsOriginLog(dbComplaintsOriginLog, db);

                //BlNotification.ResetNotification(campaignId,Config.NotificationType.Complaint, Config.NotificationSubType.Launch, complaintId, Config.NotificationStatus.Send);
                //db.SaveChanges();
        }


        private static void MakeLastLogOfComplaintStatusInactive(DbComplaintsOriginLog dbComplaintsOriginLog, DBContextHelperLinq db)
        {
            //DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplaintsOriginLog statusChangeLog = DbComplaintsOriginLog.GetComplaintsOriginLog(dbComplaintsOriginLog, db);
            if (statusChangeLog != null)
            {
                statusChangeLog.IsCurrentlyActive = false;
                db.DbComplaintsOriginLog.Add(statusChangeLog);
                db.Entry(statusChangeLog).State = EntityState.Modified;
                //db.SaveChanges();
            }
        }

        public static DbComplaintsOriginLog GetComplaintsOriginLog(DbComplaintsOriginLog dbComplaintsOriginLog, DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintsOriginLog.Where(m => m.Complaint_Id == dbComplaintsOriginLog.Complaint_Id && m.IsCurrentlyActive == true).OrderBy(m => m.Created_DateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static DbComplaintsOriginLog SaveComplaintsOriginLog(DbComplaintsOriginLog dbComplaintsOriginLogToCopy, DBContextHelperLinq db)
        {
            DbComplaintsOriginLog dbComplaintOriginLog = new DbComplaintsOriginLog();
            dbComplaintOriginLog.Complaint_Id = dbComplaintsOriginLogToCopy.Complaint_Id;
            dbComplaintOriginLog.Origin_HierarchyId = dbComplaintsOriginLogToCopy.Origin_HierarchyId;
            dbComplaintOriginLog.Origin_UserHierarchyId = dbComplaintsOriginLogToCopy.Origin_UserHierarchyId;
            dbComplaintOriginLog.Origin_UserCategoryId1 = dbComplaintsOriginLogToCopy.Origin_UserCategoryId1;
            dbComplaintOriginLog.Origin_UserCategoryId2 = dbComplaintsOriginLogToCopy.Origin_UserCategoryId2;
            dbComplaintOriginLog.Source_Id = dbComplaintsOriginLogToCopy.Source_Id;

            dbComplaintOriginLog.Event_Id = dbComplaintsOriginLogToCopy.Event_Id;
            dbComplaintOriginLog.Created_DateTime = dbComplaintsOriginLogToCopy.Created_DateTime;
            dbComplaintOriginLog.Is_AssignedToOrigin = dbComplaintsOriginLogToCopy.Is_AssignedToOrigin;
            dbComplaintOriginLog.IsCurrentlyActive = dbComplaintsOriginLogToCopy.IsCurrentlyActive;

            db.DbComplaintsOriginLog.Add(dbComplaintOriginLog);
            //db.SaveChanges();
            return dbComplaintOriginLog;
        }
    }
}