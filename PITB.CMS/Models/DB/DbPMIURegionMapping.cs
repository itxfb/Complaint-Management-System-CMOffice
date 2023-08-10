namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.PMIU_Region_Mapping")]
    public partial class DbPMIURegionMapping
    {
        public int Id { get; set; }

        public int? Hierarchy { get; set; }

        public int? Parent_Id { get; set; }

        public int? Child_Id { get; set; }

        public bool? Is_Active { get; set; }
    }
}
