using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

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
    }
}
