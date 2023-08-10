using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Hierarchy")]
    public partial class DbHierarchy
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(200)]
        public string HierarchyName { get; set; }
    }
}
