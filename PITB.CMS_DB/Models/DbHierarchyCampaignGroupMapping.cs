using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Hierarchy_Campaign_Group_Mapping")]
    public partial class DbHierarchyCampaignGroupMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int Hierarchy_Id { get; set; }

        public int Campaign_Id { get; set; }

        public int Group_Id { get; set; }
    }
}