using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.ClientMesages;
using Newtonsoft.Json;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.Business
{
    public class BIStakeholderAdmin
    {
        public static VmAddStakeholderUser GetAddUser()
        {
            VmAddStakeholderUser vms = new VmAddStakeholderUser();
            CMSCookie cookie = AuthenticationHandler.GetCookie();

            vms.ListOfProvinces = DbProvince.AllProvincesList();
            vms.ListOfCampaign = GetCampaignsAssociatedToUser(); // Get all campaigns 
            vms.ListOfHierachy = DbHierarchy.GetAllHierarchy(); // Get all campaigns 
            //vms.ListOfCategory = DbComplaintType.All(); // Get all category
            vms.IsEdit = false;
            vms.TitleHeading = "Add User";
            vms.SubmitBtnTxt = "Add";
            return vms;
        }
        public static VmEditStakeholderUser GetEditUser(int UserID)
        {
            VmEditStakeholderUser vms = new VmEditStakeholderUser();

            DbUsers us = DbUsers.GetUser(UserID);

            vms.Password = (us.Password == null) ? us.Password : us.Password.Trim();
            vms.Phone = (us.Phone == null) ? us.Phone : us.Phone.Trim();
            vms.Name = (us.Name == null) ? us.Name : us.Name.Trim();
            vms.ActiveState = (us.IsActive == true) ? 1 : 0;
            vms.Address = us.Address == null ? null : us.Address.Trim();
            vms.Designation = us.Designation == null ? us.Designation : us.Designation.Trim();
            vms.DesignationAbbrevation = us.Designation_abbr == null ? null : us.Designation_abbr.Trim();
            vms.CNIC = us.Cnic == null ? null : us.Cnic.Trim();
            vms.ImeiNo = us.Imei_No == null ? null : us.Imei_No.Trim();
            vms.UserHierarchyId = us.User_Hierarchy_Id;
            vms.Email = us.Email == null ? null : us.Email.Trim();
            vms.ProvinceId = Utility.GetConvertedNullInt(us.Province_Id);
            vms.DivisionId = Utility.GetConvertedNullInt(us.Division_Id);
            vms.DistrictId = Utility.GetConvertedNullInt(us.District_Id);
            vms.TehsilId = us.Tehsil_Id == null ? null : Utility.ConvertStringListToIntList(us.Tehsil_Id.Split(new char[] { ',' }).ToList<string>());
            vms.UnionCounilId = us.UnionCouncil_Id == null ? null : Utility.ConvertStringListToIntList(us.UnionCouncil_Id.Split(new char[] { ',' }).ToList<string>());
            int? groupId = null;
            List<DbHierarchyCampaignGroupMapping> listHierarchyCampaignGroupMappings =
            DbHierarchyCampaignGroupMapping.GetModelByCampaignId(Convert.ToInt32(us.Campaign_Id));
            DbHierarchyCampaignGroupMapping hierarchyGroupMapping = listHierarchyCampaignGroupMappings.Where(n => n.Hierarchy_Id == 5)
                            .FirstOrDefault();

            if (hierarchyGroupMapping != null)
            {
                groupId = hierarchyGroupMapping.Group_Id;
            }

            vms.Hierarchy = Convert.ToInt32(us.Hierarchy_Id);
            vms.Campaign = us.Campaign_Id.ToString();
            vms.Categories = us.Categories.Split(',').Select(int.Parse).ToList();
            vms.ListOfProvinces = DbProvince.AllProvincesList();
            vms.ListOfCampaign = GetCampaignsAssociatedToUser(); // Get all campaigns 
            vms.ListOfHierachy = DbHierarchy.GetAllHierarchy(); // Get all campaigns 
            vms.ListOfDivision = DbDivision.GetByProvinceAndGroupId(Convert.ToInt32(us.Province_Id), GetGroupIdForHierarchyAndCampaign(listHierarchyCampaignGroupMappings, Config.Hierarchy.Division));
            vms.ListOfDistrict = DbDistrict.GetDistrictByProvinceAndGroup(Convert.ToInt32(vms.ProvinceId), GetGroupIdForHierarchyAndCampaign(listHierarchyCampaignGroupMappings, Config.Hierarchy.District));

            vms.ListOfTehsil = DbTehsil.GetTehsilList(Convert.ToInt32(vms.DistrictId), GetGroupIdForHierarchyAndCampaign(listHierarchyCampaignGroupMappings, Config.Hierarchy.Tehsil));
            vms.ListOfUnionCouncils = DbUnionCouncils.GetUnionCouncilList(vms.TehsilId, GetGroupIdForHierarchyAndCampaign(listHierarchyCampaignGroupMappings, Config.Hierarchy.UnionCouncil));
            vms.ListOfCategory = DbComplaintType.GetByCampaignId(Convert.ToInt32(us.Campaigns));


            //vms.ListOfUnionCouncils = DbUnionCouncils.GetUnionCouncilList(Convert.ToInt32(vms.TehsilId), Convert.ToInt32(vms.Campaign));




            vms.UserId = UserID;
            vms.IsEdit = true;


            List<DbPermissionsAssignment> listPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions(new DBContextHelperLinq(), (int)Config.PermissionsType.User, UserID,
                    (int)Config.Permissions.TransferComplaint);



            vms.TransferState = (listPermissionAssignment.Count > 0) ? 1 : 0;

            vms.TitleHeading = "Edit User id = " + us.User_Id;
            vms.SubmitBtnTxt = "Edit";
            return vms;
        }
        private static int? GetGroupIdForHierarchyAndCampaign(List<DbHierarchyCampaignGroupMapping> param, Config.Hierarchy hierarchy)
        {
            var x2 = param.Where(x => x.Hierarchy_Id == (int)hierarchy).FirstOrDefault();
            return x2 == null ? null : (int?)x2.Group_Id;
        }
        public static VmAddStakeholderUser GetVmAddStakeholderUser()
        {
            VmAddStakeholderUser vms = new VmAddStakeholderUser();
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            var campaigns = cookie.Campaigns.Split(',');
            vms.ListOfCampaign = GetCampaignsAssociatedToUser(); // Get all campaigns 
            vms.ListOfHierachy = DbHierarchy.GetAllHierarchy(); // Get all campaigns 
            return vms;
        }

        private static List<DbCampaign> GetCampaignsAssociatedToUser()
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            var campaigns = cookie.Campaigns.Split(',');
            var lstCampaigns = DbCampaign.GetByIds(Utility.GetIntArrayFromStringArray(campaigns).Distinct().ToList());
            return lstCampaigns;
        }
        public static ClientMessage OnUserSubmit(VmAddStakeholderUser stakeholderUserVm)
        {
            string message = "";
            if (stakeholderUserVm.IsEdit) // when editing user
            {
                bool isUserAlreadyPresent = BIStakeholderAdmin.IsUserPresentWithUsername((int)stakeholderUserVm.UserId, stakeholderUserVm.UserName, true);
                DbUsers dbUser;//= BIStakeholderAdmin.EditUser(stakeholderUserVm);
                if (!isUserAlreadyPresent)
                {
                    ///dbUser = BIStakeholderAdmin.EditUser(stakeholderUserVm);
                    //message = "User id = " + dbUser.User_Id + " has been Updated successfully";
                    return new ClientMessage(message, true);
                }
                else
                {
                    message = "User with user name = " + stakeholderUserVm.UserName + " is already present";
                    return new ClientMessage(message, false);
                }
            }
            else
            {
                bool isUserAlreadyPresent = BIStakeholderAdmin.IsUserPresentWithUsername(0, stakeholderUserVm.UserName, false);
                DbUsers dbUser; //= BIStakeholderAdmin.AddUser(stakeholderUserVm);
                if (!isUserAlreadyPresent)
                {
                    //dbUser = BIStakeholderAdmin.AddUser(stakeholderUserVm);
                    //message = "User id = " + dbUser.User_Id + " has been Added successfully";
                    return new ClientMessage(message, true);
                }
                else
                {
                    message = "User with user name = " + stakeholderUserVm.UserName + " is already present";
                    return new ClientMessage(message, false);
                }
            }

            return null;
        }


        public static bool AddUser(VmAddStakeholderUser stakeholderUserVm)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbUsers dbDf;
                DbUsers duplicateUser = db.DbUsers.FirstOrDefault(x => object.Equals(stakeholderUserVm.UserName, x.Username));
                if (duplicateUser != null)
                {
                    return false;
                }

                dbDf = new DbUsers();
                db.DbUsers.Add(dbDf);

                dbDf.Name = stakeholderUserVm.Name;
                dbDf.Username = stakeholderUserVm.UserName;
                dbDf.Password = stakeholderUserVm.Password;
                dbDf.IsActive = (stakeholderUserVm.ActiveState == 1) ? true : false;
                dbDf.Province_Id = Utility.GetConvertedString(stakeholderUserVm.ProvinceId);
                dbDf.District_Id = Utility.GetConvertedString(stakeholderUserVm.District); //stakeholderUserVm.District.ToString();
                dbDf.Division_Id = Utility.GetConvertedString(stakeholderUserVm.DivisionId); //Convert.ToInt32(stakeholderUserVm.DivisionId).ToString();
                dbDf.Tehsil_Id = stakeholderUserVm.TehsilId == null ? null : Utility.GetCommaSepStrFromList(stakeholderUserVm.TehsilId);
                dbDf.UnionCouncil_Id = stakeholderUserVm.UnionCounilId == null ? null : Utility.GetCommaSepStrFromList(stakeholderUserVm.UnionCounilId);
                dbDf.Ward_Id = "0";
                dbDf.Campaign_Id = Convert.ToInt32(stakeholderUserVm.Campaign);
                dbDf.Campaigns = stakeholderUserVm.Campaign;
                dbDf.Role_Id = Config.Roles.Stakeholder;
                dbDf.Categories = string.Join(",", stakeholderUserVm.Categories.Select(n => n.ToString()).ToArray());
                dbDf.Hierarchy_Id = (Config.Hierarchy)stakeholderUserVm.Hierarchy;
                dbDf.Created_Date = DateTime.Now;
                dbDf.Created_By = AuthenticationHandler.GetCookie().UserId;
                dbDf.User_Hierarchy_Id = stakeholderUserVm.UserHierarchy;
                if (stakeholderUserVm.ImeiNo == null)
                {
                    dbDf.Imei_No = null;
                }
                else if (stakeholderUserVm.ImeiNo != null && stakeholderUserVm.ImeiNo.Length == 0)
                {
                    dbDf.Imei_No = null;
                }
                else
                {
                    dbDf.Imei_No = stakeholderUserVm.ImeiNo.Trim();
                }

                if (stakeholderUserVm.Phone == null)
                {
                    dbDf.Phone = null;
                }
                else if (stakeholderUserVm.Phone != null && stakeholderUserVm.Phone.Length == 0)
                {
                    dbDf.Phone = null;
                }
                else
                {
                    dbDf.Phone = stakeholderUserVm.Phone.Trim();
                }

                if (stakeholderUserVm.CNIC == null)
                {
                    dbDf.Cnic = null;
                }
                else if (stakeholderUserVm.CNIC != null && stakeholderUserVm.CNIC.Length == 0)
                {
                    dbDf.Cnic = null;
                }
                else
                {
                    dbDf.Cnic = stakeholderUserVm.CNIC.Trim();
                }

                //dbDf.Imei_No = stakeholderUserVm.ImeiNo.Trim();
                //dbDf.Phone = stakeholderUserVm.Phone.Trim();
                //dbDf.Cnic = stakeholderUserVm.CNIC.Trim(); 
                if (stakeholderUserVm.TransferState == 1)
                {
                    BlPermissions.DeleteAndAddPermissionFromDb(db, Config.PermissionsType.User,
                        Config.Permissions.TransferComplaint, dbDf.User_Id);
                }
                if (!string.IsNullOrEmpty(stakeholderUserVm.Phone))
                {

                }
                db.SaveChanges();
                EditAndAddUserPermission(false,db, dbDf.User_Id, dbDf.Campaign_Id, (int?)dbDf.Hierarchy_Id, (int?)dbDf.User_Hierarchy_Id);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public class ResolverUserPermissionClass
        {
            public int HierarchyId { get; set; }
            public string HierarchyName { get; set; }
            public int UserHierarchyId { get; set; }
            public string UserHierarchyName { get; set; }

            public Dictionary<string, string> PermissionsList { get; set; }
        }
        public static void EditAndAddUserPermission(bool isEdit, DBContextHelperLinq db, int userId, int? campaignId, int? phierarchyId, int? pUserHierarchyId)
        {
            if (campaignId == null)
                campaignId = 0;
            if (isEdit)
            {
                if (campaignId == (int)Config.Campaign.FixItLwmc)
                {
                    var dbPermissions = db.DbPermissionsAssignments.Where(x => x.Type == (int)Config.PermissionsType.User && x.Type_Id == userId);
                    db.DbPermissionsAssignments.RemoveRange(dbPermissions);
                    db.SaveChanges();
                    var permission = db.DbPermissionsAssignments.SingleOrDefault(x => x.Type == (int)Config.PermissionsType.Campaign && x.Type_Id == campaignId && x.Permission_Id == (int)Config.CampaignPermissions.ResolverUserPermission);
                    if (permission != null)
                    {
                        string value = permission.Permission_Value;
                        if (value != null)
                        {
                            Dictionary<string, Dictionary<string, string>> dict = Utility.ConvertCollonFormatToMultipleDict(value);
                            if (dict != null && dict.Count > 0)
                            {
                                List<ResolverUserPermissionClass> PermsFromDB = new List<ResolverUserPermissionClass>();

                                foreach (var item in dict)
                                {
                                    string key = item.Key;
                                    string[] ids = key.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

                                    string hierarchyName = ids[0].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string hierarchyId = ids[0].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                    string userHierarchyName = ids[1].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string userHierarchyId = ids[1].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                    ResolverUserPermissionClass perm = new ResolverUserPermissionClass();
                                    perm.HierarchyId = Int32.Parse(hierarchyId);
                                    perm.HierarchyName = hierarchyName;
                                    perm.UserHierarchyId = Int32.Parse(userHierarchyId);
                                    perm.UserHierarchyName = userHierarchyName;
                                    perm.PermissionsList = item.Value;
                                    PermsFromDB.Add(perm);
                                }
                                if (phierarchyId == null)
                                    phierarchyId = 0;
                                if (pUserHierarchyId == null)
                                    pUserHierarchyId = 0;
                                ResolverUserPermissionClass finalPermission = PermsFromDB.Find(x => x.HierarchyId == phierarchyId && x.UserHierarchyId == pUserHierarchyId);
                                if (finalPermission != null)
                                {
                                    foreach (KeyValuePair<string, string> pair in finalPermission.PermissionsList)
                                    {
                                        DbPermissionsAssignment permissionAssignment = new DbPermissionsAssignment();
                                        permissionAssignment.Type = (int)Config.PermissionsType.User;
                                        permissionAssignment.Type_Id = userId;
                                        permissionAssignment.Permission_Id = Int16.Parse(pair.Key);
                                        permissionAssignment.Permission_Value = pair.Value;
                                        db.DbPermissionsAssignments.Add(permissionAssignment);
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                if (campaignId == (int)Config.Campaign.FixItLwmc)
                {
                    var permission = db.DbPermissionsAssignments.SingleOrDefault(x => x.Type == (int)Config.PermissionsType.Campaign && x.Type_Id == campaignId && x.Permission_Id == (int)Config.CampaignPermissions.ResolverUserPermission);
                    if (permission != null)
                    {
                        string value = permission.Permission_Value;
                        if (value != null)
                        {
                            Dictionary<string, Dictionary<string, string>> dict = Utility.ConvertCollonFormatToMultipleDict(value);
                            if (dict != null && dict.Count > 0)
                            {
                                List<ResolverUserPermissionClass> PermsFromDB = new List<ResolverUserPermissionClass>();

                                foreach (var item in dict)
                                {
                                    string key = item.Key;
                                    string[] ids = key.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

                                    string hierarchyName = ids[0].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string hierarchyId = ids[0].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                    string userHierarchyName = ids[1].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string userHierarchyId = ids[1].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                    ResolverUserPermissionClass perm = new ResolverUserPermissionClass();
                                    perm.HierarchyId = Int32.Parse(hierarchyId);
                                    perm.HierarchyName = hierarchyName;
                                    perm.UserHierarchyId = Int32.Parse(userHierarchyId);
                                    perm.UserHierarchyName = userHierarchyName;
                                    perm.PermissionsList = item.Value;
                                    PermsFromDB.Add(perm);
                                }
                                if (phierarchyId == null)
                                    phierarchyId = 0;
                                if (pUserHierarchyId == null)
                                    pUserHierarchyId = 0;
                                ResolverUserPermissionClass finalPermission = PermsFromDB.Find(x => x.HierarchyId == phierarchyId && x.UserHierarchyId == pUserHierarchyId);
                                if (finalPermission != null)
                                {
                                    foreach (KeyValuePair<string, string> pair in finalPermission.PermissionsList)
                                    {
                                        DbPermissionsAssignment permissionAssignment = new DbPermissionsAssignment();
                                        permissionAssignment.Type = (int)Config.PermissionsType.User;
                                        permissionAssignment.Type_Id = userId;
                                        permissionAssignment.Permission_Id = Int16.Parse(pair.Key);
                                        permissionAssignment.Permission_Value = pair.Value;
                                        db.DbPermissionsAssignments.Add(permissionAssignment);
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            
        }

        public static DbUsers EditUserSubmit(VmEditStakeholderUser vm)
        {
            using (var db = new DBContextHelperLinq())
            {
                DbUsers user = db.DbUsers.Single(x => x.User_Id == vm.UserId);
                db.Entry(user).State = EntityState.Modified;
                user.Name = vm.Name.Trim();
                user.Password = vm.Password.Trim();
                user.Phone = vm.Phone == null ? null : vm.Phone.Trim();
                user.Email = vm.Email == null ? null : vm.Email.Trim();
                user.Imei_No = vm.ImeiNo == null ? null : vm.ImeiNo.Trim();
                user.Address = vm.Address == null ? null : vm.Address;
                user.Designation = vm.Designation == null ? null : vm.Designation.Trim();
                user.Designation_abbr = vm.DesignationAbbrevation == null ? null : vm.DesignationAbbrevation.Trim();
                user.Cnic = vm.CNIC == null ? null : vm.CNIC.Trim();
                user.Province_Id = vm.ProvinceId == null ? null : vm.ProvinceId.ToString();
                user.Division_Id = vm.DivisionId == null ? null : vm.DivisionId.ToString();
                user.District_Id = vm.DistrictId == null ? null : vm.DistrictId.ToString();
                user.Tehsil_Id = vm.TehsilId == null ? null : Utility.GetCommaSepStrFromList(vm.TehsilId);
                user.UnionCouncil_Id = vm.UnionCounilId == null ? null : Utility.GetCommaSepStrFromList(vm.UnionCounilId);
                user.Ward_Id = vm.WardId == null ? null : vm.WardId.ToString();
                user.Campaign_Id = short.Parse(vm.Campaign);
                var dbHierarchyId = (int?)user.Hierarchy_Id;
                var dbUserHierarchyId = (int?)user.User_Hierarchy_Id;
                user.Hierarchy_Id = (Config.Hierarchy?)vm.Hierarchy;
                user.User_Hierarchy_Id = vm.UserHierarchyId;
                bool changePermissions = false;
                if (dbHierarchyId != vm.Hierarchy || dbUserHierarchyId != vm.UserHierarchyId)
                {
                    changePermissions = true;
                }
                user.IsActive = Convert.ToBoolean(vm.ActiveState);
                user.Updated_Date = DateTime.Now;
                user.Categories = Utility.GetCommaSepStrFromList(vm.Categories);
                if (vm.TransferState == 1) // if is adding transfer id
                {
                    BlPermissions.DeleteAndAddPermissionFromDb(db, Config.PermissionsType.User,
                        Config.Permissions.TransferComplaint, user.User_Id);
                }
                else
                {
                    BlPermissions.DeletePersonInformation(db, Config.PermissionsType.User,
                        Config.Permissions.TransferComplaint, user.User_Id);
                }
                if(changePermissions)
                    EditAndAddUserPermission(true, db, user.User_Id, user.Campaign_Id, (int?)user.Hierarchy_Id, (int?)user.User_Hierarchy_Id);
                
                db.SaveChanges();
                return user;
            }
        }


        public static bool IsUserPresentWithUsername(int userId, string userName, bool isEditMode)
        {
            DbUsers dbUser = DbUsers.GetUserAgainstUserName(userName);

            if (isEditMode) // edit mode
            {
                DbUsers dbEditUser = DbUsers.GetUser(userId);
                if (dbEditUser.Username != userName && dbUser != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else // add mode
            {
                return (dbUser != null);
            }
        }
        public static bool IsUserPresentWithUsername(string userName)
        {
            return DbUsers.CheckIfUsernameExists(userName);

        }
        public static string GetVerificationAndPasswordChangedCount(int campaign_id)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int verificationCount = DbUsers.GetVerificationCodeUsersCount(campaign_id);
            int passwordchangedCount = DbUsers.GetPasswordChangedUsersCount(campaign_id);
            dict.Add("verificationcodecount", verificationCount);
            dict.Add("passwordchangedcount", passwordchangedCount);
            return JsonConvert.SerializeObject(dict);
        }

        
        public static Tuple<int, DataTable> GetUsersList_P(string Campaigns, int Hierarchy_id, DataTableParamsModel dtModel)
        {
            List<string> prefixStrList = new List<string> { "us", "us", "us", "us", "us", "pr", "dv", "ds", "ts", "uc", "us" };
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("uc.UnionCouncils_Name", " uc.Councils_Name Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>(), prefixStrList, dictFilterQuery);

            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@Campaigns", Campaigns.ToDbObj());
            paramDict.Add("@Hie_Id", Hierarchy_id.ToDbObj());
            paramDict.Add("@StartRow", (dtModel.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtModel.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", (dtModel.ListOrder[0].columnName).ToDbObj());
            paramDict.Add("@OrderByDirection", (dtModel.ListOrder[0].sortingDirectionStr).ToDbObj());
            paramDict.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch.ToDbObj());


            string queryStr = QueryHelper.GetFinalQuery("AdminStakeholder_StakeholderUsers", Config.ConfigType.Query,
                            paramDict);


            //DataSet ds = DBHelper.GetDataSetByQueryString(queryStr, null);
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null); //ds.Tables[0];
            int totalRows = 0, totalFilteredRows=0;
            if (dt.Rows.Count > 0)
            {
                //totalRows = (int) ds.Tables[1].Rows[0]["Total_Count"];
                
                totalRows = (int) dt.Rows[0]["Total_Rows"];
                totalFilteredRows = dt.Rows.Count;
            }
            Tuple<int, int, DataTable> dpt = new Tuple<int, int, DataTable>(totalRows, totalFilteredRows, dt);
            //Tuple<int, int, DataTable> dpt = DBHelper.GetDataTableAndSizeByStoredProcedure("[PITB].[Get_User_List_P]", paramDict);
            //DataTable dt = dpt.Item3;
            //int totalRows = dpt.Item1, totalFilteredRows = dpt.Item2;

            if (dt != null)
            {
                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    DataTable table = dt.Clone();
                    foreach (var row in dt.Rows.OfType<DataRow>().GroupBy(x => x.Field<int>("Id")))
                    {
                        string tehsilName = null;
                        string unionCouncilName = null;
                        foreach (var tehsil in row.GroupBy(t => t.Field<string>("Tehsil_Name")))
                        {
                            tehsilName = string.Concat(tehsilName, string.Format("{0}, ", tehsil.First().Field<string>("Tehsil_Name")));
                        }
                        foreach (var unionCouncil in row.GroupBy(uc => uc.Field<string>("UnionCouncils_Name")))
                        {
                            unionCouncilName = string.Concat(unionCouncilName, string.Format("{0}, ", unionCouncil.First().Field<string>("UnionCouncils_Name")));
                        }
                        char[] trim = { ',', ' ', '.' };
                        tehsilName = tehsilName.TrimEnd(trim);
                        unionCouncilName = unionCouncilName.TrimEnd(trim);
                        DataRow data = table.NewRow();
                        table.Rows.Add(data);
                        data.BeginEdit();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (Equals(dt.Columns[i].ColumnName, "Tehsils_Name"))
                            {
                                data.SetField(dt.Columns[i].ColumnName, tehsilName);
                            }
                            else if (Equals(dt.Columns[i].ColumnName, "UnionCouncils_Name"))
                            {
                                data.SetField(dt.Columns[i].ColumnName, unionCouncilName);
                            }
                            else
                            {
                                data.SetField(dt.Columns[i].ColumnName, row.ElementAt(0).Field<object>(dt.Columns[i].ColumnName.ToString()));
                            }
                        }
                        data.AcceptChanges();
                        data.EndEdit();
                    }
                    return new Tuple<int, DataTable>(totalRows, table); ;
                }
            }
            return new Tuple<int, DataTable>(totalRows, dt);
        }

        public static DataTable GetComplaintsOfAgents(string Campaigns, int Hierarchy_id)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@Campaigns", Campaigns.ToDbObj());
            paramDict.Add("@Hie_Id", Hierarchy_id.ToDbObj());

            DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_User_List]", paramDict);
            if (dt != null)
            {
                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    DataTable table = dt.Clone();
                    foreach (var row in dt.Rows.OfType<DataRow>().GroupBy(x => x.Field<int>("Id")))
                    {
                        string tehsilName = null;
                        string unionCouncilName = null;
                        foreach (var tehsil in row.GroupBy(t => t.Field<string>("Tehsils_Name")))
                        {
                            tehsilName = string.Concat(tehsilName, string.Format("{0}, ", tehsil.First().Field<string>("Tehsils_Name")));
                        }
                        foreach (var unionCouncil in row.GroupBy(uc => uc.Field<string>("UnionCouncils_Name")))
                        {
                            unionCouncilName = string.Concat(unionCouncilName, string.Format("{0}, ", unionCouncil.First().Field<string>("UnionCouncils_Name")));
                        }
                        char[] trim = { ',', ' ', '.' };
                        tehsilName = tehsilName.TrimEnd(trim);
                        unionCouncilName = unionCouncilName.TrimEnd(trim);
                        DataRow data = table.NewRow();
                        table.Rows.Add(data);
                        data.BeginEdit();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (Equals(dt.Columns[i].ColumnName, "Tehsils_Name"))
                            {
                                data.SetField(dt.Columns[i].ColumnName, tehsilName);
                            }
                            else if (Equals(dt.Columns[i].ColumnName, "UnionCouncils_Name"))
                            {
                                data.SetField(dt.Columns[i].ColumnName, unionCouncilName);
                            }
                            else
                            {
                                data.SetField(dt.Columns[i].ColumnName, row.ElementAt(0).Field<object>(dt.Columns[i].ColumnName.ToString()));
                            }
                        }
                        data.AcceptChanges();
                        data.EndEdit();
                    }
                    return table;
                }
            }
            return dt;
        }

        public static DataTable GetUserListings(DataTableParamsModel dtModel)
        {
            List<string> prefixStrList = new List<string> { "personInfo", "personInfo", "personInfo", "personInfo", "personInfo", "district" };
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("district.District_Name", " district.District_Name Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>(), prefixStrList, dictFilterQuery);          
 
            Dictionary<string, object> dictParam = new Dictionary<string, object>();
            if (dtModel != null)
            {
                dictParam.Add("@StartRow", (dtModel.Start).ToDbObj());
                dictParam.Add("@EndRow", (dtModel.End).ToDbObj());
                dictParam.Add("@OrderByColumnName", (dtModel.ListOrder[0].columnName).ToDbObj());
                dictParam.Add("@OrderByDirection", (dtModel.ListOrder[0].sortingDirectionStr).ToDbObj());
                dictParam.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch.ToDbObj());
            }

            string queryStr = null;
           
            string exportStr = "";

            queryStr = QueryHelper.GetFinalQuery("AdminStakeholder_ComplainantUsers" + exportStr, Config.ConfigType.Query, dictParam);


            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }


        public static bool UpdatePersonActiveInactive(int id, int isActive)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonInfo = DbPersonInformation.GetById(id, db);
            if (isActive ==1)
                dbPersonInfo.Is_Active = false;
            else
                dbPersonInfo.Is_Active = true;

            DBContextHelperLinq.UpdateEntity(dbPersonInfo, db, new List<string> { "Is_Active" });
            db.SaveChanges();
            return true;
        }
    }
}