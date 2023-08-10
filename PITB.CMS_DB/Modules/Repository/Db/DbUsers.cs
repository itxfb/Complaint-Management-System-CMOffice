using System.Threading;
using Z.BulkOperations;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using PITB.CMS_Common;
using PITB.CMS_Common.Models.ApiModels.Response;

namespace PITB.CMS_DB.Models
{

    public partial class DbUsers
    {
        #region HelperMethods
        public static DbUsers GetUserAgainstUserNameAndPassword(string userName, string password)
        {
            try
            {
                var db = new DBContextHelperLinq();
                var user = db.DbUsers.Include(n => n.ListDbUserCategory).Where(n => n.Username == userName && n.Password == password && n.IsActive == true);
                DbUsers dbUser = user.FirstOrDefault();

                //dbUser.ListUserPermissions = listPermissions;
                return dbUser;
                /*return db.DbUsers
                    .Where(n => n.Username == userName && n.Password == password && n.IsActive)
                */
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static Boolean AuthenticateUserVerificationCode(string username, string verification_code)
        {
            try
            {
                var db = new DBContextHelperLinq();

                DbUsers dbUser = db.DbUsers.Where(n => n.Username == username && n.Verification_code == verification_code && n.IsActive == true).FirstOrDefault();
                if (dbUser != null)
                {

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool CheckIfUsernameExists(string username)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    var result = db.DbUsers.Any(x => x.Username.Equals(username));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DbUsers GetUserAgainstUserName(string userName)
        {
            try
            {
                var db = new DBContextHelperLinq();

                DbUsers dbUser = db.DbUsers.Where(n => n.Username == userName && n.IsActive == true).FirstOrDefault();
                return dbUser;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbUsers GetUser(string phoneNo, string cnic)
        {
            try
            {
                var db = new DBContextHelperLinq();

                DbUsers dbUser = db.DbUsers.Where(n => n.Phone == phoneNo && n.Cnic == cnic && n.IsActive == true).FirstOrDefault();
                return dbUser;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DbUsers GetUserAgainstUserNameForForgotPassword(string userName)
        {
            try
            {
                var db = new DBContextHelperLinq();

                DbUsers dbUser = db.DbUsers.Where(n => n.Username == userName && n.IsActive == true).OrderBy(x => x.User_Id).FirstOrDefault();
                return dbUser;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbUsers> GetUsersAgainstCampaign(int campaignId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbUsers.Where(m => m.Campaign_Id == campaignId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUsers> GetUsersAgainstCampaigns(List<int> listCampaigns, bool isActive = true)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbUsers.Where(m => listCampaigns.Contains(m.Campaign_Id.Value) && m.IsActive == isActive).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbUsers GetActiveUser(int id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUsers.FirstOrDefault(m => m.User_Id == id && m.IsActive);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUsers> GetUserAgainstCampaign(int campaignId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUsers.Where(m => m.Campaign_Id == campaignId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<UsersHierarchyMapping> GetActiveUserFilteredDataAgainstParams(int campaignId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUsers.Where(m => m.Campaign_Id == campaignId && m.IsActive).ToList().Select(y => new UsersHierarchyMapping()
                {
                    UserId = y.User_Id,
                    Hierarchy = y.Hierarchy_Id,
                    ProvinceId = y.Province_Id,
                    DivisionId = y.Division_Id,
                    DistrictId = y.District_Id,
                    TehsilId = y.Tehsil_Id,
                    UcId = y.UnionCouncil_Id,
                    WardId = y.Ward_Id,
                    UsersHierarchy = y.User_Hierarchy_Id
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<DbUsers> GetActiveUserAgainstParams(int campaignId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUsers.Where(m => m.Campaign_Id == campaignId && m.IsActive).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static List<DbUsers> GetActiveUserAgainstParams(DBContextHelperLinq db, int campaignId)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUsers.Where(m => m.Campaign_Id == campaignId && m.IsActive).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DbUsers GetUser(int id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbUsers.FirstOrDefault(m => m.User_Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<DbUsers> GetUsers(string cnic)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Cnic == cnic && m.Cnic != null && m.IsActive).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<DbUsers> GetByCnic(string cnic, DBContextHelperLinq db)
        {
            try
            {
                return
                    db.DbUsers.Include(n => n.ListDbUserCategory).Where(n => n.Cnic == cnic && n.IsActive)
                        .ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbUsers> GetUser(int campaignId, Config.Hierarchy? hierarchyId, List<int?> listUserCategoryId1, DateTime from, DateTime to)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaign_Id == campaignId && m.Hierarchy_Id == hierarchyId && listUserCategoryId1.Contains(m.UserCategoryId1)
                    && ((m.Created_Date != null && m.Created_Date >= from && m.Created_Date <= to) ||
                    (m.Updated_Date != null && m.Updated_Date >= from && m.Updated_Date <= to))).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetStakeholderUsernames(int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, int? userHierarchyId, int categoryId, int? userCategory1, int? userCategory2)
        {
            try
            {
                List<DbUsers> listUsers = GetByCampIdH_IdUserH_Id(campaignId, hierarchyId, hierarchyIdVal, userHierarchyId, categoryId, userCategory1, userCategory2);
                string usersList = (listUsers.Count > 0) ? string.Join(",", listUsers.Select(n => n.Username).ToList()) : "None";
                return usersList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetHierarchyIdValueAgainstHierarchyId(DbUsers dbUsers)
        {
            Config.Hierarchy hierarchyValue = (Config.Hierarchy)dbUsers.Hierarchy_Id;
            return GetHierarchy(hierarchyValue, dbUsers);
        }

        public static string GetHierarchyIdValueAgainstHierarchyId(DbUsers dbUsers, Config.Hierarchy hierarchyId)
        {
            //Config.Hierarchy hierarchyValue = (Config.Hierarchy)dbUsers.Hierarchy_Id;
            return GetHierarchy(hierarchyId, dbUsers);
        }

        public static string GetHierarchyVal(DbUsers dbUser)
        {
            return Utility.GetHierarchyValueName((Config.Hierarchy)dbUser.Hierarchy_Id,
                Utility.GetIntByCommaSepStr(DbUsers.GetHierarchy((Config.Hierarchy)dbUser.Hierarchy_Id, dbUser)));
        }

        public static string GetHierarchy(Config.Hierarchy hierarchyValue, DbUsers dbUsers)
        {
            switch (hierarchyValue)
            {
                case Config.Hierarchy.Division:
                    return dbUsers.Division_Id;
                    break;

                case Config.Hierarchy.District:
                    return dbUsers.District_Id;
                    break;

                case Config.Hierarchy.Province:
                    return dbUsers.Province_Id;
                    break;

                case Config.Hierarchy.Tehsil:
                    return dbUsers.Tehsil_Id;
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return dbUsers.UnionCouncil_Id;
                    break;

                case Config.Hierarchy.Ward:
                    return dbUsers.Ward_Id;
                    break;
            }
            return "-1";
        }

        public static Config.CommandStatus ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                var user = db.DbUsers.FirstOrDefault(m => m.User_Id == userId && m.Password == oldPassword);
                if (user != null)
                {
                    user.Password = newPassword;
                    user.Password_Updated = DateTime.Now;
                    db.SaveChanges();
                    return Config.CommandStatus.Success;
                }
                return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return Config.CommandStatus.Exception;
            }
        }
        public static Config.CommandStatus ForgotPasswordChange(string username, string newPassword)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                var user = db.DbUsers.FirstOrDefault(m => m.Username == username && m.IsActive == true);
                if (user != null)
                {
                    user.Password = newPassword;
                    user.Password_Updated = DateTime.Now;
                    user.Verification_code = null;
                    user.VerificationCodeSentDate = null;
                    user.PasswordChangedDate = user.Password_Updated;
                    db.SaveChanges();
                    return Config.CommandStatus.Success;
                }
                return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return Config.CommandStatus.Exception;
            }
        }
        public static int GetVerificationCodeUsersCount(int campaignId)
        {
            try
            {
                int verificationcount = 0;
                DBContextHelperLinq db = new DBContextHelperLinq();
                verificationcount = db.DbUsers.Where(x => x.Campaign_Id == campaignId && x.IsActive == true).Count(y => y.Verification_code != null && (y.VerificationCodeSentDate == null || ((y.PasswordChangedDate == null) ? true : (y.VerificationCodeSentDate > y.PasswordChangedDate))));
                return verificationcount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int GetPasswordChangedUsersCount(int campaignId)
        {
            try
            {
                int passwordchangedcount = 0;
                DBContextHelperLinq db = new DBContextHelperLinq();
                passwordchangedcount = db.DbUsers.Where(x => x.Campaign_Id == campaignId && x.IsActive == true).Count(y => y.PasswordChangedDate != null && y.Verification_code == null && ((y.VerificationCodeSentDate == null) ? true : (y.PasswordChangedDate > y.VerificationCodeSentDate)));
                return passwordchangedcount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Config.CommandStatus UpdateProfile(int userId, string name, string phone, string email)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                var user = db.DbUsers.FirstOrDefault(m => m.User_Id == userId);
                if (user != null)
                {
                    user.Name = name;
                    user.Phone = phone;
                    user.Email = email;
                    user.Updated_Date = DateTime.Now;
                    db.SaveChanges();
                    return Config.CommandStatus.Success;
                }
                return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return Config.CommandStatus.Exception;
            }
        }

        public static Config.CommandStatus UpdateVerificaionCode(int userId, string verification_code)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                var user = db.DbUsers.FirstOrDefault(m => m.User_Id == userId);
                if (user != null)
                {
                    if (user.Verification_code != null && user.VerificationCodeSentDate != null)
                    {
                        user.Verification_code = verification_code;
                    }
                    else
                    {
                        user.Verification_code = verification_code;
                        user.VerificationCodeSentDate = DateTime.Now;
                    }
                    db.SaveChanges();
                    return Config.CommandStatus.Success;
                }
                return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return Config.CommandStatus.Exception;
            }
        }
        public static void UpdateLastLoginDate(int userId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbUsers dbUser = db.DbUsers.FirstOrDefault(m => m.User_Id == userId);
                if (dbUser != null)
                {
                    db.DbUsers.Attach(dbUser);
                    dbUser.LastLoginDate = DateTime.Now;
                    dbUser.LastOpenDate = dbUser.LastLoginDate;
                    dbUser.IsLoggedIn = true;
                    db.Entry(dbUser).Property(x => x.LastLoginDate).IsModified = true;
                    db.Entry(dbUser).Property(x => x.LastOpenDate).IsModified = true;
                    db.Entry(dbUser).Property(x => x.IsLoggedIn).IsModified = true;
                    db.SaveChanges();
                    //return Config.CommandStatus.Success;
                }
                //return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return;
                //return Config.CommandStatus.Exception;
            }
        }

        public static void UpdateSignOutDate(int userId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbUsers dbUser = db.DbUsers.FirstOrDefault(m => m.User_Id == userId);
                if (dbUser != null)
                {
                    db.DbUsers.Attach(dbUser);
                    dbUser.SignOutDate = DateTime.Now;
                    dbUser.IsLoggedIn = false;
                    db.Entry(dbUser).Property(x => x.SignOutDate).IsModified = true;
                    db.Entry(dbUser).Property(x => x.IsLoggedIn).IsModified = true;
                    db.SaveChanges();
                    //return Config.CommandStatus.Success;
                }
                //return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return;
                //return Config.CommandStatus.Exception;
            }
        }

        public static void UpdateLastOpenedDate(int userId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbUsers dbUser = db.DbUsers.FirstOrDefault(m => m.User_Id == userId);
                if (dbUser != null)
                {
                    db.DbUsers.Attach(dbUser);
                    //dbUser.LastLoginDate = DateTime.Now;
                    dbUser.LastOpenDate = DateTime.Now;
                    //dbUser.IsLoggedIn = true;
                    //db.Entry(dbUser).Property(x => x.LastLoginDate).IsModified = true;
                    db.Entry(dbUser).Property(x => x.LastOpenDate).IsModified = true;
                    //db.Entry(dbUser).Property(x => x.IsLoggedIn).IsModified = true;
                    db.SaveChanges();
                    //return Config.CommandStatus.Success;
                }
                //return Config.CommandStatus.Failure;
            }
            catch (Exception)
            {
                return;
                //return Config.CommandStatus.Exception;
            }
        }

        //---------------- Get responsible official ----------------------------
        public static List<DbUsers> GetResponsibleOfficialList(DbComplaint dbComplaint)
        {
            List<DbUsers> listDbUser = null;
            int? campaignId = dbComplaint.Compaign_Id;
            Config.Hierarchy hierarchyId = (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id;
            int hierarchyIdVal = -1;
            int? userHierarchyId = -1;

            bool isOverdue = false, isValuesAssigned = false;
            if ((int)hierarchyId < 1)// for overdue complaint
            {
                isOverdue = true;
            }


            if (!isOverdue && dbComplaint.Status_ChangedBy != null && dbComplaint.Complaint_Status_Id != null) // if status has been changed
            {
                int? escalationStatus = DbStatus.GetById((int)dbComplaint.Complaint_Status_Id).EscalationStatus;
                if (escalationStatus != null)
                {
                    if (escalationStatus == 0) //If no escalation is provided
                    {
                        isValuesAssigned = true;
                        listDbUser = new List<DbUsers>();
                        listDbUser.Add(DbUsers.GetUser((int)dbComplaint.Status_ChangedBy));
                        //DbUsers dbUser = DbUsers.GetActiveUser((int)dbComplaint.Status_ChangedBy);
                        //hierarchyId = (Config.Hierarchy) dbUser.Hierarchy_Id;
                        //hierarchyIdVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint, hierarchyId);
                        //userHierarchyId = dbUser.User_Hierarchy_Id;
                        //List<DbUserCategory> listDbUserCategory = DbUserCategory.GetCategories(dbUser.User_Id);
                        //if(listDbUserCategory!=null && listDbUserCategory.Count > 0)
                        //{
                        //    dbComplaint.UserCategoryId1 = listDbUserCategory[0].Parent_Category_Id;
                        //    dbComplaint.UserCategoryId2 = listDbUserCategory[0].Child_Category_Id;
                        //}
                        //else
                        //{
                        //    dbComplaint.UserCategoryId1 = null;
                        //    dbComplaint.UserCategoryId2 = null;
                        //}
                    }
                }
            }
            if (!isValuesAssigned) // is value assigned
            {
                if (isOverdue)// if minimum level/overdue
                {
                    if (dbComplaint.SrcId1 != null) //If min hierarchy exist
                    {
                        hierarchyId = (Config.Hierarchy)dbComplaint.SrcId1;
                        hierarchyIdVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint,
                                (Config.Hierarchy)hierarchyId);
                        if (dbComplaint.UserSrcId1 == null)
                        {
                            userHierarchyId = Config.UserMaxHierarchy;
                        }
                        else
                        {
                            userHierarchyId = dbComplaint.MinUserSrcId;
                        }
                    }
                }
                else
                {
                    hierarchyIdVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
                    userHierarchyId = dbComplaint.Complaint_Computed_User_Hierarchy_Id;
                }

                listDbUser = DbUsers.GetByCampIdH_IdUserH_Id2(Convert.ToInt32(dbComplaint.Compaign_Id), hierarchyId,
                            hierarchyIdVal, userHierarchyId, Convert.ToInt32(dbComplaint.Complaint_Category), dbComplaint.UserCategoryId1, dbComplaint.UserCategoryId2);
            }
            return listDbUser;
        }

        //---------------- For New User Category Structure ---------------------
        public static List<DbUsers> GetByCampIdH_IdUserH_Id2(int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, int? userHierarchyId, int categoryId, int? userCatogoryId1, int? userCategoryId2)
        {
            try
            {
                string campaignIdStr = campaignId.ToString();
                userHierarchyId = (userHierarchyId == Config.UserMaxHierarchy) ? 0 : userHierarchyId;
                List<DbUserCategory> listComplaintDbUserCategory = DbUserCategory.GetuserCategoryList(userCatogoryId1, userCategoryId2);
                List<DbUsers> listUsersToReturn = null;
                DBContextHelperLinq db = new DBContextHelperLinq();
                switch (hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                             //m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 &&
                             m.IsActive /*&& UsersHandler.AreUserCategoriesSame(m.ListDbUserCategory, listComplaintDbUserCategory)*/).ToList()
                            .Where(n => n.Province_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            //.Where(UsersHandler.AreUserCategoriesSame(n.ListDbUserCategory, listComplaintDbUserCategory))
                            .ToList();
                        break;

                    case Config.Hierarchy.Division:
                        listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                             //m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 &&
                             m.IsActive /*&& UsersHandler.AreUserCategoriesSame(m.ListDbUserCategory, listComplaintDbUserCategory)*/).ToList()
                            .Where(n => n.Division_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();
                        break;

                    case Config.Hierarchy.District:
                        listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                             //m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 &&
                             m.IsActive /*&& UsersHandler.AreUserCategoriesSame(m.ListDbUserCategory, listComplaintDbUserCategory)*/).ToList()
                        .Where(n => n.District_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();
                        break;

                    case Config.Hierarchy.Tehsil:
                        listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                             //m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 &&
                             m.IsActive /*&& UsersHandler.AreUserCategoriesSame(m.ListDbUserCategory, listComplaintDbUserCategory)*/).ToList()
                        .Where(n => n.Tehsil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();
                        break;

                    case Config.Hierarchy.UnionCouncil:

                        listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Include(n => n.ListDbUserWiseDevices).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                               m.IsActive).ToList()
                        .Where(n => n.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();

                        // Check userwise category
                        if (userCatogoryId1 != null || userCategoryId2 != null)
                        {
                            //return listUsersToReturn.Select(n => n.ListDbUserCategory.Where(n.UserCategoryId1 == userCatogoryId1 || n.UserCategoryId2 == userCategoryId2))
                            return listUsersToReturn.Where(n => n.ListDbUserCategory.Any(n2 => n2.Parent_Category_Id == userCatogoryId1 && n2.Child_Category_Id == userCatogoryId1)).ToList();
                        }
                        else
                        {
                            return listUsersToReturn;
                        }




                        //listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Include(n=>n.ListDbUserWiseDevices).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                        //     m.IsActive).ToList()
                        //.Where(n => n.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        //.Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                        //    .ToList();

                        //IQueryable query = db.DbUsers.Include(n => n.ListDbUserCategory).Include(n => n.ListDbUserWiseDevices).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                        //        m.IsActive)
                        // .Where(n => n.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        // .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId)).Select(n=>n);

                        //var sql = query.ToString();

                        //var sql = ((System.Data.Entity.Core.Objects.ObjectQuery)query).ToTraceString();

                        /** usman **/
                        List<DbUsers> _selectedUsers = db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                               m.IsActive).ToList();

                        var _listUsersToReturn0 = _selectedUsers.Join(db.DbUserCategory, o => o.User_Id, s => s.User_Id, (o, s) => new { _selectedUsers = o, DbUserCategory = s }).ToList();

                        var _listUsersToReturn1 = _listUsersToReturn0.Join(db.DbUserWiseDevices, e => e._selectedUsers.User_Id, u => u.User_Id, (e, u) => new { _listUsersToReturn0 = e, ListDbUserWiseDevices = u }).ToList();



                        listUsersToReturn = _listUsersToReturn1.Where(n => n._listUsersToReturn0._selectedUsers.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal) && n._listUsersToReturn0._selectedUsers.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId)).Select(u => u._listUsersToReturn0._selectedUsers).ToList();

                        break;

                    case Config.Hierarchy.Ward:
                        listUsersToReturn = db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                             //m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 &&
                             m.IsActive /*&& UsersHandler.AreUserCategoriesSame(m.ListDbUserCategory, listComplaintDbUserCategory)*/).ToList()
                        .Where(n => n.Ward_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();
                        break;
                }
                /*foreach (DbUserCategory dbUserCategory in listUsersToReturn)
                {
                    
                }*/
                if (listUsersToReturn == null)
                {
                    listUsersToReturn = new List<DbUsers>();
                }
                listUsersToReturn =
                    listUsersToReturn.Where(n => UsersHandler.AreUserCategoriesSame(listComplaintDbUserCategory,
                        n.ListDbUserCategory)).ToList();
                return listUsersToReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //----------------------------------------------------------------------

        public static List<DbUsers> GetByCampIdH_IdUserH_Id(int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, int? userHierarchyId, int categoryId, int? userCatogoryId1, int? userCategoryId2)
        {
            try
            {
                string campaignIdStr = campaignId.ToString();
                userHierarchyId = (userHierarchyId == Config.UserMaxHierarchy) ? 0 : userHierarchyId;
                DBContextHelperLinq db = new DBContextHelperLinq();
                switch (hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        return db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                             m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 && m.IsActive).ToList()
                            .Where(n => n.Province_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();
                        break;

                    case Config.Hierarchy.Division:
                        return db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                         m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 && m.IsActive).ToList()
                        .Where(n => n.Division_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList();
                        break;

                    case Config.Hierarchy.District:
                        return db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                        m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 && m.IsActive).ToList()
                        .Where(n => n.District_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.Tehsil:
                        return db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                        m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 && m.IsActive).ToList()
                        .Where(n => n.Tehsil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.UnionCouncil:
                        return db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                        m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 && m.IsActive).ToList()
                        .Where(n => n.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.Ward:
                        return db.DbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.User_Hierarchy_Id == userHierarchyId &&
                         m.UserCategoryId1 == userCatogoryId1 && m.UserCategoryId2 == userCategoryId2 && m.IsActive).ToList()
                        .Where(n => n.Ward_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                        .Where(n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                            .ToList().ToList();
                        break;
                }
                return new List<DbUsers>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUsers> GetUsersUnderHierarchy(int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal)
        {
            try
            {
                string campaignIdStr = campaignId.ToString();
                //userHierarchyId = (userHierarchyId == Config.UserMaxHierarchy) ? null : userHierarchyId;
                DBContextHelperLinq db = new DBContextHelperLinq();
                switch (hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        /*List<DbUserCategory> listDbUsers = DbUserCategory.GetCategories(1717);
                        foreach (DbUserCategory userCategory in listDbUsers)
                        {
                            if (userCategory.User_Id == 1717)
                            {
                                
                            }
                        }*/
                        return db.DbUsers.Include(n => n.ListDbUserCategory).Where(n => n.Campaigns == campaignIdStr/* && m.Hierarchy_Id >= hierarchyId && m.IsActive*/)//.ToList()
                                                                                                                                                                        //.Where(n => n.Province_Id!=null && n.Province_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList();
                        break;

                    case Config.Hierarchy.Division:
                        return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.Division_Id != null && n.Division_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList();
                        break;

                    case Config.Hierarchy.District:
                        return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.District_Id != null && n.District_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.Tehsil:
                        return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.Tehsil_Id != null && n.Tehsil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.UnionCouncil:
                        return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.UnionCouncil_Id != null && n.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.Ward:
                        return db.DbUsers.Include(n => n.ListDbUserCategory).Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.Ward_Id != null && n.Ward_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;
                }

                return new List<DbUsers>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUsers> GetUsersUnderHierarchy(int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, List<DbUsers> listDbUsers)
        {
            try
            {
                string campaignIdStr = campaignId.ToString();
                //userHierarchyId = (userHierarchyId == Config.UserMaxHierarchy) ? null : userHierarchyId;
                DBContextHelperLinq db = new DBContextHelperLinq();
                switch (hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        /*List<DbUserCategory> listDbUsers = DbUserCategory.GetCategories(1717);
                        foreach (DbUserCategory userCategory in listDbUsers)
                        {
                            if (userCategory.User_Id == 1717)
                            {
                                
                            }
                        }*/
                        return listDbUsers.Where(n => n.IsActive)//.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                                                                 //.Where(n => n.Province_Id!=null && n.Province_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList();
                        break;

                    case Config.Hierarchy.Division:
                        return listDbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.Division_Id != null && n.Division_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList();
                        break;

                    case Config.Hierarchy.District:
                        return listDbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.District_Id != null && n.District_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.Tehsil:
                        return listDbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.Tehsil_Id != null && n.Tehsil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.UnionCouncil:
                        return listDbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.UnionCouncil_Id != null && n.UnionCouncil_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;

                    case Config.Hierarchy.Ward:
                        return listDbUsers.Where(m => m.Campaigns == campaignIdStr && m.Hierarchy_Id >= hierarchyId && m.IsActive).ToList()
                            .Where(n => n.Ward_Id != null && n.Ward_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                            .ToList().ToList();
                        break;
                }

                return new List<DbUsers>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUsers> GetUsersAboveHierarchy(int campaignId, List<Tuple<int, string>> listHierarchyValues)
        {
            try
            {
                string campaignIdStr = campaignId.ToString();
                //userHierarchyId = (userHierarchyId == Config.UserMaxHierarchy) ? null : userHierarchyId;
                List<DbUsers> listDbUsers = new List<DbUsers>();
                DBContextHelperLinq db = new DBContextHelperLinq();
                Config.Hierarchy hierarchyId;
                int hierarchyIdVal = -1;
                foreach (Tuple<int, string> hierarchy in listHierarchyValues)
                {
                    hierarchyId = (Config.Hierarchy)hierarchy.Item1;
                    hierarchyIdVal = Convert.ToInt32(hierarchy.Item2);

                    switch ((Config.Hierarchy)hierarchyId)
                    {
                        case Config.Hierarchy.Province:
                            listDbUsers.AddRange(db.DbUsers.Where(
                                    m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.IsActive)
                                    .ToList()
                                    .Where(
                                        n =>
                                            n.Province_Id.Split(',')
                                                .Select(i => int.Parse(i))
                                                .ToList()
                                                .Contains(hierarchyIdVal))
                                    .ToList());

                            break;

                        case Config.Hierarchy.Division:
                            listDbUsers.AddRange(db.DbUsers.Where(
                                    m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.IsActive)
                                    .ToList()
                                    .Where(
                                        n =>
                                            n.Division_Id.Split(',')
                                                .Select(i => int.Parse(i))
                                                .ToList()
                                                .Contains(hierarchyIdVal))
                                    .ToList());
                            break;

                        case Config.Hierarchy.District:
                            listDbUsers.AddRange(db.DbUsers.Where(
                                m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.IsActive)
                                .ToList()
                                .Where(
                                    n =>
                                        n.District_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .ToList());
                            break;

                        case Config.Hierarchy.Tehsil:
                            listDbUsers.AddRange(db.DbUsers.Where(
                                m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.IsActive)
                                .ToList()
                                .Where(
                                    n =>
                                        n.Tehsil_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .ToList());
                            break;

                        case Config.Hierarchy.UnionCouncil:
                            listDbUsers.AddRange(db.DbUsers.Where(
                                    m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.IsActive)
                                    .ToList()
                                    .Where(
                                        n =>
                                            n.UnionCouncil_Id.Split(',')
                                                .Select(i => int.Parse(i))
                                                .ToList()
                                                .Contains(hierarchyIdVal))
                                    .ToList());
                            break;

                        case Config.Hierarchy.Ward:
                            listDbUsers.AddRange(db.DbUsers.Where(
                                    m => m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId && m.IsActive)
                                    .ToList()
                                    .Where(
                                        n =>
                                            n.Ward_Id.Split(',')
                                                .Select(i => int.Parse(i))
                                                .ToList()
                                                .Contains(hierarchyIdVal))
                                    .ToList());
                            break;
                    }
                }
                return listDbUsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsUserCatPresent(int catId, int catHierarchyId)
        {
            DbUserCategory dbUserCategory = this.ListDbUserCategory.Where(n => n.Parent_Category_Id == catId && n.Category_Hierarchy == catHierarchyId)
                .FirstOrDefault();
            return (dbUserCategory != null);
        }

        public bool IsUserSubCatPresent(int catId, int catHierarchyId)
        {
            DbUserCategory dbUserCategory = this.ListDbUserCategory.Where(n => n.Child_Category_Id == catId && n.Category_Hierarchy == catHierarchyId)
                .FirstOrDefault();
            return (dbUserCategory != null);
        }


        public bool SetUpdated(
            ApiResponseSyncSEModel.ResponseSEDataUsers.Users syncUsers, List<DbCrmIdsMappingToOtherSystem> listHierarchyMapping,
            Dictionary<Config.Hierarchy, string> dictUsersHierarchyPMIUIds,
            List<DbProvince> dbProvince, List<DbDivision> dbDivision, List<DbDistrict> listDistrict, List<DbTehsil> listTehsil, List<DbUnionCouncils> listUc)
        {

            if (syncUsers.crmu_id == "48782")
            {
                //bool isUserActive = this.IsActive;
            }

            //if (syncUsers.crmu_cnic == "3110376528715")
            //{
            //    bool isUserActive = this.IsActive;
            //}

            bool isUpdated = false;
            List<DbUserCategory> listPmiuUserCat = BlUserCategory.GetSchoolEducationUserCategoryList(this.User_Id,
                Utility.ToNullableInt(syncUsers.crmu_user_category_id1),
                Utility.ToNullableInt(syncUsers.crmu_user_category_id2), 1);
            //List<DbUserCategory> listPmiuUserCat = new List<DbUserCategory>();
            //listPmiuUserCat.Add(new DbUserCategory{User_Id = this.User_Id, Parent_Category_Id = Utility.ToNullableInt(syncUsers.crmu_user_category_id1)
            //                                    , Child_Category_Id = Utility.ToNullableInt(syncUsers.crmu_user_category_id2),Category_Hierarchy = 1});



            if (!this.Username.Contains(syncUsers.crmu_cnic.TrimNullable())
                || !syncUsers.crmu_name.IsEqualAfterTrim(this.Name.TrimNullable())
                || !syncUsers.crmu_cnic.IsEqualAfterTrim(this.Cnic.TrimNullable())
                || !syncUsers.crmu_phone.IsEqualAfterTrim(this.Phone.TrimNullable())
                || !syncUsers.crmu_designation.IsEqualAfterTrim(this.Designation.TrimNullable())
                || !syncUsers.crmu_designation_abbreviation.IsEqualAfterTrim(this.Designation_abbr.TrimNullable())

                || (Config.Hierarchy?)Utility.ToNullableInt(syncUsers.crmu_hierarchy_id) != this.Hierarchy_Id // compare hierarchy
                || Utility.ToNullableInt(syncUsers.crmu_user_hierarchy_id) != this.User_Hierarchy_Id           // compare user hierarchy

                || !dictUsersHierarchyPMIUIds[Config.Hierarchy.Province].EqualsTrimmedStr(syncUsers.crmu_province_id_Fk)
                || !dictUsersHierarchyPMIUIds[Config.Hierarchy.Division].EqualsTrimmedStr(syncUsers.crmu_division_id_Fk)
                || !dictUsersHierarchyPMIUIds[Config.Hierarchy.District].EqualsTrimmedStr(syncUsers.crmu_district_id_Fk)
                || !dictUsersHierarchyPMIUIds[Config.Hierarchy.Tehsil].EqualsTrimmedStr(syncUsers.crmu_tehsil_id_Fk)
                || !dictUsersHierarchyPMIUIds[Config.Hierarchy.UnionCouncil].EqualsTrimmedStr(syncUsers.crmu_markaz_id_Fk)

                || !UsersHandler.AreUserCategoriesSame(this.ListDbUserCategory, listPmiuUserCat)
                )
            {
                //db.DbSchoolsMapping.Attach(this);

                if (!dictUsersHierarchyPMIUIds[Config.Hierarchy.Province].EqualsTrimmedStr(syncUsers.crmu_province_id_Fk))
                {
                    this.Province_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                           n =>
                               n.Crm_Module_Cat1 == (int)Config.Hierarchy.Province &&
                               Utility.ToNullableInt(syncUsers.crmu_province_id_Fk) == (n.OTS_Id))
                           .FirstOrDefault().Crm_Id);

                }

                if (!dictUsersHierarchyPMIUIds[Config.Hierarchy.Division].EqualsTrimmedStr(syncUsers.crmu_division_id_Fk))
                {
                    this.Division_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                           n =>
                               n.Crm_Module_Cat1 == (int)Config.Hierarchy.Division &&
                               Utility.ToNullableInt(syncUsers.crmu_division_id_Fk) == (n.OTS_Id))
                           .FirstOrDefault().Crm_Id);
                }

                if (!dictUsersHierarchyPMIUIds[Config.Hierarchy.District].EqualsTrimmedStr(syncUsers.crmu_district_id_Fk))
                {
                    this.District_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                           n =>
                               n.Crm_Module_Cat1 == (int)Config.Hierarchy.District &&
                               Utility.ToNullableInt(syncUsers.crmu_district_id_Fk) == (n.OTS_Id))
                           .FirstOrDefault().Crm_Id);
                }

                if (!dictUsersHierarchyPMIUIds[Config.Hierarchy.Tehsil].EqualsTrimmedStr(syncUsers.crmu_tehsil_id_Fk))
                {
                    //this.Tehsil_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                    //       n =>
                    //           n.Crm_Module_Cat1 == (int)Config.Hierarchy.Tehsil &&
                    //           Utility.ToNullableInt(syncUsers.crmu_tehsil_id_Fk) == (n.OTS_Id))
                    //       .FirstOrDefault().Crm_Id);

                    try
                    {
                        string[] strArr = syncUsers.crmu_tehsil_id_Fk.Replace(" ", "").Split(',');
                        List<string> listCrmTehsil = new List<string>();
                        for (int i = 0; i < strArr.Length; i++)
                        {
                            listCrmTehsil.Add(Utility.GetConvertedString(listHierarchyMapping.Where(
                             n =>
                                 n.Crm_Module_Cat1 == (int)Config.Hierarchy.Tehsil &&
                                 Utility.ToNullableInt(strArr[i]) == (n.OTS_Id))
                             .FirstOrDefault().Crm_Id));

                            //this.UnionCouncil_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                            // n =>
                            //     n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                            //     Utility.ToNullableInt(syncUsers.crmu_markaz_id_Fk) == (n.OTS_Id))
                            // .FirstOrDefault().Crm_Id);
                        }
                        //string str = Utility.GetCommaSepStrFromList(listCrmUc);
                        this.Tehsil_Id = Utility.GetCommaSepStrFromList(listCrmTehsil);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

                if (!dictUsersHierarchyPMIUIds[Config.Hierarchy.UnionCouncil].EqualsTrimmedStr(syncUsers.crmu_markaz_id_Fk))
                {
                    if (syncUsers.crmu_markaz_id_Fk == "-1")
                    {
                        this.UnionCouncil_Id = null;
                    }
                    else
                    {
                        try
                        {
                            string[] strArr = syncUsers.crmu_markaz_id_Fk.Replace(" ", "").Split(',');
                            List<string> listCrmUc = new List<string>();
                            for (int i = 0; i < strArr.Length; i++)
                            {
                                listCrmUc.Add(Utility.GetConvertedString(listHierarchyMapping.Where(
                                 n =>
                                     n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                                     Utility.ToNullableInt(strArr[i]) == (n.OTS_Id))
                                 .FirstOrDefault().Crm_Id));

                                //this.UnionCouncil_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                                // n =>
                                //     n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                                //     Utility.ToNullableInt(syncUsers.crmu_markaz_id_Fk) == (n.OTS_Id))
                                // .FirstOrDefault().Crm_Id);
                            }
                            //string str = Utility.GetCommaSepStrFromList(listCrmUc);
                            this.UnionCouncil_Id = Utility.GetCommaSepStrFromList(listCrmUc);
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                }



                this.Name = syncUsers.crmu_name.TrimNullable();
                this.Cnic = syncUsers.crmu_cnic.TrimNullable();
                this.Phone = syncUsers.crmu_phone.TrimNullable();
                this.Designation = syncUsers.crmu_designation.TrimNullable();
                this.Designation_abbr = syncUsers.crmu_designation_abbreviation.TrimNullable();

                this.UserCategoryId1 = Utility.ToNullableInt(syncUsers.crmu_user_category_id1);
                this.UserCategoryId2 = Utility.ToNullableInt(syncUsers.crmu_user_category_id2);

                if (!UsersHandler.AreUserCategoriesSame(this.ListDbUserCategory, listPmiuUserCat))
                {
                    // Make Point UserCat To NUll
                    foreach (DbUserCategory dbUserCat in this.ListDbUserCategory)
                    {
                        dbUserCat.User_Id = -1;
                    }
                    // 
                    foreach (DbUserCategory dbUserCat in listPmiuUserCat)
                    {
                        this.ListDbUserCategory.Add(dbUserCat);
                    }
                }
                //DbUserCategory dbUserCat = ListDbUserCategory.FirstOrDefault();

                //if (dbUserCat != null)
                //{
                //    dbUserCat.Parent_Category_Id = Utility.ToNullableInt(syncUsers.crmu_user_category_id1);
                //    dbUserCat.Child_Category_Id = Utility.ToNullableInt(syncUsers.crmu_user_category_id2);
                //}
                //else
                //{
                //    ListDbUserCategory = new List<DbUserCategory>();
                //    DbUserCategory dbUserCategory = new DbUserCategory();
                //    dbUserCategory.Parent_Category_Id = Utility.ToNullableInt(syncUsers.crmu_user_category_id1);
                //    dbUserCategory.Child_Category_Id = Utility.ToNullableInt(syncUsers.crmu_user_category_id2);
                //    dbUserCategory.Category_Hierarchy = 1;
                //    ListDbUserCategory.Add(dbUserCategory);
                //}


                if (!this.Username.Contains(this.Cnic))
                {

                    this.Username = this.Username.Replace(" ", "");

                    //string[] userArr = this.Username.Replace(" ", "").Split('-');
                    //string userToComp = userArr[0];


                    int index = this.Username.LastIndexOf('_');


                    if (index == -1) // if index has not been found
                    {
                        index = this.Username.Length;
                    }
                    //string usernamebefore1 = this.Username;
                    //string usernamebefore2 = this.Cnic;
                    string strToBeReplaced = this.Username.Substring(0, index);

                    //string usernamebefore = this.Username;

                    this.Username = this.Username.Replace(strToBeReplaced, this.Cnic);

                    //string usernameafter = this.Username;
                }


                isUpdated = true;
            }
            if (this.IsActive != syncUsers.is_Active)
            {
                this.IsActive = syncUsers.is_Active;
                isUpdated = true;
            }
            return isUpdated;
        }
        public static bool AuthenticateUserWithCNIC(string username, string cnic, string phone, string lastname)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                IQueryable<DbUsers> result = db.DbUsers.Where(x => x.Username.Equals(username) && x.Cnic.Equals(cnic) && x.Phone.Equals(phone));
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        string Lastname = item.Name.Trim().Split(new char[] { ' ' }).LastOrDefault();
                        if (Lastname.Equals(lastname, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DbUsers GetDbUser(ApiResponseSyncSEModel.ResponseSEDataUsers.Users syncUsers, List<DbCrmIdsMappingToOtherSystem> listHierarchyMapping,
            /*Dictionary<Config.Hierarchy, string> dictUsersHierarchyPMIUIds,*/
            List<DbProvince> dbProvince, List<DbDivision> dbDivision, List<DbDistrict> listDistrict, List<DbTehsil> listTehsil, List<DbUnionCouncils> listUc,
            int campaignId, List<DbComplaintType> listDbComplaintType, List<DbUsers> listDbUsers)
        {

            if (syncUsers.crmu_cnic == "3410462002802")
            {

            }

            DbUsers dbUser = new DbUsers();
            List<DbUsers> listUsersWithSameCnic = listDbUsers.Where(n => n.Cnic == syncUsers.crmu_cnic).ToList().OrderByDescending(n => n.LastLoginDate).ToList();

            int totalActiveUsersWithSameCnic = listUsersWithSameCnic.Where(n => n.IsActive).ToList().Count;
            string strCnicSuffix = "";
            if (totalActiveUsersWithSameCnic > 0)
            {
                strCnicSuffix = strCnicSuffix + "_" + totalActiveUsersWithSameCnic;
            }

            dbUser.Username = syncUsers.crmu_cnic; //+ strCnicSuffix;
            dbUser.Password = Utility.GetAutoGeneratedPassword(10,
                new List<Config.PasswordProperty> { Config.PasswordProperty.Numbers });

            if (listUsersWithSameCnic.Count > 0)
            {
                DbUsers u = listUsersWithSameCnic.FirstOrDefault();
                if (u.Password != "1234")
                {
                    dbUser.Password = u.Password;
                }
            }

            dbUser.Cnic = syncUsers.crmu_cnic;
            dbUser.Campaign_Id = campaignId;
            dbUser.Campaigns = "" + campaignId;
            dbUser.Name = syncUsers.crmu_name;
            dbUser.Phone = syncUsers.crmu_phone;

            dbUser.Hierarchy_Id = (Config.Hierarchy?)Utility.ToNullableInt(syncUsers.crmu_hierarchy_id);
            dbUser.User_Hierarchy_Id = Utility.ToNullableInt(syncUsers.crmu_user_hierarchy_id);

            dbUser.Categories =
                Utility.GetCommaSepStrFromList(listDbComplaintType.Select(n => n.Complaint_Category).ToList());
            dbUser.Designation = syncUsers.crmu_designation;
            dbUser.Designation_abbr = syncUsers.crmu_designation_abbreviation;
            dbUser.Role_Id = Config.Roles.Stakeholder;

            dbUser.IsLoggedIn = true;
            dbUser.IsMultipleLoginsAllowed = false;
            dbUser.IsActive = syncUsers.is_Active;
            dbUser.Created_Date = DateTime.Now;
            dbUser.Created_By = (int)Config.CreatedBy.Pmiu;

            if (syncUsers.crmu_province_id_Fk != null)
            {
                dbUser.Province_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                    n =>
                        n.Crm_Module_Cat1 == (int)Config.Hierarchy.Province &&
                        Utility.ToNullableInt(syncUsers.crmu_province_id_Fk) == (n.OTS_Id))
                    .FirstOrDefault().Crm_Id);

            }
            if (syncUsers.crmu_division_id_Fk != null)
            {
                dbUser.Division_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                    n =>
                        n.Crm_Module_Cat1 == (int)Config.Hierarchy.Division &&
                        Utility.ToNullableInt(syncUsers.crmu_division_id_Fk) == (n.OTS_Id))
                    .FirstOrDefault().Crm_Id);

            }
            if (syncUsers.crmu_district_id_Fk != null)
            {
                dbUser.District_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                    n =>
                        n.Crm_Module_Cat1 == (int)Config.Hierarchy.District &&
                        Utility.ToNullableInt(syncUsers.crmu_district_id_Fk) == (n.OTS_Id))
                    .FirstOrDefault().Crm_Id);

            }
            if (syncUsers.crmu_tehsil_id_Fk != null)
            {
                dbUser.Tehsil_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                    n =>
                        n.Crm_Module_Cat1 == (int)Config.Hierarchy.Tehsil &&
                        Utility.ToNullableInt(syncUsers.crmu_tehsil_id_Fk) == (n.OTS_Id))
                    .FirstOrDefault().Crm_Id);

            }
            if (syncUsers.crmu_markaz_id_Fk != null)
            {
                /*
            dbUser.UnionCouncil_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                   n =>
                       n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                       Utility.ToNullableInt(syncUsers.crmu_markaz_id_Fk) == (n.OTS_Id))
                   .FirstOrDefault().Crm_Id);

            */
                // new code

                if (syncUsers.crmu_markaz_id_Fk == "-1")
                {
                    dbUser.UnionCouncil_Id = null;
                }
                else
                {
                    try
                    {
                        string[] strArr = syncUsers.crmu_markaz_id_Fk.Replace(" ", "").Split(',');
                        List<string> listCrmUc = new List<string>();
                        for (int i = 0; i < strArr.Length; i++)
                        {
                            listCrmUc.Add(Utility.GetConvertedString(listHierarchyMapping.Where(
                                n =>
                                    n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                                    Utility.ToNullableInt(strArr[i]) == (n.OTS_Id))
                                .FirstOrDefault().Crm_Id));

                            //this.UnionCouncil_Id = Utility.GetConvertedString(listHierarchyMapping.Where(
                            // n =>
                            //     n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                            //     Utility.ToNullableInt(syncUsers.crmu_markaz_id_Fk) == (n.OTS_Id))
                            // .FirstOrDefault().Crm_Id);
                        }
                        //string str = Utility.GetCommaSepStrFromList(listCrmUc);
                        dbUser.UnionCouncil_Id = Utility.GetCommaSepStrFromList(listCrmUc);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                // end new code

            }

            dbUser.UserCategoryId1 = Utility.ToNullableInt(syncUsers.crmu_user_category_id1);
            dbUser.UserCategoryId2 = Utility.ToNullableInt(syncUsers.crmu_user_category_id2);

            List<DbUserCategory> listPmiuUserCat = BlUserCategory.GetSchoolEducationUserCategoryList(null,
                Utility.ToNullableInt(syncUsers.crmu_user_category_id1),
                Utility.ToNullableInt(syncUsers.crmu_user_category_id2), 1);

            dbUser.ListDbUserCategory = listPmiuUserCat;


            // Send message start

            if (!string.IsNullOrEmpty(dbUser.Phone))
            {
                string messageStr = "Dear Sir/Madam, \n" +
                                    "This is to inform you that SED Hotline user password has been updated. New password is " +
                                    dbUser.Password + " \n " +
                                    "https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver";

                SmsModel smsModel = null;
                smsModel = new SmsModel((int)CMS.Config.Campaign.SchoolEducationEnhanced, dbUser.Phone, messageStr,
                    (int)Config.MsgType.ToStakeholder,
                    (int)Config.MsgSrcType.Web, DateTime.Now, 1,
                    (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
                new Thread(delegate ()
                {
                    TextMessageHandler.SendMessageToPhoneNo(dbUser.Phone, messageStr, true, smsModel);

                }).Start();
            }
            // end message start



            //dbUser.ListDbUserCategory = new List<DbUserCategory>();
            //dbUser.ListDbUserCategory.Add(new DbUserCategory { Parent_Category_Id = dbUser.UserCategoryId1, Child_Category_Id = dbUser.UserCategoryId2, Category_Hierarchy = 1});
            return dbUser;
        }

        public static bool BulkMerge(List<DbUsers> listToMerge, SqlConnection con, bool isAdding)
        {
            /*
            dbUser.Username = syncUsers.crmu_cnic + strCnicSuffix;
            dbUser.Password = "1234";
            dbUser.Campaign_Id = campaignId;
            dbUser.Campaigns = "" + campaignId;
            dbUser.Name = syncUsers.crmu_name;
            dbUser.Phone = syncUsers.crmu_phone;

            dbUser.Hierarchy_Id = (Config.Hierarchy?) Utility.ToNullableInt(syncUsers.crmu_hierarchy_id);
            dbUser.User_Hierarchy_Id = Utility.ToNullableInt(syncUsers.crmu_user_hierarchy_id);

            dbUser.Categories = Utility.GetCommaSepStrFromList(listDbComplaintType.Select(n => n.Complaint_Category).ToList());
            dbUser.Designation = syncUsers.crmu_designation;
            dbUser.Designation_abbr = syncUsers.crmu_designation_abbreviation;
            dbUser.Role_Id = Config.Roles.Stakeholder;

            dbUser.IsLoggedIn = true;
            dbUser.IsMultipleLoginsAllowed = false;
            dbUser.IsActive = syncUsers.is_Active;
            dbUser.Created_Date = DateTime.Now;
            dbUser.Created_By = (int)Config.CreatedBy.Pmiu;
            */
            BulkOperation<DbUsers> bulkOp = new BulkOperation<DbUsers>(con);
            bulkOp.BatchSize = 1000;
            bulkOp.ColumnInputExpression = c => new
            {
                c.Username,
                c.Password,
                c.Campaign_Id,
                c.Campaigns,
                c.Name,
                c.Cnic,
                c.Phone,
                c.Designation,
                c.Designation_abbr,

                c.Province_Id,
                c.Division_Id,
                c.District_Id,
                c.Tehsil_Id,
                c.UnionCouncil_Id,


                c.Hierarchy_Id,
                c.User_Hierarchy_Id,
                c.UserCategoryId1,
                c.UserCategoryId2,

                c.Categories,
                c.Role_Id,
                c.IsLoggedIn,
                c.IsMultipleLoginsAllowed,
                c.IsActive,
                c.Created_Date,
                c.Created_By
            };
            bulkOp.DestinationTableName = "PITB.Users";
            bulkOp.ColumnOutputExpression = c => c.Id;
            bulkOp.ColumnPrimaryKeyExpression = c => c.Id;

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            bulkOp.BulkMerge(listToMerge);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            if (isAdding)
            {
                List<DbUserCategory> listDbUserCategory = new List<DbUserCategory>();
                List<DbUserCategory> listTempDbUserCat = null;

                List<DbPermissionsAssignment> listDbPermissionsAssignment = new List<DbPermissionsAssignment>();
                List<DbPermissionsAssignment> listTempDbPermissionsAssignment = null;


                //PendingFresh = 1,
                //ResolvedUnverified = 2,
                //ResolvedVerified = 3,
                //Notapplicable = 4,
                //Fake = 5,
                //UnsatisfactoryClosed = 6,
                //PendingReopened = 7,
                //Resolved = 8,
                //NotApplicableVerified = 9,
                //FakeVerified = 10,
                //ClosedVerified = 11

                for (int i = 0; i < listToMerge.Count; i++)
                {
                    listTempDbUserCat = listToMerge[i].ListDbUserCategory;
                    for (int j = 0; j < listTempDbUserCat.Count; j++)
                    {
                        if (listTempDbUserCat[j].User_Id != -1)
                        {
                            listTempDbUserCat[j].User_Id = listToMerge[i].Id;
                        }
                    }
                    listDbUserCategory.AddRange(listToMerge[i].ListDbUserCategory);

                    listDbPermissionsAssignment.Add(new DbPermissionsAssignment
                    {
                        Type = (int)Config.PermissionsType.User,
                        Type_Id = listToMerge[i].Id,
                        Permission_Id = (int)Config.Permissions.StatusesForComplaintListing,
                        Permission_Value =
                            "" + (int)Config.ComplaintStatus.PendingFresh + "," +
                            (int)Config.ComplaintStatus.UnsatisfactoryClosed + "," +
                            (int)Config.ComplaintStatus.PendingReopened

                    });

                    listDbPermissionsAssignment.Add(new DbPermissionsAssignment
                    {
                        Type = (int)Config.PermissionsType.User,
                        Type_Id = listToMerge[i].Id,
                        Permission_Id = (int)Config.Permissions.StatusesForComplaintListingAll,
                        Permission_Value =
                            "" + (int)Config.ComplaintStatus.PendingFresh + "," +
                            (int)Config.ComplaintStatus.ResolvedUnverified + "," +
                            (int)Config.ComplaintStatus.ResolvedVerified + "," +
                            (int)Config.ComplaintStatus.UnsatisfactoryClosed + "," +
                            (int)Config.ComplaintStatus.PendingReopened + "," +
                            (int)Config.ComplaintStatus.ClosedVerified

                    });

                    listDbPermissionsAssignment.Add(new DbPermissionsAssignment
                    {
                        Type = (int)Config.PermissionsType.User,
                        Type_Id = listToMerge[i].Id,
                        Permission_Id = (int)Config.Permissions.HideProfileInfoInStakeholderDetail,
                        Permission_Value = null

                    });

                    listDbPermissionsAssignment.Add(new DbPermissionsAssignment
                    {
                        Type = (int)Config.PermissionsType.User,
                        Type_Id = listToMerge[i].Id,
                        Permission_Id = (int)Config.Permissions.StakeholderStatusesOnStatusChangeView,
                        Permission_Value = "" + (int)Config.ComplaintStatus.ResolvedUnverified

                    });
                }

                DbUserCategory.BulkMerge(listDbUserCategory, con);
                DbPermissionsAssignment.BulkMerge(listDbPermissionsAssignment, con);
            }
            return true;
        }

        #endregion
    }
}
