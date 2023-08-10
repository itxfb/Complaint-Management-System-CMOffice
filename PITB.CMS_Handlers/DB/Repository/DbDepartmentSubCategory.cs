using System.Linq;
using System;
using System.Collections.Generic;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbDepartmentSubCategory
    {
        #region Helpers

        public static List<DbDepartmentSubCategory> GetByCategoryId(int catId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDepartmentSubCategory.AsNoTracking().Where(n => n.Dep_Cat_Id == catId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}