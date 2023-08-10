using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    public partial class DbHealthComplaintSubCategories
    {
        public static DbHealthComplaintSubCategories GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthComplaintSubCategories dbHealthCategory = db.DbHealthComplaintSubCategories.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbComplaintSubType).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
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
