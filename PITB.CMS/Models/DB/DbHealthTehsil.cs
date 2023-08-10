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

    [Table("PITB.Health_Tehsil")]
    public partial class DbHealthTehsil
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Tehsil_Name { get; set; }

        public int? District_Id { get; set; }

        public int? Group_Id { get; set; }

        public int? FID { get; set; }

        public bool? Is_Active { get; set; }

        public int? Tehsil_Origin_Id { get; set; }

        public int? SystemID { get; set; }

        public int? Type { get; set; }

        public int? RefId { get; set; }

        [ForeignKey("DbTehsil")]
        public int? CrmId { get; set; }


        public virtual DbTehsil DbTehsil { get; set; }

        public static DbHealthTehsil GetBy(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbHealthTehsil dbHealthDistrict = db.DbHealthTehsil.AsNoTracking().Where(m => m.RefId == id).Include(n => n.DbTehsil).FirstOrDefault();//db.Form_Controls.Select(n => n).ToList();
                    return dbHealthDistrict;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
