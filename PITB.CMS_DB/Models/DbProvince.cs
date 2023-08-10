using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Provinces")]
    public partial class DbProvince
    {
        [Key]
        [Column("Id")]
        public int Province_Id { get; set; }

        [StringLength(30)]
        public string Province_Name { get; set; }


        public virtual List<DbDivision> listDivisions { get; set; }
    }
}
