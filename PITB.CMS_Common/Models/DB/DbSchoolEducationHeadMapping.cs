using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("dbo.SchoolEducationHeadMapping")]
    public partial class DbSchoolEducationHeadMapping
    {
        [Key]
        [Column("School_Emis_Code_Str")]
        public string school_emis_code { get; set; }

        public string School_Head_Name { get; set; }

        public string School_Head_Designation { get; set; }

        public string School_Head_PhoneNo { get; set; }
    }
}