using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Categories")]
    public class DbCategory
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public double? RetainingHours { get; set; }

        [StringLength(200)]
        public string Tag_1 { get; set; }

        public int? Tag_2 { get; set; }

        public int? Tag_3 { get; set; }

        public int? Hierarchy { get; set; }

        public int? Type_1 { get; set; }

        public int? Type_2 { get; set; }

        public int? Type_3 { get; set; }

        public int? Type_4 { get; set; }

        public int? Type_5 { get; set; }

        public int? Origin_Id { get; set; }

        public bool? Is_Active { get; set; }
    }
}
