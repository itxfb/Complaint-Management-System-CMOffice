using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Divisions")]
    public partial class DbDivision
    {
        [Key]
        [Column("Id")]
        public int Division_Id { get; set; }

        [StringLength(50)]
        public string Division_Name { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(20)]
        public string Division_Abbr { get; set; }

        public int? Group_Id { get; set; }

        public bool? Is_Active { get; set; }

        public virtual List<DbDistrict> listDistrict { get; set; }
    }
}
