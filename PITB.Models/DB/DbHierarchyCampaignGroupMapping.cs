using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Hierarchy_Campaign_Group_Mapping")]
    public class DbHierarchyCampaignGroupMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int Hierarchy_Id { get; set; }

        public int Campaign_Id { get; set; }

        public int Group_Id { get; set; }
    }
}