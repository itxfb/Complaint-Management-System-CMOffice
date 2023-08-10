using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{
    [Table("PITB.Statuses")]
    public partial class DbStatus
    {
        [Key]
        [Column("Id")]
        public int Complaint_Status_Id { get; set; }
        //[Column("Name")]
        public string Status { get; set; }
        public string Campaigns { get; set; }

        public bool? IsEscalatable { get; set; }

        public int? EscalationStatus { get; set; }

        public bool? ForceEscalateOnStatusChange { get; set; }

        public string EscalationAction { get; set; }
    }
}