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

    public class RepoDbFormControl
    {
        public static List<DbFormControl> GetBy(int campaignId, string TagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    List<DbFormControl> listDbFC = db.DbFormControl.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Tag_Id == TagId).OrderBy(m => m.Priority).Include(n => n.ListDbFormPermission).ToList();//db.Form_Controls.Select(n => n).ToList();
                    return listDbFC;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
