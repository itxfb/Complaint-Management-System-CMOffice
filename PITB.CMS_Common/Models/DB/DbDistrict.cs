using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Districts")]
    public partial class DbDistrict
    {
        [Key]
        [Column("id")]
        public int District_Id { get; set; }

        [NotMapped]
        public int id { get; set; }

        [StringLength(50)]
        public string District_Name { get; set; }

        public int? Division_Id { get; set; }

        [StringLength(10)]
        public string District_Short { get; set; }

        public int? Group_Id { get; set; }

        [NotMapped]
        public string District_UrduName { get; set; }

        public bool? Is_Active { get; set; }
    }


}


