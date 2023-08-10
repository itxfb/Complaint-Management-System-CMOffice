using System;
using System.Linq;

namespace PITB.CMS_Common.Models
{
    public partial class DbComplaintFollowupLogs
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