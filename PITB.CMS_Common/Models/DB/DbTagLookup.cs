namespace PITB.CMS_Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Tag_Lookup")]
    public partial class DbTagLookup
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
