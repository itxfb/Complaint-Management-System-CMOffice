using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{


    public class RepoDbComplaintCallLogs
    {
        public static List<DbComplaintCallLogs> GetByComplaintId(int complaintId, DBContextHelperLinq db)
        {
            try
            {
                return db.DbComplaint_Call_Logs.Where(m => m.complaint_id == complaintId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
