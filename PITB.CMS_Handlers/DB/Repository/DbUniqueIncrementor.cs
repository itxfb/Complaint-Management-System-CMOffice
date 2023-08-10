using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbUniqueIncrementor
    {
        public static decimal GetUniqueValue(Config.UniqueIncrementorTag tagId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbUniqueIncrementor dbUniqueIncrementor = db.DbUniqueIncrementor.Where(n => n.TagId == (int)tagId).FirstOrDefault();
                    dbUniqueIncrementor.UniqueValue++;
                    db.DbUniqueIncrementor.Attach(dbUniqueIncrementor);
                    db.Entry(dbUniqueIncrementor).Property(x => x.UniqueValue).IsModified = true;
                    db.SaveChanges();
                    return dbUniqueIncrementor.UniqueValue;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}