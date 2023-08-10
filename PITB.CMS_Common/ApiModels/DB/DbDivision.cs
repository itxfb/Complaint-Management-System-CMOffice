namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using PITB.CRM_API.Helper.Database;
    using System.Linq;

    [Table("PITB.Divisions")]
    public class DbDivision
    {
        [Key]
        [Column("Id")]
        public int Division_Id { get; set; }

        [StringLength(50)]
        public string Division_Name { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(20)]
        public string Division_Abbr { get; set; }

        public virtual List<DbDistrict> listDistrict { get; set; }


        public static DbDivision GetById(int id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDivisions.AsNoTracking().Where(m => m.Division_Id == id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDivision> GetByProvinceId(int provinceId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDivisions.AsNoTracking().Where(m => m.Province_Id == provinceId).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
