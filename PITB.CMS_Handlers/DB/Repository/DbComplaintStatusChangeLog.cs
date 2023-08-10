using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbComplaintStatusChangeLog
    {
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