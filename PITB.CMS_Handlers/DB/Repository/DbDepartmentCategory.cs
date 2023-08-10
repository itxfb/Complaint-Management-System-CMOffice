using System.Linq;
using System;
using System.Collections.Generic;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbDepartmentCategory
    {
        #region Helpers

        public static List<DbDepartmentCategory> All()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDepartmentCategory.AsNoTracking().ToList();
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