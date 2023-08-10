using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Unique_Incrementor")]
    public class DbUniqueIncrementor
    {
        public int Id { get; set; }

        public int TagId { get; set; }

        public string TagName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal UniqueValue { get; set; }

        public static decimal GetUniqueValue(Config.UniqueIncrementorTag tagId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbUniqueIncrementor dbUniqueIncrementor = db.DbUniqueIncrementor.Where(n=>n.TagId==(int)tagId).FirstOrDefault();
                    dbUniqueIncrementor.UniqueValue ++;
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