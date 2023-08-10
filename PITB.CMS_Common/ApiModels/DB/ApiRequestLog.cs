namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

     [Table("PITB.ApiRequestLogs")]
    public partial class ApiRequestLog
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(4000)]
        public string JsonString { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [StringLength(100)]
        public string IpAddress { get; set; }

        [StringLength(1000)]
        public string Exception { get; set; }

        public bool? IsValid { get; set; }
    }
}
