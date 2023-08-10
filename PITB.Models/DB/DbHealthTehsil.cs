using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Health_Tehsil")]
    public class DbHealthTehsil
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Tehsil_Name { get; set; }

        public int? District_Id { get; set; }

        public int? Group_Id { get; set; }

        public int? FID { get; set; }

        public bool? Is_Active { get; set; }

        public int? Tehsil_Origin_Id { get; set; }

        public int? SystemID { get; set; }

        public int? Type { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbTehsil")]
        public int? CrmId { get; set; }


        public virtual DbTehsil DbTehsil { get; set; }
    }
}
