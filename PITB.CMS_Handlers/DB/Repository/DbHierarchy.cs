using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbHierarchy
    {
        public static List<DbHierarchy> GetAllHierarchy()
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbHierarchy.AsNoTracking().Select(m => m).OrderBy(m => m.Id).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbHierarchy> Get(List<int> listHierarchyIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbHierarchy.AsNoTracking().Where(m => listHierarchyIds.Contains(m.Id)).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
