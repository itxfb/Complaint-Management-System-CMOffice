using System;
using System.Collections.Generic;
using PITB.CMS;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;


namespace PITB.CRM_API.Models.API.SchoolEducation.Resolver
{
    public class SEResponseStakeholderLogin: ApiStatus
    {
        public List<SELoginsInfo> ListLoginsInfo { get; set; }
        public SEResponseStakeholderLogin(List<DbUsers> listDbUsers, ApiStatus apiStatus)
        {
            ListLoginsInfo = new List<SELoginsInfo>();
            if (listDbUsers != null)
            {
                SELoginsInfo loginInfo = null;

                foreach (DbUsers dbUser in listDbUsers)
                {
                    loginInfo = new SELoginsInfo(dbUser);
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

        public SEResponseStakeholderLogin()
        {
            
        }

    }



    public class SELoginsInfo
    {
        public string Username { get; set; }

        public string Info { get; set; }

        public int Role { get; set; }

        public string PhoneNo { get; set; }

        public string Name { get; set; }

        public string Designation { get; set; }

        public SELoginsInfo(DbUsers dbUser)
        {
            string infoStr = "";
            this.Username = dbUser.Username;
            //infoStr = infoStr + DbCampaign.GetById(Convert.ToInt32(dbUser.Campaigns)).Campaign_Name+"_";
            //infoStr = infoStr + ((Config.Hierarchy)dbUser.Hierarchy_Id).GetDisplayName() + "_";
            //infoStr = infoStr + dbUser.Designation;
            infoStr = infoStr  + 
                      CMS.Utility.GetHierarchyValueName((CMS.Config.Hierarchy) dbUser.Hierarchy_Id,
                          CMS.Utility.GetIntByCommaSepStr(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)));
            //infoStr = infoStr + Username;
            Role = Convert.ToInt32(dbUser.SubRole_Id);
            PhoneNo = dbUser.Phone;
            Name = dbUser.Name;
            Designation = dbUser.Designation;
            this.Info = infoStr;
        }
    }

    //public class ResponseStakeholderLogin : ApiStatus
    //{
    //    public List<LoginsInfo> ListLoginsInfo { get; set; }
    //    public ResponseStakeholderLogin(List<DbUsers> listDbUsers, ApiStatus apiStatus)
    //    {
    //        ListLoginsInfo = new List<LoginsInfo>();
    //        if (listDbUsers != null)
    //        {
    //            LoginsInfo loginInfo = null;

    //            foreach (DbUsers dbUser in listDbUsers)
    //            {
    //                loginInfo = new LoginsInfo(dbUser);
    //                ListLoginsInfo.Add(loginInfo);
    //            }
    //            this.Message = apiStatus.Message;
    //            this.Status = apiStatus.Status;
    //        }
    //        else
    //        {
    //            this.Message = apiStatus.Message;
    //            this.Status = apiStatus.Status;
    //        }
    //    }

    //}



    //public class LoginsInfo
    //{
    //    public string Username { get; set; }
    //    public Config.UserHirerchyLwmc HierarchyId { get; set; }

    //    public string HierarchyName
    //    {
    //        get { return HierarchyId.ToString(); }
    //    }

    //    public string Info { get; set; }

    //    public LoginsInfo(DbUsers dbUser)
    //    {
    //        HierarchyId = Config.UserHirerchyLwmc.None;
    //        string infoStr = "";
    //        this.Username = dbUser.Username;
    //        //infoStr = infoStr + DbCampaign.GetById(Convert.ToInt32(dbUser.Campaigns)).Campaign_Name+"_";
    //        infoStr = infoStr + ((Config.Hierarchy)dbUser.Hierarchy_Id).GetDisplayName() + "_";
    //        //infoStr = infoStr + dbUser.Designation;
    //        infoStr = infoStr + Username;
    //        this.Info = infoStr;



    //        int userHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);
    //        if (userHierarchyId == (int)Config.LwmcResolverHirarchyId.Zoner)
    //        {
    //            HierarchyId = Config.UserHirerchyLwmc.Zoner;
    //        }
    //        else if (userHierarchyId == (int)Config.LwmcResolverHirarchyId.Am)
    //        {
    //            HierarchyId = Config.UserHirerchyLwmc.AM;

    //        }
    //        else if (userHierarchyId >= (int)Config.LwmcResolverHirarchyId.HigherLevelInitial)
    //        {
    //            HierarchyId = Config.UserHirerchyLwmc.HigherManagement;

    //        }



    //    }
    //}
}