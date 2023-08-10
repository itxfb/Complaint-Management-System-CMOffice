using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Configuration_Assignment")]
    public partial class DbConfiguration_Assignment
    {
        public int Id { get; set; }

        public int? Type_1 { get; set; }

        public int? Type_2 { get; set; }

        public int? Type_3 { get; set; }

        public string Tag_Id { get; set; }

        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }

        public bool Is_Active { get; set; }
    }
}
