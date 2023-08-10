
using System.Collections.Generic;
using PITB.CMS_Common.Helper;
using System.Linq;
using System;
using PITB.CMS_Models.DB;
using PITB.CMS_Common;

namespace PITB.CMS_Handlers.Custom
{
    public class CMSCookie
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ProvinceId { get; set; }
        public List<int> listProvinceIds { get; set; }
        public string DistrictId { get; set; }
        public List<int> listDistrictIds { get; set; }
        public string DivisionId { get; set; }
        public List<int> listDivisionIds { get; set; }
        public string TehsilId { get; set; }
        public List<int> listTehsilIds { get; set; }

        public string TownId { get; set; }
        public List<int> listTownIds { get; set; }
        public string UcId { get; set; }
        public List<int> listUcIds { get; set; }
        public string WardId { get; set; }
        public List<int> listWardIds { get; set; }
        public string Name { get; set; }
        public Config.Roles Role { get; set; }

        public Config.SubRoles? SubRoles { get; set; }
        public string Campaigns { get; set; }

        public string CategoryIdCommaSep { get; set; }

        public Config.Hierarchy Hierarchy_Id { get; set; }

        public int? User_Hierarchy_Id { get; set; }
        public List<int> CategoryIds { get; set; }

        public int? UserCategoryId1 { get; set; }

        public int? UserCategoryId2 { get; set; }

        public int? PreviousLoginId { get; set; }

        public string UrlConfig { get; set; }

        public string Designation { get; set; }
        
        public string DesignationAbbrevation { get; set; }

        public List<string> UrlNavigationList { get; set; }

        public List<DbUserCategory> ListDbUserCategory { get; set; }

        public List<DbPermissionsAssignment> ListCampaignPermissions { get; set; }
        public List<DbPermissionsAssignment> ListPermissions{ get; set;}

        public List<ControllerModel.Response.PartialViewData> ListPartialViewData { get; set; }



        public static CMSCookie DbUserToCookie(DbUsers dbUsers)
        {
            CMSCookie cmsCookie = new CMSCookie();
            cmsCookie.UrlNavigationList = new List<string>();
            cmsCookie.UserId = dbUsers.User_Id;
            cmsCookie.UserName = dbUsers.Username;

            cmsCookie.ProvinceId = dbUsers.Province_Id;
            cmsCookie.listProvinceIds = (!string.IsNullOrEmpty(cmsCookie.ProvinceId)) ? cmsCookie.ProvinceId.Split(',').Select(int.Parse).ToList() : new List<int>();

            cmsCookie.DistrictId = dbUsers.District_Id;
            cmsCookie.listDistrictIds = (!string.IsNullOrEmpty(cmsCookie.DistrictId)) ? cmsCookie.DistrictId.Split(',').Select(int.Parse).ToList() : new List<int>();

            cmsCookie.DivisionId = dbUsers.Division_Id;
            cmsCookie.listDivisionIds = (!string.IsNullOrEmpty(cmsCookie.DivisionId)) ? cmsCookie.DivisionId.Split(',').Select(int.Parse).ToList() : new List<int>();

            cmsCookie.TehsilId = dbUsers.Tehsil_Id;
            cmsCookie.listTehsilIds = (!string.IsNullOrEmpty(cmsCookie.TehsilId)) ? cmsCookie.TehsilId.Split(',').Select(int.Parse).ToList() : new List<int>();

            cmsCookie.UcId = dbUsers.UnionCouncil_Id;
            cmsCookie.listUcIds = (!string.IsNullOrEmpty(cmsCookie.UcId)) ? cmsCookie.UcId.Split(',').Select(int.Parse).ToList() : new List<int>();


            cmsCookie.WardId = dbUsers.Ward_Id;
            cmsCookie.listWardIds = (!string.IsNullOrEmpty(cmsCookie.WardId)) ? cmsCookie.WardId.Split(',').Select(int.Parse).ToList() : new List<int>();


            cmsCookie.Name = dbUsers.Name;
            cmsCookie.Campaigns = dbUsers.Campaigns;
            cmsCookie.CategoryIdCommaSep = dbUsers.Categories;
            if (dbUsers.Categories != null)
            {
                dbUsers.Categories = dbUsers.Categories.Replace(" ", "");
                cmsCookie.CategoryIds = dbUsers.Categories.Split(',').ToIntList();
            }

            cmsCookie.Designation = dbUsers.Designation;
            cmsCookie.DesignationAbbrevation = dbUsers.Designation_abbr;

            //cmsCookie.ListPermissions = dbUsers.ListUserPermissions;
            //cmsCookie.ListPermissions = 
            cmsCookie.ListPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, dbUsers.User_Id);
            /*if (dbUsers.Role_Id == Config.Roles.Stakeholder)
            {
                cmsCookie.ListPermissions.AddRange(
                    DbPermissionsAssignment.GetListOfPermissions((int) Config.PermissionsType.Campaign,
                        Utility.GetNullableIntList(cmsCookie.Campaigns)));
            }*/

            // Start populating CampaignPermission
            DbPermissionsAssignment dbPermissionAssignment = cmsCookie.ListPermissions.FirstOrDefault(n => n.Type_Id == dbUsers.User_Id && n.Permission_Id == (int) Config.Permissions.MultipleCampaignsAssignment);
            if (dbPermissionAssignment != null)
            {
                cmsCookie.ListCampaignPermissions =
                    DbPermissionsAssignment.GetListOfPermissions(
                        (int) Config.PermissionsType.Campaign,
                        Utility.GetNullableIntList(dbPermissionAssignment.Permission_Value));

            }
            else
            {
                cmsCookie.ListCampaignPermissions =
                    DbPermissionsAssignment.GetListOfPermissions(
                        (int)Config.PermissionsType.Campaign,
                        Utility.GetNullableIntList(cmsCookie.Campaigns));
            }
            //------ end Campaign permissions

            cmsCookie.Role = dbUsers.Role_Id;
            cmsCookie.Hierarchy_Id = (dbUsers.Hierarchy_Id==null) ? Config.Hierarchy.Province : (Config.Hierarchy) dbUsers.Hierarchy_Id;
            cmsCookie.User_Hierarchy_Id = (dbUsers.User_Hierarchy_Id == null) ? null : dbUsers.User_Hierarchy_Id;
            cmsCookie.UserCategoryId1 = dbUsers.UserCategoryId1;
            cmsCookie.UserCategoryId2 = dbUsers.UserCategoryId2;

            cmsCookie.ListDbUserCategory = DbUserCategory.GetCategories(cmsCookie.UserId);

            cmsCookie.SubRoles = dbUsers.SubRole_Id;

            return cmsCookie;
        }
        /*
        public int HierarchyTypeId
        {
            get
            {
                switch (this.Hierarchy_Id)
                {
                    case Config.Hierarchy.Province:
                        return Convert.ToInt32(this.ProvinceId);        
                    break;
                    case Config.Hierarchy.Division:
                        return Convert.ToInt32(this.DivisionId);  
                    break;
                    case Config.Hierarchy.District:
                        return Convert.ToInt32(this.DistrictId);
                    break;
                    case Config.Hierarchy.Tehsil:
                        return Convert.ToInt32(this.TehsilId);
                    break;
                    case Config.Hierarchy.UnionCouncil:
                        return Convert.ToInt32(this.UcId);
                    break;
                }
                return Convert.ToInt32(this.ProvinceId);  
            }
        }
        */

        /*
        public CMSCookie(int id, string userName, int? provinceId, int? districtId, string name)
        {
            this.UserID = id;
            this.UserName = userName;
            this.ProvinceId = provinceId;
            this.DistrictId = districtId;
            this.Name = name;
        }*/
    }
}