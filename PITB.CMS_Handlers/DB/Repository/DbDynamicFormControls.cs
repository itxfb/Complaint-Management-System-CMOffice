using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbDynamicFormControls
    {

        public static List<DbDynamicFormControls> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Is_Active == true).OrderBy(m => m.Control_Type).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicFormControls> GetBy(int campaignId, string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.TagId == tagId && m.Is_Active == true).OrderBy(m => m.Control_Type).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static DbDynamicFormControls GetByControlId(int controlId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => m.Id == controlId && m.Is_Active == true).OrderBy(m => m.Control_Type).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicFormControls> GetByControlIds(List<int> listControlIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => listControlIds.Contains(m.Id) && m.Is_Active == true).OrderBy(m => m.Control_Type).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
