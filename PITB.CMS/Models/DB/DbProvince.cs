using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
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


        public virtual List<DbDivision> listDivisions {get; set;}

        #region HelperMethods

        public static List<DbProvince> AllProvincesList()
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

        public static DbProvince  GetById(int id)
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
        public static string GetByProvinceIdsStr(string idsStr)
        {
            try
            {
                List<int> listProvince = idsStr.Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    List<string> listNames = db.DbProvinces.AsNoTracking().Where(m => listProvince.Contains(m.Province_Id)).Select(n=>n.Province_Name).ToList();
                    return string.Join(",", listNames.Select(n => n.ToString()).ToArray());
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
