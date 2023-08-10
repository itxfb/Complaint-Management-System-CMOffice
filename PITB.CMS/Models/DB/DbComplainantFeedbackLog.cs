using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using PITB.CMS.Helper.Database;
using PITB.CMS.Helper;
using PITB.CMS.Models.Custom;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Complainant_Feedback_Log")]
    public partial class DbComplainantFeedbackLog
    {
        public int Id { get; set; }

        public int? SubmittedByUserId { get; set; }

        public int? ComplainantId { get; set; }

        public int? ComplaintId { get; set; }

        public int? CampaignId { get; set; }

        public int? ConferenceCallArrangedId { get; set; }

        public string ConferenceCallArrangedIdStr { get; set; }

        public int? CallWithin08HoursStatusId { get; set; }

        [StringLength(50)]
        public string CallWithin08HoursStatusIdStr { get; set; }

        [StringLength(500)]
        public string CallWithin08HoursStatusComments { get; set; }

        public int? MeetingWithin24HoursStatusId { get; set; }

        [StringLength(50)]
        public string MeetingWithin24HoursStatusIdStr { get; set; }

        [StringLength(500)]
        public string MeetingWithin24HoursStatusComments { get; set; }

        public int? ProceedingWithin48HoursStatusId { get; set; }

        [StringLength(50)]
        public string ProceedingWithin48HoursStatusIdStr { get; set; }

        [StringLength(500)]
        public string ProceedingWithin48HoursStatusComments { get; set; }

        public int? FeedbackStatusId { get; set; }

        [StringLength(50)]
        public string FeedbackStatusIdStr { get; set; }

        [StringLength(500)]
        public string FeedbackStatusComments { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public bool? IsCurrentlyActive { get; set; }


        public static DbComplainantFeedbackLog InsertLog(DbComplainantFeedbackLog dbComplainantFeedbackLog)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                MakeLastLogOfComplaintStatusInactive((int)dbComplainantFeedbackLog.ComplaintId, db);
                db.DbComplainantFeedbackLog.Add(dbComplainantFeedbackLog);
                db.SaveChanges();
            }
            return dbComplainantFeedbackLog;
        }

        public static void InsertLog(DbComplainantFeedbackLog dbComplainantFeedbackLog, DBContextHelperLinq db)
        {    
            MakeLastLogOfComplaintStatusInactive((int)dbComplainantFeedbackLog.ComplaintId, db);
            db.DbComplainantFeedbackLog.Add(dbComplainantFeedbackLog);
        }

        private static void MakeLastLogOfComplaintStatusInactive(int complaintId, DBContextHelperLinq db)
        {
            //DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplainantFeedbackLog feedbackLog = DbComplainantFeedbackLog.GetLastComplainantFeedbackLogOfParticularComplaint(complaintId, db);
            if (feedbackLog != null)
            {
                feedbackLog.IsCurrentlyActive = false;
                db.DbComplainantFeedbackLog.Add(feedbackLog);
                db.Entry(feedbackLog).State = EntityState.Modified;
                //db.SaveChanges();
            }
        }

        public static DbComplainantFeedbackLog GetLastComplainantFeedbackLogOfParticularComplaint(int complaintId, DBContextHelperLinq _db = null)
        {
            DBContextHelperLinq db = (_db == null) ? new DBContextHelperLinq() : _db;
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
               
                    return
                        db.DbComplainantFeedbackLog.Where(
                            m => m.ComplaintId == complaintId && m.IsCurrentlyActive == true)
                            .OrderBy(m => m.CreatedDateTime)
                            .ToList()
                            .LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally 
            {
                if (_db == null)
                {
                    db.Dispose();
                }
            }
        }

        
    }
}
