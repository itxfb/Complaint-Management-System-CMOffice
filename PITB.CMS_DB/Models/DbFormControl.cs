using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Form_Controls")]
    public partial class DbFormControl
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Control_Type { get; set; }

        public int? Priority { get; set; }

        [StringLength(200)]
        public string Field_Name { get; set; }

        [StringLength(200)]
        public string Tag_Id { get; set; }

        [ForeignKey("Control_Id")]
        public virtual List<DbFormPermissionsAssignment> ListDbFormPermission { get; set; }
    }
}
