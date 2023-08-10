using System.Web.UI;
using Amazon.IdentityManagement.Model;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.StakeHolder
{
    public class UsersHandler
    {
        public static List<DbUsers> FindUserLowerThanCurrentHierarchy(int userId, int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, bool CanDiscartHierarchyLowerThanImmediateOne)
        {
            List<DbUsers> listDbUsers = DbUsers.GetUsersUnderHierarchy(campaignId, hierarchyId, hierarchyIdVal);
            DbUsers currUser = listDbUsers.Where(n => n.User_Id == userId).FirstOrDefault();
            //listDbUsers.RemoveAll(n => n.User_Id == userId);

            List<DbUsers> listUsersToRemove = new List<DbUsers>() ;
            listUsersToRemove.Add(currUser);

            

            foreach (DbUsers dbUser in listDbUsers)
            {
                if (dbUser.Hierarchy_Id == hierarchyId && ( /*dbUser.User_Hierarchy_Id==null || */(dbUser.User_Hierarchy_Id!=null && dbUser.User_Hierarchy_Id >= currUser.User_Hierarchy_Id))) // if users are above same hierarchy
                {
                    listUsersToRemove.Add(dbUser);
                }

                if ((currUser.UserCategoryId1 != null && dbUser.UserCategoryId1!=currUser.UserCategoryId1) ||
                    (currUser.UserCategoryId2 != null && dbUser.UserCategoryId2 != currUser.UserCategoryId2))// if user category are not same
                {
                    if (!listUsersToRemove.Contains(dbUser))
                    {
                        listUsersToRemove.Add(dbUser);
                    }
                }
            }
            listDbUsers = listDbUsers.Except(listUsersToRemove).ToList();
            listDbUsers = HideUserAtLevel(Config.Hierarchy.District, 30, listDbUsers);
            if (CanDiscartHierarchyLowerThanImmediateOne)
            {
                listDbUsers = DiscartHierarchyLowerThanImmediateOne(listDbUsers, hierarchyId, currUser.User_Hierarchy_Id);
            }

            return listDbUsers;
        }


        //----------------- For New UserCategoryChange --------------------
        public static List<DbUsers> FindUserLowerThanCurrentHierarchy2(int userId, int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, bool CanDiscartHierarchyLowerThanImmediateOne, List<DbUsers> listDbUsers, List<int> listCategories  )
        {
            bool canFilterCategories = false;
            if (listCategories != null && listCategories.Count > 0)
            {
                canFilterCategories = true;
            }
            
            if (listDbUsers == null)
            {
                listDbUsers = DbUsers.GetUsersUnderHierarchy(campaignId, hierarchyId, hierarchyIdVal);
            }
            else
            {
                listDbUsers = DbUsers.GetUsersUnderHierarchy(campaignId, hierarchyId, hierarchyIdVal,listDbUsers);
            }
            
            //List<DbUsers> listDbUsers = DbUsers.GetUsersUnderHierarchy(campaignId, hierarchyId, hierarchyIdVal);
            DbUsers currUser = listDbUsers.Where(n => n.User_Id == userId).FirstOrDefault();
            //listDbUsers.RemoveAll(n => n.User_Id == userId);

            List<DbUsers> listUsersToRemove = new List<DbUsers>();
            listUsersToRemove.Add(currUser);



            foreach (DbUsers dbUser in listDbUsers)
            {
                
                if (dbUser.Hierarchy_Id == hierarchyId && ( /*dbUser.User_Hierarchy_Id==null || */(dbUser.User_Hierarchy_Id != null && dbUser.User_Hierarchy_Id >= currUser.User_Hierarchy_Id))) // if users are above same hierarchy
                {
                    listUsersToRemove.Add(dbUser);
                }

                //if ((currUser.UserCategoryId1 != null && dbUser.UserCategoryId1 != currUser.UserCategoryId1) ||
                //    (currUser.UserCategoryId2 != null && dbUser.UserCategoryId2 != currUser.UserCategoryId2))// if user category are not same
                if (!AreUserCategoriesSame(currUser, dbUser))
                {
                    if (!listUsersToRemove.Contains(dbUser))
                    {
                        listUsersToRemove.Add(dbUser);
                    }
                }
            }
            listDbUsers = listDbUsers.Except(listUsersToRemove).ToList();
            listDbUsers = HideUserAtLevel(Config.Hierarchy.District, 30, listDbUsers);
            if (CanDiscartHierarchyLowerThanImmediateOne)
            {
                listDbUsers = DiscartHierarchyLowerThanImmediateOne(listDbUsers, hierarchyId, currUser.User_Hierarchy_Id);
            }

            listUsersToRemove.Clear();
            if (canFilterCategories)
            {
                foreach (DbUsers dbUsers in listDbUsers)
                {
                    List<int> listCommonCategories = Utility.GetIntList(dbUsers.Categories).ToList().Intersect(listCategories).ToList();
                    if (listCommonCategories.Count == 0)
                    {
                        listUsersToRemove.Add(dbUsers);
                    }
                    else
                    {
                        dbUsers.Categories = Utility.GetCommaSepStrFromList(listCommonCategories);
                    }
                }
            }

            listDbUsers = listDbUsers.Except(listUsersToRemove).ToList();

            return listDbUsers;
        }

        public static bool AreUserCategoriesSame(DbUsers dbUserToMacth, DbUsers dbUser2) // user1 = current user
        {
            if (dbUserToMacth.ListDbUserCategory.Count != 0)
            {
                int count = 0;
                foreach (DbUserCategory dbUserCat1 in dbUserToMacth.ListDbUserCategory)
                {
                    count = dbUser2.ListDbUserCategory.Where(n => (dbUserCat1.Parent_Category_Id ==null ||  n.Parent_Category_Id == dbUserCat1.Parent_Category_Id) &&
                                                          (dbUserCat1.Child_Category_Id == null || n.Child_Category_Id == dbUserCat1.Child_Category_Id) &&
                                                          n.Category_Hierarchy == dbUserCat1.Category_Hierarchy).ToList().Count;
                    if (count == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            if (dbUserToMacth.ListDbUserCategory.Count == 0 && dbUser2.ListDbUserCategory.Count == 0)
            {
                return true;
            }
            return false;
        }

        public static bool AreUserCategoriesSame(List<DbUserCategory> listToMacth, List<DbUserCategory> list2) // user1 = current user
        {
            if (listToMacth!=null && list2!=null && listToMacth.Where(n => n.Parent_Category_Id == null && n.Child_Category_Id == null).Count() ==
                listToMacth.Count && list2.Count == 0)
            {
                return true;
            }

            if (listToMacth.Count != 0)
            {
                int count = 0;
                foreach (DbUserCategory dbUserCat1 in listToMacth)
                {
                    count = list2.Where(n => (dbUserCat1.Parent_Category_Id ==null || n.Parent_Category_Id == dbUserCat1.Parent_Category_Id) &&
                                                          (dbUserCat1.Child_Category_Id == null || n.Child_Category_Id == dbUserCat1.Child_Category_Id) &&
                                                          n.Category_Hierarchy == dbUserCat1.Category_Hierarchy).ToList().Count;
                    if (count == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsAnyCatPresent(DbUsers dbUser)
        {
            if (dbUser.ListDbUserCategory != null && dbUser.ListDbUserCategory.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsAnyCatPresent(List<DbUserCategory> listUserCat)
        {
            if (listUserCat != null && listUserCat.Count > 0)
            {
                return true;
            }
            return false;
        }

        /*public static bool IsCatPresent(DbUsers dbUser,int catId, int catHierarchy)
        {
            dbUser
            return false;
        }*/


        public static List<DbUsers> FindUserUpperThanCurrentHierarchy2(DbUsers currUser, DbComplaint dbComplaint, int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, bool CanDiscartHierarchyUpperThanImmediateOne)
        {

            //DbComplaint dbComplaint = null;
            /*
            if (dbComplaint != null)
            {
                dbComplaint = DbComplaint.GetByComplaintId((int)complaintId);
            }*/
            List<Tuple<int, string>> listHierarchyMappings = new List<Tuple<int, string>>();

            if (dbComplaint != null)
            {
                listHierarchyMappings = Utility.GetHierarchyMappingListByComplaint(dbComplaint, (int)hierarchyId);
            }
            else if (currUser != null)
            {
                listHierarchyMappings = Utility.GetHierarchyMappingListByUser(currUser, (int)hierarchyId);
            }

            List<DbUsers> listDbUsers = DbUsers.GetUsersAboveHierarchy(campaignId, listHierarchyMappings);
            //DbUsers currUser = listDbUsers.Where(n => n.User_Id == userId).FirstOrDefault();
            //listDbUsers.RemoveAll(n => n.User_Id == userId);

            List<DbUsers> listUsersToRemove = new List<DbUsers>();
            listUsersToRemove.Add(currUser);

            int? userCategoryId1 = null, userCategoryId2 = null, userHierarchyId = null;
            List<DbUserCategory> listDbUserCatogory = null;
            if (dbComplaint != null)
            {
                //userCategoryId1 = dbComplaint.UserCategoryId1;
                //userCategoryId2 = dbComplaint.UserCategoryId2;
                listDbUserCatogory = dbComplaint.GetUserCategoryList();
                // dbComplaint.ListUserCategory;
                if (dbComplaint.Complaint_Computed_User_Hierarchy_Id == Config.UserMaxHierarchy)
                {
                    userHierarchyId = 0;
                }
                else
                {
                    userHierarchyId = dbComplaint.Complaint_Computed_User_Hierarchy_Id;
                }
            }
            else if (currUser != null)
            {
                //userCategoryId1 = currUser.UserCategoryId1;
                //userCategoryId2 = currUser.UserCategoryId2;
                listDbUserCatogory = currUser.ListDbUserCategory;
                userHierarchyId = currUser.User_Hierarchy_Id;
            }

            foreach (DbUsers dbUser in listDbUsers)
            {
                if (dbUser.Hierarchy_Id == hierarchyId && ( /*dbUser.User_Hierarchy_Id==null || */(dbUser.User_Hierarchy_Id != null && dbUser.User_Hierarchy_Id >= userHierarchyId))) // if users are above same hierarchy
                {
                    listUsersToRemove.Add(dbUser);
                }

                //if ((userCategoryId1 != null && dbUser.UserCategoryId1 != null && dbUser.UserCategoryId1 != userCategoryId1) ||
                //    (userCategoryId2 != null && dbUser.UserCategoryId2 != null && dbUser.UserCategoryId2 != userCategoryId2))// if user category are not same
                if (/*(userCategoryId1 != null || IsAnyCatPresent(currUser))*/IsAnyCatPresent(listDbUserCatogory) && !AreUserCategoriesSame(listDbUserCatogory, dbUser.ListDbUserCategory))
                {
                    if (!listUsersToRemove.Contains(dbUser))
                    {
                        listUsersToRemove.Add(dbUser);
                    }
                }
            }
            listDbUsers = listDbUsers.Except(listUsersToRemove).ToList();
            //listDbUsers = HideUserAtLevel(Config.Hierarchy.District, 30, listDbUsers);
            if (CanDiscartHierarchyUpperThanImmediateOne)
            {
                listDbUsers = DiscartHierarchyUpperThanImmediateOne(listDbUsers, hierarchyId, userHierarchyId);
            }

            return listDbUsers;
        }




        public static List<DbUsers> GetUsersPresentForCurrentHierarchy2(List<DbUsers> listUsers, Config.Hierarchy? hierarchyId, int? hierarchyVal, int? userHierarchyId, int? userCat1, int? userCat2)
        {
            //int hierarchyVal = listHierarchyMapping.Where(n=>n.Item1==hierarchyId).FirstOrDefault().Item2;
            int hierarchyValue = Convert.ToInt32(hierarchyVal.ToString());
            List<DbUsers> listUsersToReturn = null;
            List<DbUserCategory> listComplaintDbUserCategory = DbUserCategory.GetuserCategoryList(userCat1, userCat2);
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && Utility.GetIntList(n.Province_Id).Contains(hierarchyValue) &&
                                n.User_Hierarchy_Id == userHierarchyId &&
                                //n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                                AreUserCategoriesSame(listComplaintDbUserCategory, n.ListDbUserCategory)).ToList();
                    break;
                case Config.Hierarchy.Division:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && Utility.GetIntList(n.Division_Id).Contains(hierarchyValue) && n.User_Hierarchy_Id == userHierarchyId &&
                                //n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                                AreUserCategoriesSame(listComplaintDbUserCategory, n.ListDbUserCategory)).ToList();
                    break;

                case Config.Hierarchy.District:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && Utility.GetIntList(n.District_Id).Contains(hierarchyValue) && n.User_Hierarchy_Id == userHierarchyId &&
                                //n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                                AreUserCategoriesSame(listComplaintDbUserCategory, n.ListDbUserCategory)).ToList();
                    break;

                case Config.Hierarchy.Tehsil:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && Utility.GetIntList(n.Tehsil_Id).Contains(hierarchyValue) && n.User_Hierarchy_Id == userHierarchyId &&
                                //n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                                AreUserCategoriesSame(listComplaintDbUserCategory, n.ListDbUserCategory)).ToList();
                     break;

                case Config.Hierarchy.UnionCouncil:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && Utility.GetIntList(n.UnionCouncil_Id).Contains(hierarchyValue) && n.User_Hierarchy_Id == userHierarchyId &&
                                //n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                                AreUserCategoriesSame(listComplaintDbUserCategory, n.ListDbUserCategory)).ToList();
                    break;

                case Config.Hierarchy.Ward:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && Utility.GetIntList(n.Ward_Id).Contains(hierarchyValue) && n.User_Hierarchy_Id == userHierarchyId &&
                                //n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                                AreUserCategoriesSame(listComplaintDbUserCategory, n.ListDbUserCategory)).ToList();
                    break;
            }

            return listUsersToReturn;
        }



        //-------------------------- End ----------------------------------

        private static List<DbUsers> HideUserAtLevel(Config.Hierarchy hierarchyId, int userHirarchyId, List<DbUsers> listDbUsers )
        {
            List<DbUsers> listDbUsersToRemove =
                listDbUsers.Where(
                    n => n.Hierarchy_Id == hierarchyId && n.User_Hierarchy_Id == userHirarchyId).ToList();
            listDbUsers = listDbUsers.Except(listDbUsersToRemove).ToList();
            return listDbUsers;
        }

        private static List<DbUsers> DiscartHierarchyLowerThanImmediateOne(List<DbUsers> listDbUsers, Config.Hierarchy currHierarchy, int? currUserHierarchyId)
        {

            DbUsers dbUser = listDbUsers.Where(n => n.Hierarchy_Id > currHierarchy).OrderBy(n => n.Hierarchy_Id).ThenByDescending(n=>n.User_Hierarchy_Id).FirstOrDefault();
            int nextHierarchy = (dbUser == null) ? 0 : (int) dbUser.Hierarchy_Id;
            int nextUserHierarchy = (dbUser == null) ? 0 : (int)dbUser.User_Hierarchy_Id; 

            List<DbUsers> listUsersOfSameHierarchy = listDbUsers.Where(n => n.Hierarchy_Id == currHierarchy).ToList();


            bool isLowerUserHierarchy = false;

            int count = listUsersOfSameHierarchy.Count;
            if (count > 0) // user of same hierarchy and userHierarchy Id 
            {
                var userHierarchyGroup = listUsersOfSameHierarchy.OrderByDescending(n=>n.User_Hierarchy_Id).GroupBy(n => n.User_Hierarchy_Id);
                foreach (var userGroup in userHierarchyGroup)
                {
                    if (userGroup.Key < currUserHierarchyId && !isLowerUserHierarchy)
                    {
                        isLowerUserHierarchy = true;
                        listDbUsers.RemoveAll(n =>  n.Hierarchy_Id == currHierarchy && n.User_Hierarchy_Id < (userGroup.Key));
                    }
                }
            }

            if (!isLowerUserHierarchy)
            {
                listDbUsers.RemoveAll(n => n.Hierarchy_Id > (Config.Hierarchy)nextHierarchy);
                listDbUsers.RemoveAll(n => n.User_Hierarchy_Id < nextUserHierarchy);
            }
            else if (dbUser!=null) // if user exist beneath hierarchy and is not user hierarchy
            {
                listDbUsers.RemoveAll(n => n.Hierarchy_Id >= (Config.Hierarchy)nextHierarchy);
            }

            return listDbUsers;
        }


        public static List<DbUsers> FindUserUpperThanCurrentHierarchy(DbUsers currUser, DbComplaint dbComplaint, int campaignId, Config.Hierarchy hierarchyId, int hierarchyIdVal, bool CanDiscartHierarchyUpperThanImmediateOne)
        {
            
            //DbComplaint dbComplaint = null;
            /*
            if (dbComplaint != null)
            {
                dbComplaint = DbComplaint.GetByComplaintId((int)complaintId);
            }*/
            List<Tuple<int,string>> listHierarchyMappings = new List<Tuple<int, string>>();
            
            if (dbComplaint != null)
            {
                listHierarchyMappings = Utility.GetHierarchyMappingListByComplaint(dbComplaint, (int) hierarchyId);
            }
            else if (currUser != null)
            {
                listHierarchyMappings = Utility.GetHierarchyMappingListByUser(currUser, (int) hierarchyId);
            }

            List<DbUsers> listDbUsers = DbUsers.GetUsersAboveHierarchy(campaignId, listHierarchyMappings);
            //DbUsers currUser = listDbUsers.Where(n => n.User_Id == userId).FirstOrDefault();
            //listDbUsers.RemoveAll(n => n.User_Id == userId);

            List<DbUsers> listUsersToRemove = new List<DbUsers>();
            listUsersToRemove.Add(currUser);

            int? userCategoryId1 = null, userCategoryId2 = null, userHierarchyId = null;
            if (dbComplaint!=null)
            {
                userCategoryId1 = dbComplaint.UserCategoryId1;
                userCategoryId2 = dbComplaint.UserCategoryId2;
                if (dbComplaint.Complaint_Computed_User_Hierarchy_Id == Config.UserMaxHierarchy)
                {
                    userHierarchyId = 0;
                }
                else
                {
                    userHierarchyId = dbComplaint.Complaint_Computed_User_Hierarchy_Id;
                }
            }
            else if (currUser != null)
            {
                userCategoryId1 = currUser.UserCategoryId1;
                userCategoryId2 = currUser.UserCategoryId2;
                userHierarchyId = currUser.User_Hierarchy_Id;
            }

            foreach (DbUsers dbUser in listDbUsers)
            {
                if (dbUser.Hierarchy_Id == hierarchyId && ( /*dbUser.User_Hierarchy_Id==null || */(dbUser.User_Hierarchy_Id != null && dbUser.User_Hierarchy_Id >= userHierarchyId))) // if users are above same hierarchy
                {
                    listUsersToRemove.Add(dbUser);
                }

                if ((userCategoryId1 != null && dbUser.UserCategoryId1 != null && dbUser.UserCategoryId1 != userCategoryId1) ||
                    (userCategoryId2 != null && dbUser.UserCategoryId2 != null && dbUser.UserCategoryId2 != userCategoryId2))// if user category are not same
                {
                    if (!listUsersToRemove.Contains(dbUser))
                    {
                        listUsersToRemove.Add(dbUser);
                    }
                }
            }
            listDbUsers = listDbUsers.Except(listUsersToRemove).ToList();
            //listDbUsers = HideUserAtLevel(Config.Hierarchy.District, 30, listDbUsers);
            if (CanDiscartHierarchyUpperThanImmediateOne)
            {
                listDbUsers = DiscartHierarchyUpperThanImmediateOne(listDbUsers, hierarchyId, userHierarchyId);
            }

            return listDbUsers;
        }

        public static List<DbUsers> GetUsersHierarchyMapping(int campaignId)
        {
            return DbUsers.GetActiveUserAgainstParams(campaignId)
                .OrderByDescending(n => n.Hierarchy_Id)
                .ThenBy(n => n.User_Hierarchy_Id)
                .ToList();
        }

        public static List<DbUsers> GetUsersHierarchyMapping(DBContextHelperLinq db, int campaignId)
        {
            return DbUsers.GetActiveUserAgainstParams(db, campaignId)
                .OrderByDescending(n => n.Hierarchy_Id)
                .ThenBy(n => n.User_Hierarchy_Id)
                .ToList();
        }

        public static List<DbUsers> GetUsersPresentForCurrentHierarchy(List<DbUsers> listUsers, Config.Hierarchy? hierarchyId, int? hierarchyVal, int? userHierarchyId, int? userCat1, int? userCat2)
        {
            //int hierarchyVal = listHierarchyMapping.Where(n=>n.Item1==hierarchyId).FirstOrDefault().Item2;
            string hierarchyStr = hierarchyVal.ToString();
            List<DbUsers> listUsersToReturn = null;
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && n.Province_Id==hierarchyStr  && n.User_Hierarchy_Id == userHierarchyId &&
                                n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                break;
                case Config.Hierarchy.Division:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && n.Division_Id == hierarchyStr && n.User_Hierarchy_Id == userHierarchyId &&
                                n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();
                break;

                case Config.Hierarchy.District:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && n.District_Id == hierarchyStr && n.User_Hierarchy_Id == userHierarchyId &&
                                n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();        

                break;

                case Config.Hierarchy.Tehsil:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && n.Tehsil_Id == hierarchyStr && n.User_Hierarchy_Id == userHierarchyId &&
                                n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();     
                break;

                case Config.Hierarchy.UnionCouncil:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && n.UnionCouncil_Id == hierarchyStr && n.User_Hierarchy_Id == userHierarchyId &&
                                n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();     

                break;

                case Config.Hierarchy.Ward:
                    listUsersToReturn =
                        listUsers.Where(
                            n =>
                                n.Hierarchy_Id == hierarchyId && n.Ward_Id == hierarchyStr && n.User_Hierarchy_Id == userHierarchyId &&
                                n.UserCategoryId1 == userCat1 && n.UserCategoryId2 == userCat2).ToList();  
                break;
            }

            return listUsersToReturn;
        }

        public static List<DbUsers> GetUsersPresentForUpperUserHierarchy(List<Tuple<Config.Hierarchy?, int>> listHierarchyMapping, List<DbUsers> listUsers, Config.Hierarchy? hierarchyId, int? userHierarchyId)
        {
            int hierarchyVal = 0;
            string hierarchyStr = "";
            List<DbUsers> listUsersToReturn = null;
            List<DbUsers> listUsersFinal = new List<DbUsers>();
            int count = 0;
            while (Convert.ToInt32(hierarchyId) > 2)
            {
                hierarchyVal = listHierarchyMapping.Where(n => n.Item1 == hierarchyId).FirstOrDefault().Item2;
                hierarchyStr = hierarchyVal.ToString();
                if (count > 0)
                {
                    userHierarchyId = -1;
                }
                switch (hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        //listUsersToReturn = new List<DbUsers>();
                            listUsers.Where(
                                n =>
                                    n.Hierarchy_Id == hierarchyId && n.Province_Id == hierarchyStr &&
                                    n.User_Hierarchy_Id > userHierarchyId).ToList();
                        break;
                    case Config.Hierarchy.Division:
                        listUsersToReturn =
                            listUsers.Where(
                                n =>
                                    n.Hierarchy_Id == hierarchyId && n.Division_Id == hierarchyStr &&
                                    n.User_Hierarchy_Id > userHierarchyId).ToList();
                        break;

                    case Config.Hierarchy.District:
                        listUsersToReturn =
                            listUsers.Where(
                                n =>
                                    n.Hierarchy_Id == hierarchyId && n.District_Id == hierarchyStr &&
                                    n.User_Hierarchy_Id > userHierarchyId).ToList();

                        break;

                    case Config.Hierarchy.Tehsil:
                        listUsersToReturn =
                            listUsers.Where(
                                n =>
                                    (n.Hierarchy_Id == hierarchyId &&
                                    n.User_Hierarchy_Id > userHierarchyId) && n.Tehsil_Id == hierarchyStr).ToList();
                        break;

                    case Config.Hierarchy.UnionCouncil:
                        listUsersToReturn =
                            listUsers.Where(
                                n =>
                                    n.Hierarchy_Id == hierarchyId && n.UnionCouncil_Id == hierarchyStr &&
                                    n.User_Hierarchy_Id > userHierarchyId).ToList();

                        break;

                    case Config.Hierarchy.Ward:
                        listUsersToReturn =
                            listUsers.Where(
                                n =>
                                    n.Hierarchy_Id == hierarchyId && n.Ward_Id == hierarchyStr ||
                                    n.User_Hierarchy_Id > userHierarchyId).ToList();
                        break;
                }

                hierarchyId--;
                count ++;
                listUsersFinal.AddRange(listUsersToReturn);
            }
            listUsersFinal = listUsersFinal.OrderByDescending(n => n.Hierarchy_Id)
                .ThenBy(n => n.User_Hierarchy_Id)
                .ToList();
            return listUsersFinal;
        }

        private static List<Tuple<int, string>> GetHierarchyMappings(DbUsers dbUser, DbComplaint dbComplaint, int hierarchyId)
        {
            List<Tuple<int, string>> listHierarchyMappings = new List<Tuple<int, string>>();

            if (dbComplaint != null)
            {
                listHierarchyMappings = Utility.GetHierarchyMappingListByComplaint(dbComplaint, hierarchyId);
            }
            else if (dbUser != null)
            {
                listHierarchyMappings = Utility.GetHierarchyMappingListByUser(dbUser, hierarchyId);
            }
            return listHierarchyMappings;
        }

        private static List<DbUsers> DiscartHierarchyUpperThanImmediateOne(List<DbUsers> listDbUsers, Config.Hierarchy currHierarchy, int? currUserHierarchyId)
        {

            DbUsers dbUser = listDbUsers.Where(n => n.Hierarchy_Id < currHierarchy).OrderByDescending(n => n.Hierarchy_Id).ThenBy(n => n.User_Hierarchy_Id).FirstOrDefault();
            int nextHierarchy = (dbUser == null) ? 0 : (int)dbUser.Hierarchy_Id;
            int nextUserHierarchy = (dbUser == null) ? 0 : (int)dbUser.User_Hierarchy_Id;

            List<DbUsers> listUsersOfSameHierarchy = listDbUsers.Where(n => n.Hierarchy_Id == currHierarchy).ToList();


            bool isHigherHierarchy = false;

            int count = listUsersOfSameHierarchy.Count;
            if (count > 0) // user of same hierarchy and userHierarchy Id 
            {
                var userHierarchyGroup = listUsersOfSameHierarchy.OrderBy(n => n.User_Hierarchy_Id).GroupBy(n => n.User_Hierarchy_Id);
                foreach (var userGroup in userHierarchyGroup)
                {
                    if (userGroup.Key > currUserHierarchyId && !isHigherHierarchy)
                    {
                        isHigherHierarchy = true;
                        listDbUsers.RemoveAll(n => n.Hierarchy_Id == currHierarchy && n.User_Hierarchy_Id > (userGroup.Key));
                    }
                }
            }

            if (!isHigherHierarchy)
            {
                listDbUsers.RemoveAll(n => n.Hierarchy_Id < (Config.Hierarchy)nextHierarchy);
                listDbUsers.RemoveAll(n => n.User_Hierarchy_Id > nextUserHierarchy);
            }
            else
            {
                listDbUsers.RemoveAll(n => n.Hierarchy_Id <= (Config.Hierarchy)nextHierarchy);
            }

            return listDbUsers;
        }

    }
}