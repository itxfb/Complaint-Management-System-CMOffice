using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{
    [Table("PITB.Translation_Mapping")]
    public partial class DbTranslationMapping
    {
        public int Id { get; set; }

        public int? Parent_Type_Id { get; set; }

        public int? Type_Id { get; set; }

        public int? SubType_Id { get; set; }

        [StringLength(400)]
        public string OrignalString { get; set; }

        [StringLength(400)]
        public string UrduMappedString { get; set; }

        public bool Is_Active { get; set; }
    }
}