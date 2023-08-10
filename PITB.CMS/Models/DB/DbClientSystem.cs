namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Client_System")]
    public partial class DbClientSystem
    {
        public int Id { get; set; }

        public int? Type { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Abbr { get; set; }

        [StringLength(50)]
        public string Ip { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public int? Token_Type { get; set; }

        public TimeSpan? Token_Expiry_Time { get; set; }

        public int? Token_Expiry_Exchanges { get; set; }

        public bool? Is_Token_Allowed { get; set; }

        public bool? Is_Active { get; set; }
    }
}
