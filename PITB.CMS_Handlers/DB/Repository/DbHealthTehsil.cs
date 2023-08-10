using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbHealthTehsil
    {
        public static DbHealthTehsil GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthTehsil dbHealthDistrict = db.DbHealthTehsil.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbTehsil).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
                    return dbHealthDistrict;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
