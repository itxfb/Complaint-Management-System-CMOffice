using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

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
    }
}