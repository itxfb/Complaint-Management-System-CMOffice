using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    [Table("Complaint_Votes", Schema = "PITB")]
    public class DbComplaintVote
    {
        public int Id { get; set; }
        public long Complaint_ID { get; set; }
        public byte Is_Positive_Vote { get; set; }
        public DateTime Vote_DateTime { get; set; }
        public string Voted_By { get; set; }
        public string Voted_By_Provider { get; set; }

        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime? Updated_Date_Time { get; set; }
        public string Cnic { get; set; }
        public string Cell_Number { get; set; }

        #region Helpers

        public static List<DbComplaintVote> GetVotesByComplaintId(long complaintId)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaintVotes.AsNoTracking().Where(m => m.Complaint_ID == complaintId).ToList();
            }
        }
        #endregion
    }




}