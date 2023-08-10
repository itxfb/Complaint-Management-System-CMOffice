using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Unique_Incrementor")]
    public partial class DbUniqueIncrementor
    {
        public int Id { get; set; }

        public int TagId { get; set; }

        public string TagName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal UniqueValue { get; set; }
    }
}