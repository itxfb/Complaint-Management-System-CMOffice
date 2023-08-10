using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Complaints_Status_Change_Log")]
    public class DbComplaintStatusChangeLog
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? StatusChangedByUserId { get; set; }

        public int? Complaint_Id { get; set; }

        public int? StatusId { get; set; }

        public DateTime StatusChangeDateTime { get; set; }

        public string Comments { get; set; }

        public bool IsCurrentlyActive { get; set; }

        [ForeignKey("StatusChangedByUserId")]
        public virtual DbUsers ChangedBy { get; set; }

        [ForeignKey("StatusId")]
        public virtual DbStatus DbStatus { get; set; }
        

        [NotMapped]
        public virtual List<DbAttachments> ListDbAttachments { get; set; }


        public static DbComplaintStatusChangeLog GetLastStatusChangeOfParticularComplaint(int complaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintStatusChangeLog.Where(m => m.Complaint_Id == complaintId && m.IsCurrentlyActive == true).OrderBy(m => m.StatusChangeDateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbComplaintStatusChangeLog GetLastStatusChangeOfParticularComplaint(int complaintId, DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintStatusChangeLog.Where(m => m.Complaint_Id == complaintId && m.IsCurrentlyActive == true).OrderBy(m => m.StatusChangeDateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbComplaintStatusChangeLog> GetStatusChangeLogsAgainstComplaintId(int Complaint_Id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbComplaintStatusChangeLog> listStatusChangeLogs = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id).OrderBy(n => n.StatusChangeDateTime).ToList();
                return listStatusChangeLogs;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static DbComplaintStatusChangeLog GetActiveStatusChangeLogAgainstComplaintId(int Complaint_Id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbComplaintStatusChangeLog statusChangeLog = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.IsCurrentlyActive == true).ToList().FirstOrDefault();
                return statusChangeLog;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<DbComplaintStatusChangeLog> GetStatusChangeLogsAgainstComplaintIds(List<int?> listComplaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbComplaintStatusChangeLog> listStatusChangeLogs = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => listComplaintId.Contains(n.Complaint_Id)).Include(n => n.ChangedBy).OrderBy(n => n.StatusChangeDateTime).ToList();
                //DBContextHelperLinq db2 = new DBContextHelperLinq();

                List<DbAttachments> listDbAttachments =
                    db.DbAttachments.Where(n => listComplaintId.Contains(n.Complaint_Id) && n.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                        .ToList();
                foreach (DbComplaintStatusChangeLog dbStatusChangeLog in listStatusChangeLogs)
                {
                    dbStatusChangeLog.ListDbAttachments =
                        listDbAttachments.Where(n => n.ReferenceTypeId == dbStatusChangeLog.Id).ToList();
                }
                return listStatusChangeLogs;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<DbComplaintStatusChangeLog> StatusChangeLogsListAgainstComplaintId(int Complaint_Id, int statusId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbComplaintStatusChangeLog> listStatusChangeLog = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.StatusId == statusId).ToList();
                return listStatusChangeLog;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        

    }
}