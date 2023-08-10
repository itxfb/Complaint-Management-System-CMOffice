using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Configuration_Assignment")]
    public class DbConfiguration_Assignment
    {
        public int Id { get; set; }

        public int? Type_1 { get; set; }

        public int? Type_2 { get; set; }

        public int? Type_3 { get; set; }

        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
