using System.Data.Entity;
using System.Linq;
using Amazon.DirectConnect.Model.Internal.MarshallTransformations;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Health_Districts")]
    public partial class DbHealthDistricts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int District_ID { get; set; }

        [StringLength(50)]
        public string District_Name { get; set; }

        [StringLength(20)]
        public string District_Code { get; set; }

        public int Region_Id { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(100)]
        public string Urdu_Name { get; set; }

        [ForeignKey("DbDistrict")]
        public int? Crm_Dist { get; set; }

        public virtual DbDistrict DbDistrict { get; set; }

        public static DbHealthDistricts GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthDistricts dbHealthCategory = db.DbHealthDistricts.AsNoTracking().Where(m => m.District_ID == id).Include(n => n.DbDistrict).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
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
