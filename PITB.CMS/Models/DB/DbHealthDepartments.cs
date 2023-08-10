using System.Data.Entity;
using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Health_Departments")]
    public partial class DbHealthDepartments
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Department_Name { get; set; }

        public int? District_Id { get; set; }

        public byte? SystemId { get; set; }

        [StringLength(500)]
        public string Urdu_Name { get; set; }

        public bool? IsSpecial { get; set; }

        [StringLength(10)]
        public string Province_Id { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbDepartment")]
        public int? CrmId { get; set; }


        public virtual DbDepartment DbDepartment { get; set; }


        public static DbHealthDepartments GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthDepartments dbHealthCategory = db.DbHealthDepartments.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbDepartment).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
                    return dbHealthCategory;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
