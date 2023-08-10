using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Attachments")]
    public partial class DbAttachments
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? Complaint_Id { get; set; }

        public int? Source_Id { get; set; }

        public string Source_Url { get; set; }

        public Decimal? ApiRequestId { get; set; }

        public int? ReferenceType { get; set; }

        public int? ReferenceTypeId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FileContentType { get; set; }

        public int? FileType { get; set; }

        public string TagId { get; set; }
    }
}