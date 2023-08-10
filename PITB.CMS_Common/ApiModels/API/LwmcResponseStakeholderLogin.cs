using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;


namespace PITB.CMS_Common.ApiModels.API
{
    public class LwmcResponseStakeholderLogin : ApiStatus
    {
        public List<LwmcLoginsInfo> ListLoginsInfo { get; set; }

        public LwmcResponseStakeholderLogin()
        {
            ListLoginsInfo = new List<LwmcLoginsInfo>();
        }

        public LwmcResponseStakeholderLogin(List<DbUsers> listDbUsers, ApiStatus apiStatus)
        {
            ListLoginsInfo = new List<LwmcLoginsInfo>();
            if (listDbUsers != null)
            {
                LwmcLoginsInfo loginInfo = null;

                foreach (DbUsers dbUser in listDbUsers)
                {
                    loginInfo = new LwmcLoginsInfo(dbUser);
                    ListLoginsInfo.Add(loginInfo);
                }
                this.Message = apiStatus.Message;
                this.Status = apiStatus.Status;
            }
            else
            {
                this.Message = apiStatus.Message;
                this.Status = apiStatus.Status;
            }
        }

    }



    public class LwmcLoginsInfo
    {
        public string Username { get; set; }
        public Config.UserHirerchyLwmc HierarchyId { get; set; }

        public string HierarchyName
        {
            get { return HierarchyId.ToString(); }
        }

        public string Info { get; set; }

        public LwmcLoginsInfo(DbUsers dbUser)
        {
            HierarchyId = Config.UserHirerchyLwmc.None;
            string infoStr = "";
            this.Username = dbUser.Username;
            //infoStr = infoStr + DbCampaign.GetById(Convert.ToInt32(dbUser.Campaigns)).Campaign_Name+"_";
            infoStr = infoStr + ((Config.Hierarchy)dbUser.Hierarchy_Id).GetDisplayName() + "_";
            //infoStr = infoStr + dbUser.Designation;
            infoStr = infoStr + Username;
            this.Info = infoStr;



            int userHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);
            if (userHierarchyId == (int)Config.LwmcResolverHirarchyId.Zoner)
            {
                HierarchyId = Config.UserHirerchyLwmc.Zoner;
            }
            else if (userHierarchyId == (int)Config.LwmcResolverHirarchyId.Am)
            {
                HierarchyId = Config.UserHirerchyLwmc.AM;

            }
            else if (userHierarchyId >= (int)Config.LwmcResolverHirarchyId.HigherLevelInitial)
            {
                HierarchyId = Config.UserHirerchyLwmc.HigherManagement;

            }



        }
    }
}