using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using PITB.CRM_API.Helper.Database;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.MobileRequests")]
    public partial class DbMobileRequest
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? ComplaintId { get; set; }

        public int? RequestType { get; set; }

        public int? RequestTypeId { get; set; }

        public string Imei { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public Int64 ApiRequestId { get; set; }

    }
}