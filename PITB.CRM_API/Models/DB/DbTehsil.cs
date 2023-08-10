namespace PITB.CRM_API.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Tehsil")]
    public class DbTehsil
    {
        [Key]
        [Column("Id")]
        public int Tehsil_Id { get; set; }

        [StringLength(50)]
        public string Tehsil_Name { get; set; }

        public int? District_Id { get; set; }

        public int? Group_Id { get; set; }
        [NotMapped]
        public string Tehsil_UrduName { get; set; }

        #region HelperMethods


            public static List<DbTehsil> GetAllTehsilList()
            {
                try
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbTehsils.AsNoTracking().ToList();

                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static List<DbTehsil> GetTehsilList(int districtId)
            {
                try
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return (from d in db.DbDistricts
                            join t in db.DbTehsils on
                            d.District_Id equals t.District_Id
                            where d.District_Id==districtId
                            orderby t.Tehsil_Name
                            select t).ToList();

                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static DbTehsil GetById(int tehsilId)
            {
                try
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbTehsils.Where(n => n.Tehsil_Id == tehsilId).ToList().First();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static List<DbTehsil> GetByDistrictId(int districtId)
            {
                try
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbTehsils.Where(n => n.District_Id == districtId).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static List<DbTehsil> GetByDistrictAndGroupId(int districtId, int? groupId)
            {
                try
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbTehsils.Where(n => n.District_Id == districtId && n.Group_Id == groupId).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static List<DbTehsil> GetByDistrictIdsAndGroupId(string districtIds, int? groupId)
            {
                try
                {
                    List<int> listIds = districtIds.Split(',').Select(int.Parse).ToList();

                    DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbTehsils.Where(n => listIds.Contains((int)n.District_Id) && n.Group_Id == groupId).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        #endregion
    }
}
