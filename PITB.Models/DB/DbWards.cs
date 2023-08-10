using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Wards")]
    public class DbWards
    {
        [Key]
        [Column("id")]
        public int Ward_Id { get; set; }

        public int Uc_Id { get; set; }

        public string Wards_Name { get; set; }

        public int? Group_Id { get; set; }

        public bool? Is_Active { get; set; }
    }
}