using System.Data.Entity;
using System.Linq;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Health_Complaint_SubCategories")]
    public partial class DbHealthComplaintSubCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubCategory_Id { get; set; }

        [StringLength(500)]
        public string SubCategory_Name { get; set; }

        public int? Category_Id { get; set; }

        [StringLength(500)]
        public string Urdu_Name { get; set; }

        public byte? Priority { get; set; }

        public int? Level0 { get; set; }

        public int? Level1 { get; set; }

        public int? Level2 { get; set; }

        public int? Level3 { get; set; }

        public int? Level4 { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbComplaintSubType")]
        public int? CrmId { get; set; }

        public virtual DbComplaintSubType DbComplaintSubType { get; set; }

        public static DbHealthComplaintSubCategories GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthComplaintSubCategories dbHealthCategory = db.DbHealthComplaintSubCategories.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbComplaintSubType).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
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
