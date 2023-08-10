using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;

namespace PITB.CMS_Common.Models
{


    public partial class DbComplaintCallLogs
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
