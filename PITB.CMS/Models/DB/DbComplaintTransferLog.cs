namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Complaints_Transfer_Log")]
    public partial class DbComplaintTransferLog
    {
        public int Id { get; set; }

        public int? Complaint_Id { get; set; }

        [Column("AssignedFromUserId")]
        public int? User_Id { get; set; }

        public int? AssignedFromMedium { get; set; }

        public int? AssignedFromMediumValue { get; set; }

        public int? AssignedToMedium { get; set; }

        public int? AssignedToMediumValue { get; set; }

        public DateTime? AssignmentDateTime { get; set; }

        public bool? IsCurrentlyActive { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public virtual DbUsers User { get; set; }

        public static DbComplaintTransferLog GetLastTransferOfParticularComplaint(int complaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintTransferLog.Where(m => m.Complaint_Id == complaintId && m.IsCurrentlyActive == true).OrderBy(m => m.AssignmentDateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbComplaintTransferLog GetLastTransferOfParticularComplaint(int complaintId,DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintTransferLog.Where(m => m.Complaint_Id==complaintId && m.IsCurrentlyActive==true).OrderBy(m=>m.AssignmentDateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static List<DbComplaintTransferLog> GetTransferLogsAgainstComplaintId(int Complaint_Id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbComplaintTransferLog> listComplaints = db.DbComplaintTransferLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id).OrderBy(n => n.AssignmentDateTime).ToList();
                return listComplaints;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static DbComplaintTransferLog GetActiveTransferLogAgainstComplaintId(int Complaint_Id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbComplaintTransferLog dbComplaint = db.DbComplaintTransferLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.IsCurrentlyActive==true).ToList().FirstOrDefault();
                return dbComplaint;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
