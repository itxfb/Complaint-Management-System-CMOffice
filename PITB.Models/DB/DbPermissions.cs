using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Permissions")]
    public class DbPermissions
    {
        [Key]
        [Column("Id")]
        public int Permission_Id { get; set; }

        public int Permission_Type { get; set; }

        [StringLength(200)]
        public string Permissions_Name { get; set; }

        public string Permissions_Value { get; set; }
    }
}
