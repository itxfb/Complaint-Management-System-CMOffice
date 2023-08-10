using System.Data.Entity;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("PITB.Health_Complaint_Categories")]
    public partial class DbHealthComplaintCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Category_Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? DepartmentAreaId { get; set; }

        public byte? SystemId { get; set; }

        [StringLength(500)]
        public string Urdu_Name { get; set; }

        public int? ResponsibleId { get; set; }

        public byte? ResponsibleType { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbComplaintType")]
        public int? CrmId { get; set; }

        public virtual DbComplaintType DbComplaintType { get; set; }

        public static DbHealthComplaintCategories GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthComplaintCategories dbHealthCategory = db.DbHealthComplaintCategories.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbComplaintType).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
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
