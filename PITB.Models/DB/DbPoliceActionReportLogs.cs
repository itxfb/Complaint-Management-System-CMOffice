using PITB.CMS_Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Police_Action_Report_Logs")]
    public class DbPoliceActionReportLogs
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
    }
}
