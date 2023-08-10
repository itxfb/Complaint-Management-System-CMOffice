using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{
    [Table("PITB.Complaints_Status_Change_Log")]
    public partial class DbComplaintStatusChangeLog
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
    }
}