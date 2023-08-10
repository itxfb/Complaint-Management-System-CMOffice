using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.PMIU_Region")]
    public partial class DbPMIURegion
    {
        public int Id { get; set; }

        public int? PMIU_Id { get; set; }

        public int? Crm_Id { get; set; }

        public int? Hierarchy_Id { get; set; }

        [StringLength(200)]
        public string Region_Name { get; set; }

        public DateTime? Created_Date_Time { get; set; }

        public DateTime? Updated_Date_Time { get; set; }

        public bool? Is_Active { get; set; }
    }
}
