using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Department")]
    public partial class DbDepartment
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? Group_Id { get; set; }

        public bool Is_Active { get; set; }

        //[NotMapped]
        // public virtual List<DbComplaintType> ListComplaintType { get; set; }
    }
}
