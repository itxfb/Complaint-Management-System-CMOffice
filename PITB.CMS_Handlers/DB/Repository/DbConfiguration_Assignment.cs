using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{
    public class RepoDbConfiguration_Assignment
    {
        public static DbConfiguration_Assignment Get(string key)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Key == key).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
