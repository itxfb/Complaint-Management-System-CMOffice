using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Client_System")]
    public partial class DbClientSystem
    {
        public int Id { get; set; }

        public int? Type { get; set; }

        [StringLength(100)]
        public string SystemName { get; set; }

        [StringLength(20)]
        public string SystemAbbr { get; set; }

        [StringLength(50)]
        public string Ip { get; set; }

        [StringLength(50)]
        public string SystemUserName { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(500)]
        public string ClientKey { get; set; }

        [StringLength(100)]
        public string HttpSchemePermission { get; set; }

        public int? TokenType { get; set; }

        public TimeSpan? TokenExpiryTime { get; set; }

        public int? TokenExpiryExchanges { get; set; }

        public bool? IsTokenAllowed { get; set; }

        public bool? IsIpAllowed { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string AppVersion { get; set; }

        [StringLength(500)]
        public string AppUpdateUrl { get; set; }

        [StringLength(500)]
        public string AppUpdateMessage { get; set; }

        public double? AppMinVersion { get; set; }

        public double? AppMaxVersion { get; set; }

        public string AppData { get; set; }
    }
}
