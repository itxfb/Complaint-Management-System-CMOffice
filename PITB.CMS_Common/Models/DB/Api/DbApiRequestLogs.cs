using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;
//using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.ApiRequestLogs")]
    public partial class DbApiRequestLogs
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public string RequestUrl { get; set; }

        public string RequestHeaders { get; set; }

        //[StringLength(4000)]
        public string JsonString { get; set; }

        public string Response { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [StringLength(100)]
        public string IpAddress { get; set; }

        [StringLength(1000)]
        public string Exception { get; set; }

        public bool? IsValid { get; set; }
    }
}