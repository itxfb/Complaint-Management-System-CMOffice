using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Department_SubCategory")]
    public partial class DbDepartmentSubCategory
    {
        [Key]
        public int Id { get; set; }

        [StringLength(4000)]
        public string Name { get; set; }

        public int? Dep_Cat_Id { get; set; }
    }
}