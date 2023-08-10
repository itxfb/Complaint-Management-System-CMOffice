using System.Linq;
using System;
using System.Collections.Generic;

namespace PITB.CMS_DB.Models
{

    public partial class DbDepartmentCategory
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