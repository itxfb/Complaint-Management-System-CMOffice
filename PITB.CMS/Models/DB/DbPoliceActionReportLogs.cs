using System.Data.Entity;
using System.Linq;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Police_Action_Report_Logs")]
    public partial class DbPoliceActionReportLogs
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? Complaint_Id { get; set; }

        public DateTime? ProceedingDateTime { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        //[StringLength(500)]
        public string Comments { get; set; }

        public int? AttachmentRefId { get; set; }

        public Config.PoliceActionReportType? Type { get; set; }

        public string Filter1Name { get; set; }

        public int? Filter1 { get; set; }

        public string Filter2Name { get; set; }

        public int? Filter2 { get; set; }

        public string Filter3Name { get; set; }

        public int? Filter3 { get; set; }

        [StringLength(200)]
        public string TagId { get; set; }

        

        [ForeignKey("AttachmentRefId")]
        public virtual DbAttachments DbAttachment { get; set; }


        public static List<DbPoliceActionReportLogs> GetActionReportLogs(int complaintId, int filter1)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbPoliceActionReportLogs.Where(n => n.Complaint_Id == complaintId && n.Filter1 == filter1).Include(n=>n.DbAttachment).ToList();
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
