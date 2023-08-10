using PITB.CMS_Models.DB;
using System;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{
    public class RepoDbComplaintFollowupLogs
    {
        public static DbComplaintFollowupLogs GetLastStatusChangeOfParticularComplaint(int complaintId, DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintFollowupLogs.Where(m => m.Complaint_Id == complaintId && m.IsCurrentlyActive == true).OrderBy(m => m.FollowupDateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}