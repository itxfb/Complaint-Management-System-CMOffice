using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{


    //public class DbUnionCouncils2
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string Councils_Name { get; set; }

    //    public int? Tehsil_Id { get; set; }

    //    public int? Campaign_Id { get; set; }

    //    public int? Group_Id { get; set; }

    //    public int? UcNo { get; set; }

    //    public bool? Is_Active { get; set; }
    //}



    [Table("PITB.Union_Councils")]
    public class DbUnionCouncils
    {
        [Key]
        [Column("Id")]
        public int UnionCouncil_Id { get; set; }

        [NotMapped]
        public int Id { get; set; }

        [StringLength(50)]
        public string Councils_Name { get; set; }

        public int? Tehsil_Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Group_Id { get; set; }

        public int? UcNo { get; set; }

        public bool? Is_Active { get; set; }
    }
}
