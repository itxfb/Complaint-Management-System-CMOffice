namespace PITB.CRM_API.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Wards")]
    public class DbWards
    {
        [Key]
        [Column("id")]
        public int Ward_Id { get; set; }

        public int Uc_Id { get; set; }

        public string Wards_Name { get; set; }

        public int? Group_Id { get; set; }

        [NotMapped]
        public string Wards_UrduName { get; set; }


        #region StartWardHelper
        public static List<DbWards> GetByUnionCouncilId(int ucId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => n.Uc_Id == ucId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbWards GetByWardId(int wardId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => n.Ward_Id == wardId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbWards> GetAllWards()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbWards> GetByUnionCouncilIdList(List<int> listUcId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => listUcId.Contains(n.Uc_Id)).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbWards> GetByUnionCouncilAndGroupId(int ucId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => n.Uc_Id == ucId && n.Group_Id == groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbWards> GetByUnionCouncilIdListAndGroupId(List<int> listUcId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => listUcId.Contains(n.Uc_Id) && n.Group_Id==groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion
    }
}