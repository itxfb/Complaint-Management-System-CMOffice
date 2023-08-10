using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Campaign_Messages")]
    public partial class DbCampaignMessages
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [StringLength(250)]
        public string Message_Body { get; set; }

        public byte? Complaint_Status_Type { get; set; }

        [StringLength(100)]
        public string Tag_Id { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public int? Order_Id { get; set; }

        public bool? Is_Active { get; set; }
    }
}
