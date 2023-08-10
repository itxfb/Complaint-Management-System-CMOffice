using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Complaints_Followup_Log")]
    public class DbComplaintFollowupLogs
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? FollowupMarkedByUserId { get; set; }

        public int? Complaint_Id { get; set; }

        public int? FollowupId { get; set; }

        public DateTime FollowupDateTime { get; set; }

        public string Comments { get; set; }

        public bool IsCurrentlyActive { get; set; }

        public static DbComplaintFollowupLogs GetLastStatusChangeOfParticularComplaint(int complaintId, DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbComplaintFollowupLogs.Where(m => m.Complaint_Id == complaintId && m.IsCurrentlyActive == true).OrderBy(m => m.FollowupDateTime).ToList().LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}