using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Dynamic_Categories_Mapping")]
    public partial class DbDynamicCategoriesMapping
    {
        public int Id { get; set; }


        public int? Category_Id { get; set; }

        public int? Subcategory_Id { get; set; }
    }
}