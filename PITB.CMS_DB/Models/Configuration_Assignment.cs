using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS_DB.Models
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
    }
}
