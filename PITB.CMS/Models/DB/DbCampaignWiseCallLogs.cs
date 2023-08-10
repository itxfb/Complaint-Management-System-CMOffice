using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
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

        public static Dictionary<string, string> GetSessionIdsDict()
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                try
                {
                    Dictionary<string, string> dbDict =
                        db.DbCampaignWiseCallLogs.Where(n => n.Session_Id != null)
                            .ToDictionary(n => n.Session_Id, n => n.Phone_No);
                    return dbDict;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
