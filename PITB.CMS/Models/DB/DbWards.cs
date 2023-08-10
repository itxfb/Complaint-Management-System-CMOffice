namespace PITB.CMS.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web.Mvc;

    [Table("PITB.Wards")]
    public class DbWards
    {
        [Key]
        [Column("id")]
        public int Ward_Id { get; set; }

        public int Uc_Id { get; set; }

        public string Wards_Name { get; set; }

        public int? Group_Id { get; set; }

        public bool? Is_Active { get; set; }


        #region StartWardHelper
        public static List<DbWards> GetByUnionCouncilId(int ucId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => n.Uc_Id == ucId && n.Is_Active==true).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbWards GetByWardId(int? wardId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => n.Ward_Id == wardId && n.Is_Active == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IEnumerable<SelectListItem> GetSelectListofWardForCampaignAndUnionCouncilIds(int campaignId, int unionCouncilId)
        {
            try
            {
                int groupId = DbHierarchyCampaignGroupMapping.GetGroupIdForCampaignAndHierarchyIds(campaignId, (int)Config.Hierarchy.Ward);
                List<DbWards> lstWard = null;
                if (groupId == -1)
                {
                    lstWard = GetByUnionCouncilAndGroupId(unionCouncilId, null);
                }
                else
                {
                    lstWard = GetByUnionCouncilAndGroupId(unionCouncilId, groupId);
                }
                List<SelectListItem> lstSelectList = new List<SelectListItem>();
                lstSelectList.Add(new SelectListItem()
                {
                    Value = "-1",
                    Text = "--Select--",
                    Selected = true
                });
                for (int i = 0; i < lstWard.Count; i++)
                {
                    lstSelectList.Add(new SelectListItem() { Text = lstWard[i].Wards_Name, Value = lstWard[i].Ward_Id.ToString() });
                }
                return new SelectList(lstSelectList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<DbWards> GetByUnionCouncilAndGroupId(int ucId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbWards.Where(n => n.Uc_Id == ucId && n.Group_Id == groupId && n.Is_Active == true).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbWards> GetByUc(List<int?> listUcIds, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from u in db.DbUnionCouncils
                        join w in db.DbWards on
                        u.UnionCouncil_Id equals w.Uc_Id
                        where listUcIds.Contains(w.Uc_Id) && w.Group_Id == groupId && w.Is_Active == true
                        orderby w.Wards_Name
                        select w).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbWards> GetByDistrictAndGroup(int districtId, int? groupId)
        {
            try
            {

                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from d in db.DbDistricts
                        join t in db.DbTehsils on
                        d.District_Id equals t.District_Id
                        join u in db.DbUnionCouncils on
                        t.Tehsil_Id equals u.Tehsil_Id
                        join w in db.DbWards on
                        u.UnionCouncil_Id equals w.Uc_Id
                        where (d.District_Id == districtId)
                        && (w.Group_Id == groupId) && (w.Is_Active == true)
                        orderby w.Wards_Name
                        select w).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}