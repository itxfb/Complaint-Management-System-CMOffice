using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;



namespace PITB.CMS_Common.Models
{
    public partial class DbSchoolCategoryUserMapping
    {

        public static List<DbSchoolCategoryUserMapping> Get(Config.Campaign campaignId, Config.Categories categoryType, List<int?> listCategoryId, Config.SchoolEducationAssignedTo schoolEducationAssignedTo)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbSchoolCategoryUserMapping.Where(n => n.Campaign_Id == (int)campaignId && n.Category_Type == (int)categoryType
                        && listCategoryId.Contains(n.Category_Id) && n.Assigned_To == (int)schoolEducationAssignedTo).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}