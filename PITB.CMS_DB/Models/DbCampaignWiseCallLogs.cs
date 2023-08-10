using System.Linq;

namespace PITB.CMS_DB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Campaign_Wise_Call_Logs")]
    public partial class DbCampaignWiseCallLogs
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [StringLength(100)]
        public string Phone_No { get; set; }

        public DateTime? Call_DateTime { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public string Session_Id { get; set; }
    }
}
