using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbDynamicCategoriesMapping
    {
        public static List<DbDynamicCategoriesMapping> GetDynamicCategoriesByCategoryId(int categoryId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategoriesMapping.AsNoTracking().Where(m => m.Category_Id == categoryId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}