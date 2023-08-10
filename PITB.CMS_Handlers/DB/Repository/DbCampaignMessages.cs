using System.Linq;
using System;
using System.Collections.Generic;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{


    public class RepoDbCampaignMessages
    {
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
