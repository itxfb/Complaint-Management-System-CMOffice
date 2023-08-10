using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models
{
    public partial class DbTagLookup
    {
        public static string GetTag(string key)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    //List<DbTagLookup> listDbTagLookup = db.DbTagLookup.Select(n => n).ToList();

                    DbTagLookup dbTagLookup = db.DbTagLookup.AsNoTracking().Where(m => m.Key == key).FirstOrDefault();
                    if (dbTagLookup != null)
                    {
                        return dbTagLookup.Value;
                    }
                    else return null;
                    //return db.DbTagLookup.AsNoTracking().Where(m => m.Key == key).FirstOrDefault().Value;
                }
            }
            catch (Exception ex)
            {
                //return null;
                throw;
            }
        }
    }
}
