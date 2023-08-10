using System.Linq;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Department_Category")]
    public class DbDepartmentCategory
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        #region Helpers

        public static List<DbDepartmentCategory> All()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDepartmentCategory.AsNoTracking().ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }
}