using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.UserCategory")]
    public partial class DbUserCategory
    {
        public int Id { get; set; }

        //[ForeignKey("DbComplaint")]
        public int? User_Id { get; set; }

        public int? Parent_Category_Id { get; set; }

        public int? Child_Category_Id { get; set; }

        public int? Category_Hierarchy { get; set; }

        //public DbUsers DbUsers { get; set; }
    }
}