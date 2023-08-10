using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbProvince
    {
        #region HelperMethods

        public static List<DbProvince> AllProvincesList()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbProvinces.AsNoTracking().OrderBy(m => m.Province_Name).ToList();
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
                    return db.DbProvinces.AsNoTracking().Where(m => m.Province_Id == id).FirstOrDefault();
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
                    List<string> listNames = db.DbProvinces.AsNoTracking().Where(m => listProvince.Contains(m.Province_Id)).Select(n => n.Province_Name).ToList();
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
