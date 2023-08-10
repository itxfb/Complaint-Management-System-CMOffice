using System.Linq;
using System;

namespace PITB.CMS_DB.Models
{

    public partial class DbCallTagging
    {
        #region Helpers

        public static DbCallTagging GetByRecordId(int recordId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCallTagging.AsNoTracking().Where(n => n.Id == recordId).First();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static DbCallTagging GetByRecordId(int recordId, DBContextHelperLinq db)
        {
            try
            {
                return db.DbCallTagging.AsNoTracking().Where(n => n.Id == recordId).First();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
