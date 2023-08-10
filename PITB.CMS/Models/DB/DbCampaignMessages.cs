using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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

        public static List<DbCampaignMessages> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbCampaignMessages.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Is_Active == true).OrderBy(m => m.Order_Id).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
