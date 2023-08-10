using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.PMIU_Region_Mapping")]
    public class DbPMIURegionMapping
    {
        public int Id { get; set; }

        public int? Hierarchy { get; set; }

        public int? Parent_Id { get; set; }

        public int? Child_Id { get; set; }

        public bool? Is_Active { get; set; }
    }
}
