using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{


    public class RepoDbHealthComplaintCategories
    {
        public static DbHealthComplaintCategories GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthComplaintCategories dbHealthCategory = db.DbHealthComplaintCategories.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbComplaintType).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
                    return dbHealthCategory;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
