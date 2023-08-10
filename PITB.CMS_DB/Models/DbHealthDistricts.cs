using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Health_Districts")]
    public partial class DbHealthDistricts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int District_ID { get; set; }

        [StringLength(50)]
        public string District_Name { get; set; }

        [StringLength(20)]
        public string District_Code { get; set; }

        public int Region_Id { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(100)]
        public string Urdu_Name { get; set; }

        [ForeignKey("DbDistrict")]
        public int? Crm_Dist { get; set; }

        public virtual DbDistrict DbDistrict { get; set; }
    }
}
