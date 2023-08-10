
namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("PITB.Dynamic_Categories_Mapping")]
    public class DbDynamicCategoriesMapping
    {
        public int Id { get; set; }


        public int? Category_Id { get; set; }

        public int? Subcategory_Id { get; set; }

        public static List<DbDynamicCategoriesMapping> GetDynamicCategoriesByCategoryId(int categoryId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicCategoriesMapping.AsNoTracking().Where(m => m.Category_Id==categoryId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}