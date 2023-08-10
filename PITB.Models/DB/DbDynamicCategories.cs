using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Dynamic_Categories")]
    public class DbDynamicCategories
    {
        public int Id { get; set; }

        public int? CampaignId { get; set; }

        public int? CategoryTypeId { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public string TagId { get; set; }

        public bool? Is_Active { get; set; }
    }
}