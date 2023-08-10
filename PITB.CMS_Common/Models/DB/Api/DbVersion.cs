using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Version")]
    public partial class DbVersion
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? Version_Type { get; set; }

        public int? App_Id { get; set; }

        public int? Version_Id { get; set; }
    }
}