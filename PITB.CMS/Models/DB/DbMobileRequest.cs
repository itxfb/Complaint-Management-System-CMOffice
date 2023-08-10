using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using PITB.CRM_API.Helper.Database;
    using System.Linq;

    [Table("PITB.MobileRequests")]
    public class DbMobileRequest
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