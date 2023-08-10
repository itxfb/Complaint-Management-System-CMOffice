using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Web;
using AutoMapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;

namespace PITB.CMS.Handler.StakeHolder
{
    public class SchoolEducationUsersHandler
    {

        public static void DumpUserWiseSupervisorMapping(int hierarchyId, int campaignId, int maxHierarchy)
        {
            try
            {
                DbUserWiseSupervisorMapping.BulkMerge(new List<DbUserWiseSupervisorMapping>());

                List<DbUserWiseSupervisorMapping> listDbUserWiseSupervisorMappings =
                    GetUserWiseSupervisorMappings(hierarchyId, campaignId, maxHierarchy);
                DbUserWiseSupervisorMapping.DeleteEntries(campaignId);
                DbUserWiseSupervisorMapping.BulkMerge(listDbUserWiseSupervisorMappings);
                /*using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    for (int i = 0; i < listDbUserWiseSupervisorMappings.Count; i++)
                    {
                        db.DbUserWiseSupervisorMapping.Add(listDbUserWiseSupervisorMappings[i]);
                    }
                    db.SaveChanges();

                }*/
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbUserWiseSupervisorMapping> GetUserWiseSupervisorMappings(int hierarchyId, int campaignId, int maxHierarchy)
        {
            List<DbUserWiseSupervisorMapping> listdbUserWiseSupervisorMapping = new List<DbUserWiseSupervisorMapping>();
            List<DbUsers> listDbUsersLower = null;

            List<DbUsers> listDbUsers = DbUsers.GetActiveUserAgainstParams(campaignId);
            List<DbUsers> listDbUsersCurrentHierarchy = listDbUsers.Where(n => n.Hierarchy_Id == (Config.Hierarchy?) hierarchyId).ToList();
            DbUsers dbUser = new DbUsers();

            DbUserWiseSupervisorMapping dbUserWiseSupervisorMap = null;

            for (int i = 0; i < 1/*listDbUsersCurrentHierarchy.Count*/; i++)
            {
                Mapper.CreateMap<DbUsers, DbUserWiseSupervisorMapping>();
                dbUserWiseSupervisorMap = Mapper.Map<DbUserWiseSupervisorMapping>(listDbUsersCurrentHierarchy[i]);
                dbUserWiseSupervisorMap.UserIdFk = listDbUsersCurrentHierarchy[i].User_Id;
                dbUserWiseSupervisorMap.UserSupervisorIdFk = null;
                listdbUserWiseSupervisorMapping.Add(dbUserWiseSupervisorMap); 
            }


            for (int i = 0; i < listDbUsersCurrentHierarchy.Count; i++)
            {
                listDbUsersLower = new List<DbUsers>();
                dbUser = listDbUsersCurrentHierarchy[i];
                PopulateUserSupervisorRecursively(dbUser, listDbUsersLower, listDbUsers, listdbUserWiseSupervisorMapping, maxHierarchy);
            }

            return listdbUserWiseSupervisorMapping;
        }

        private static void PopulateUserSupervisorRecursively(DbUsers dbUser, List<DbUsers> listDbUsersLower, List<DbUsers> listDbUsers, List<DbUserWiseSupervisorMapping> listdbUserWiseSupervisorMapping, int maxHierarchy)
        {
            List<DbUsers> listDbUsersLowerTemp = UsersHandler.FindUserLowerThanCurrentHierarchy2(dbUser.User_Id, CMS.Utility.GetIntByCommaSepStr(dbUser.Campaigns),
                 (CMS.Config.Hierarchy)dbUser.Hierarchy_Id, CMS.Utility.GetIntByCommaSepStr(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)), true, listDbUsers,null);

            /*if (listDbUsersLowerTemp.Count == 0)
            {
                
            }*/
            //listDbUsersLower.AddRange(listDbUsersLowerTemp);
            DbUserWiseSupervisorMapping dbUserWiseSupervisorMap = null;
            for (int i = 0; i < listDbUsersLowerTemp.Count; i++)
            {
                if ((int)listDbUsersLowerTemp[i].Hierarchy_Id < maxHierarchy)
                {
                    listDbUsersLower.Add(listDbUsersLowerTemp[i]);
                }

                //if ((int) listDbUsersLowerTemp[i].Hierarchy_Id <= maxHierarchy)
                {
                    Mapper.CreateMap<DbUsers, DbUserWiseSupervisorMapping>();
                    dbUserWiseSupervisorMap = Mapper.Map<DbUserWiseSupervisorMapping>(listDbUsersLowerTemp[i]);
                    dbUserWiseSupervisorMap.UserIdFk = listDbUsersLowerTemp[i].User_Id;
                    dbUserWiseSupervisorMap.UserSupervisorIdFk = dbUser.User_Id;
                    listdbUserWiseSupervisorMapping.Add(dbUserWiseSupervisorMap);
                }
            }
            if (listDbUsersLower.Count == 1)
            {
                
            }
            if (listDbUsers.Count > 0 && listDbUsersLower != null && listDbUsersLower.Count > 0 /*&&
                (int) dbUser.Hierarchy_Id < maxHierarchy*/)
            {
                dbUser = listDbUsersLower.First();
                listDbUsersLower.Remove(dbUser);
                PopulateUserSupervisorRecursively(dbUser, listDbUsersLower, listDbUsers, listdbUserWiseSupervisorMapping,
                    maxHierarchy);
            }
            else
            {
                return;
            }
        }


        /*
        public static void EtlAllPmiuUsers()
        {
            List<DbUsers> listDbUsers = DbUsers.GetUserAgainstCampaign(47).ToList();
            var cnicGroups = listDbUsers.GroupBy(n => n.Cnic).ToList();

            string cnic = "";
            List<DbUsers> listDbUsersTemp = null, listDbUsersLogedIn;

            foreach (var cnicGroup in cnicGroups)
            {
                cnic = cnicGroup.Key;
                listDbUsersTemp = listDbUsers.Where(n => n.Cnic == cnic && cnic!=null && n.Username.Contains(n.Cnic)).OrderBy(n=>n.LastLoginDate).ToList();
                listDbUsersLogedIn = listDbUsersTemp.Where(n => n.LastLoginDate != null).ToList();
                for(int i=0; i < )
            }



            for (int i=0; i<listDbUsers.Count; i++)
            {
                listDbUsers
            }
        }*/
    }
}