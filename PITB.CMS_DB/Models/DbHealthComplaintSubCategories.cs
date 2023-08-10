using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Health_Complaint_SubCategories")]
    public partial class DbHealthComplaintSubCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubCategory_Id { get; set; }

        [StringLength(500)]
        public string SubCategory_Name { get; set; }

        public int? Category_Id { get; set; }

        [StringLength(500)]
        public string Urdu_Name { get; set; }

        public byte? Priority { get; set; }

        public int? Level0 { get; set; }

        public int? Level1 { get; set; }

        public int? Level2 { get; set; }

        public int? Level3 { get; set; }

        public int? Level4 { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbComplaintSubType")]
        public int? CrmId { get; set; }

        public virtual DbComplaintSubType DbComplaintSubType { get; set; }
    }
}
