using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Tehsil")]
    public partial class DbTehsil
    {
        [Key]
        [Column("Id")]
        public int Tehsil_Id { get; set; }

        [NotMapped]
        public int Id { get; set; }

        [StringLength(50)]
        public string Tehsil_Name { get; set; }

        public int? District_Id { get; set; }

        public int? Group_Id { get; set; }

        public bool? Is_Active { get; set; }
    }
}
