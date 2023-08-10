using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Permissions_Assignment")]
    public partial class DbPermissionsAssignment
    {
        [Key]
        public int Id { get; set; }

        public int? Type { get; set; }

        public int? Type_Id { get; set; }

        public int? Permission_Id { get; set; }

        public string Permission_Value { get; set; }

        //public virtual DbPermissions DbPermissions { get; set; }

    }
}
