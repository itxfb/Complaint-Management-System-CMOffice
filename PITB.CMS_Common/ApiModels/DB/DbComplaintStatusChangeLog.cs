using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    using PITB.CRM_API.Helper.Database;
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
        public virtual DbStatuses DbStatus { get; set; }
        
        [NotMapped]
        public virtual List<DbAttachments> ListDbAttachments { get; set; }


        public static DbComplaintStatusChangeLog GetAllStatusChangeLogByLogId(int logId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return
                    db.DbComplaintStatusChangeLog.FirstOrDefault(m => m.Id == logId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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

        public static List<DbComplaintStatusChangeLog> GetStatusChangeLogsAgainstComplaintIds(List<int?> listComplaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbComplaintStatusChangeLog> listStatusChangeLogs = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => listComplaintId.Contains(n.Complaint_Id)).Include(n=>n.ChangedBy).OrderBy(n => n.StatusChangeDateTime).ToList();
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

        public static List<int> GetActiveStatusChangeLogIdsListAgainstComplaintId(int Complaint_Id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<int> listStatusChangeLogIds = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.IsCurrentlyActive == true).Select(n => n.Id).ToList();
                return listStatusChangeLogIds;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<int> LwmcGetStatusChangeLogIdsListAgainstComplaintId(int Complaint_Id,int statusId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<int> listStatusChangeLogIds = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.StatusId == statusId).Select(n => n.Id).ToList();
                return listStatusChangeLogIds;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static DbComplaintStatusChangeLog GetLastStatusChangeOfParticularComplaintWhereCommentsAreNotNull(int complaintId, DBContextHelperLinq db=null)
        {
            try
            {
                if(db ==null ) db= new DBContextHelperLinq();

                return db.DbComplaintStatusChangeLog
                    .Where(
                        m => m.Complaint_Id == complaintId && 
                            m.Comments != "")
                    .OrderByDescending(m => m.StatusChangeDateTime)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbComplaintStatusChangeLog> GetStatusChangeLogByUsername(string username,int from,int to, DBContextHelperLinq db = null)
        {
            try
            {
                if (db == null) db = new DBContextHelperLinq();
                var dbUser = DbUsers.GetByUserName(username);
                if (dbUser == null) return null;


                return db.DbComplaintStatusChangeLog
                    .Where(m => m.StatusChangedByUserId == dbUser.Id)
                    .OrderByDescending(m => m.StatusChangeDateTime)
                    .Skip(from)
                    .Take(to)
                    .ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static int GetStatusChangeLogByUsernameCount(string username, DBContextHelperLinq db = null)
        {
            try
            {
                if (db == null) db = new DBContextHelperLinq();
                var dbUser = DbUsers.GetByUserName(username);
                if (dbUser == null) return 0;


                return db.DbComplaintStatusChangeLog
                    .Count(m => m.StatusChangedByUserId == dbUser.Id);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}