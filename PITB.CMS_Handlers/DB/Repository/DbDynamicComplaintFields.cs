using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbDynamicComplaintFields
    {

        public static List<DbDynamicComplaintFields> GetByComplaintId(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicComplaintFields.AsNoTracking().Where(m => m.ComplaintId == complaintId).OrderBy(m => m.ComplaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicComplaintFields> GetBy(int complaintId, string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicComplaintFields.AsNoTracking().Where(m => m.ComplaintId == complaintId && m.TagId == tagId).OrderBy(m => m.ComplaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicComplaintFields> GetBy(int complaintId, int filter1)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicComplaintFields.AsNoTracking().Where(m => m.ComplaintId == complaintId && m.Filter1 == filter1).OrderBy(m => m.ComplaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void Save(List<DbDynamicComplaintFields> listDbDynamicComplaintFields)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    foreach (DbDynamicComplaintFields dbDynamicComplaintField in listDbDynamicComplaintFields)
                    {
                        db.DbDynamicComplaintFields.Add(dbDynamicComplaintField);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
