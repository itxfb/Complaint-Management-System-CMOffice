using PITB.CMS_Common;
using System;
using System.Linq;

namespace PITB.CMS_DB.Models
{

    public partial class DbCategoryGroupMapping
    {
        public static int? GetModelByCampaignIdAndTypeId(int campId, Config.ComplaintType typeId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbCategoryGroupMapping dbCategoryCampaignGroupMapping = db.DbCategoryGroupMappings.AsNoTracking()
                        .Where(n => n.Campaign_Id == campId && n.Type_Id == (int)typeId)
                        .FirstOrDefault();
                    if (dbCategoryCampaignGroupMapping != null && dbCategoryCampaignGroupMapping.Group_Id != null)
                    {
                        return dbCategoryCampaignGroupMapping.Group_Id;
                    }
                    else
                    {
                        return null;
                    }
                    //return db.DbHierarchyCampaignGroupMappings.AsNoTracking().Where(m => m == id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}