using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PITB.CMS_Common.Models
{

    [Table("PITB.WindowServiceError")]
    public partial class DbWindowServiceError
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        public string StackTrace { get; set; }

        public int ErrorId { get; set; }

        public string ErrorStr { get; set; }

        public int ServiceType { get; set; }

        public DateTime? CreationDateTime { get; set; }
    }
}
