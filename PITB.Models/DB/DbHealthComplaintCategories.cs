using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PITB.CMS_Models.DB
{
    [Table("PITB.Health_Complaint_Categories")]
    public class DbHealthComplaintCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Category_Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? DepartmentAreaId { get; set; }

        public byte? SystemId { get; set; }

        [StringLength(500)]
        public string Urdu_Name { get; set; }

        public int? ResponsibleId { get; set; }

        public byte? ResponsibleType { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbComplaintType")]
        public int? CrmId { get; set; }

        public virtual DbComplaintType DbComplaintType { get; set; }
    }
}
