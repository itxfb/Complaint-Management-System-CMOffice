using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("dbo.Schools_Mapping")]

    public partial class DbSchoolsMapping
    {
        [StringLength(100)]
        public string school_emis_code { get; set; }

        [StringLength(255)]
        public string school_name { get; set; }

        [StringLength(255)]
        public string school_level { get; set; }

        [StringLength(255)]
        public string school_gender { get; set; }

        [Column("district_id")]
        public int? District_Id { get; set; }

        [StringLength(255)]
        public string district_name { get; set; }

        [Column("tehsil_id")]
        public int? Tehsil_Id { get; set; }

        [StringLength(255)]
        public string tehsil_name { get; set; }

        [Column("markaz_id")]
        public int? UnionCouncil_Id { get; set; }

        [StringLength(255)]
        public string markaz_name { get; set; }

        public int? System_District_Id { get; set; }

        public int? System_Tehsil_Id { get; set; }

        public int? System_Markaz_Id { get; set; }

        public bool? System_School_Gender { get; set; }

        public short? School_Type { get; set; }

        public int? School_Category { get; set; }

        // public virtual string School_Type_Str { get; set; }

        public int Id { get; set; }

        public int? PMIU_School_Id { get; set; }

        public bool? Is_Active { get; set; }

        //[NotMapped]
        public virtual DbSchoolEducationHeadMapping dbHeadMapping { get; set; }

        //public virtual DbDistrict dbDistrict { get; set; }

        //public virtual DbTehsil dbTehsil { get; set; }

        //public virtual DbUnionCouncils dbUc { get; set; }
    }
}
