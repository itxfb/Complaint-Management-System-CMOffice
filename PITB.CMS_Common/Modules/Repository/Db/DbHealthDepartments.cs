using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    public partial class DbHealthDepartments
    {
        public static DbHealthDepartments GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthDepartments dbHealthCategory = db.DbHealthDepartments.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbDepartment).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
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
