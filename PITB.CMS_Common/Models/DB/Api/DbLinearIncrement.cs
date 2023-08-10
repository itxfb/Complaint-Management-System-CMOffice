using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Linear_Increment")]
    public partial class DbLinearIncrement
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int Type { get; set; }

        public int Incremental_Value { get; set; }
    }
}