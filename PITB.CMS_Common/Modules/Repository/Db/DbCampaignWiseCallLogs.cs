using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    public partial class DbCampaignWiseCallLogs
    {
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
