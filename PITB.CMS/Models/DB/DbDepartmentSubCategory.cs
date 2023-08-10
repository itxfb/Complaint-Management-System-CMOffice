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

    [Table("PITB.Department_SubCategory")]
    public class DbDepartmentSubCategory
    {
        [Key]
        public int Id { get; set; }

        [StringLength(4000)]
        public string Name { get; set; }

        public int? Dep_Cat_Id { get; set; }

        #region Helpers

        public static List<DbDepartmentSubCategory> GetByCategoryId(int catId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDepartmentSubCategory.AsNoTracking().Where(n=>n.Dep_Cat_Id==catId).ToList();
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