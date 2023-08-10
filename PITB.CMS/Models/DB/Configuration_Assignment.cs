using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Configuration_Assignment")]
    public partial class DbConfiguration_Assignment
    {
        public int Id { get; set; }

        public int? Type_1 { get; set; }

        public int? Type_2 { get; set; }

        public int? Type_3 { get; set; }

        public string Tag_Id { get; set; }

        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }

        public static DbConfiguration_Assignment Get(string key)
        {
            try
            {            
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Key == key).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbConfiguration_Assignment> GetByTagId(string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Tag_Id == tagId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
