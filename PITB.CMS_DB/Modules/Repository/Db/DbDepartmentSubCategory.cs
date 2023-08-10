using System.Linq;
using System;
using System.Collections.Generic;

namespace PITB.CMS_DB.Models
{

    public partial class DbDepartmentSubCategory
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