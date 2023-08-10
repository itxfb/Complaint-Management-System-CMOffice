using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_DB.Models
{

    public partial class DbPoliceActionReportLogs
    {
        public static List<DbPoliceActionReportLogs> GetActionReportLogs(int complaintId, int filter1)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbPoliceActionReportLogs.Where(n => n.Complaint_Id == complaintId && n.Filter1 == filter1).Include(n => n.DbAttachment).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //public static List<DbPoliceActionReportLogs> GetStatusChangeLogsAgainstComplaintId(int Complaint_Id)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();
        //        List<DbPoliceActionReportLogs> listStatusChangeLogs = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id).OrderBy(n => n.StatusChangeDateTime).ToList();
        //        return listStatusChangeLogs;
        //        //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //}
    }
}
