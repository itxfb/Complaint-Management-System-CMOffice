using System.Data.Entity;
using System.Linq;
using System;

namespace PITB.CMS_Common.Models
{

    public partial class DbComplainantFeedbackLog
    {
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
