using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Campaign")]
    public partial class DbCampaign
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Campaign_Name { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(10)]
        public string Campaign_HelpLine { get; set; }

        public int? District_Id { get; set; }

        public int? Campaign_Type { get; set; }
        public string LogoUrl { get; set; }

        public string LayoutImageUrl { get; set; }

        public string UrlSuffix { get; set; }

        public bool? IsCustomUrlAllowed { get; set; }

        public string StakeholderLogoUrl { get; set; }

        public string LayoutPopupImageBg { get; set; }

        public string Campaign_ShortName { get; set; }
    }


}
