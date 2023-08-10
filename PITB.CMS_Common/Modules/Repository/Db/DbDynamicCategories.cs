using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace PITB.CMS_Common.Models
{

    public partial class DbDynamicCategories
    {
        public static List<DbDynamicCategories> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategories.AsNoTracking().Where(m => m.CampaignId == campaignId).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicCategories> GetBy(int campaignId, string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategories.AsNoTracking().Where(m => m.CampaignId == campaignId && m.TagId == tagId && m.Is_Active == true).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static DbDynamicCategories GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategories.AsNoTracking().Where(m => m.Id == id && m.Is_Active == true).OrderBy(m => m.Name).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicCategories> GetByCampaignAndCategoryId(int campaignId, int categoryId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategories.AsNoTracking().Where(m => m.CampaignId == campaignId && m.CategoryTypeId == categoryId).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicCategories> GetByCategoryIdList(List<int> listCategory)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategories.AsNoTracking().Where(m => listCategory.Contains(m.Id)).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
