using PITB.CRM_API.Helper;

namespace PITB.CRM_API.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Districts")]
    public class DbDistrict
    {
        [Key]
        [Column("id")]
        public int District_Id { get; set; }

        [StringLength(50)]
        public string District_Name { get; set; }

        public int? Division_Id { get; set; }

        [StringLength(10)]
        public string District_Short { get; set; }


        public int? Group_Id { get; set; }

        [NotMapped]
        public string District_UrduName { get; set; }


        #region HelperMethods

        public static List<DbDistrict> GetAllDistrictList()
        {
            try
            {
                try
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbDistricts.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<DbDistrict> GetDistrictByProvinceAndGroup(int ProvinceId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbProvinces
                        join d in db.DbDivisions on
                        p.Province_Id equals d.Province_Id
                        join c in db.DbDistricts on
                        d.Division_Id equals c.Division_Id
                        where (d.Province_Id == ProvinceId)
                        && (c.Group_Id == groupId)
                        orderby c.District_Name
                        select c).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbDistrict GetDistrictByDistrictIdAndGroupId(int districtId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbProvinces
                        join d in db.DbDivisions on
                        p.Province_Id equals d.Province_Id
                        join c in db.DbDistricts on
                        d.Division_Id equals c.Division_Id
                        where (c.District_Id == districtId)
                        && (c.Group_Id == groupId)
                        orderby c.District_Name
                        select c).ToList().FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDistrict> GetDistrictList(int ProvinceId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbProvinces
                        join d in db.DbDivisions on
                        p.Province_Id equals d.Province_Id
                        join c in db.DbDistricts on
                        d.Division_Id equals c.Division_Id
                        where d.Province_Id==ProvinceId
                        orderby c.District_Name
                        select c).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbDistrict GetById(int districtId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbDistricts.Where(n => n.District_Id == districtId).ToList().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<DbDistrict> GetByDivisionId(int divisionId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }

    
}


