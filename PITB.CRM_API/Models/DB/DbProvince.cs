using System.Linq;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Provinces")]
    public class DbProvince
    {
        [Key]
        [Column("Id")]
        public int Province_Id { get; set; }

        [StringLength(30)]
        public string Province_Name { get; set; }

        [NotMapped]
        public string Province_UrduName { get; set; }

        //public virtual List<DbDivision> listDivisions {get; set;}

        #region HelperMethods

        public static List<DbProvince> GetAllProvincesList()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbProvinces.AsNoTracking().OrderBy(m=>m.Province_Name).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbProvince GetById(int id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbProvinces.AsNoTracking().Where(m => m.Province_Id==id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}
