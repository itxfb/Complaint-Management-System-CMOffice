namespace PITB.CRM_API.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Union_Councils")]
    public class DbUnionCouncils
    {
        [Key]
        [Column("Id")]
        public int UnionCouncil_Id { get; set; }

        [StringLength(50)]
        public string Councils_Name { get; set; }

        public int? Tehsil_Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Group_Id { get; set; }

        public int? UcNo { get; set; }

        [NotMapped]
        public string Councils_UrduName { get; set; }

        #region HelperMethods


        public static List<DbUnionCouncils> GetAllUnionCouncilList()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => n.Group_Id == null).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetUnionCouncilList(int tehsilId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where t.Tehsil_Id == tehsilId && uc.Group_Id == null
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetUnionCouncilListByCouncilAndCampaign(int tehsilId, int? campaignId = null)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where t.Tehsil_Id == tehsilId && uc.Campaign_Id == campaignId
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetUnionCouncilAgainstCampaign(int? campaignId = null)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => n.Campaign_Id == campaignId).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbUnionCouncils GetById(int ucId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => n.UnionCouncil_Id == ucId).ToList().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetByTehsilId(int tehsilId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => n.Tehsil_Id == tehsilId && n.Group_Id == null).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetByTehsilIdList(List<int> listTehsilId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => listTehsilId.Contains((int)n.Tehsil_Id) && n.Group_Id == groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}
