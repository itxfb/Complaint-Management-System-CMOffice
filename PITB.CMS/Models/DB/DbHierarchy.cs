namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("PITB.Hierarchy")]
    public partial class DbHierarchy
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(200)]
        public string HierarchyName { get; set; }

        public static List<DbHierarchy> GetAllHierarchy()
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbHierarchy.AsNoTracking().Select(m => m).OrderBy(m => m.Id).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbHierarchy> Get(List<int> listHierarchyIds )
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbHierarchy.AsNoTracking().Where(m => listHierarchyIds.Contains(m.Id)).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
