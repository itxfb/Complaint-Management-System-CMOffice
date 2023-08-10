using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Health_Departments")]
    public class DbHealthDepartments
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Department_Name { get; set; }

        public int? District_Id { get; set; }

        public byte? SystemId { get; set; }

        [StringLength(500)]
        public string Urdu_Name { get; set; }

        public bool? IsSpecial { get; set; }

        [StringLength(10)]
        public string Province_Id { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbDepartment")]
        public int? CrmId { get; set; }
    }
}
