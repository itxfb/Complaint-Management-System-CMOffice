namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.User_Wise_Logs")]
    public partial class DbUserWiseLogs
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? Complaint_Id { get; set; }

        public int? ModuleId { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [StringLength(200)]
        public string TagId { get; set; }

        public bool? IsCurrentlyActive { get; set; }
    }
}
