using System;
using System.Collections.Generic;
using PITB.CMS;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Models.API
{
    public class ZimmedarShehriModel
    {
        public class LoginRequest
        {
            public string Password { get; set; }
            public string Username { get; set; }
            public string ImeiNo { get; set; }
        }




        public class LoginResponseModel : ApiStatus
        {
            public List<LoginsInfo> ListLoginsInfo { get; set; }
            public LoginResponseModel(List<DbUsers> listDbUsers, ApiStatus apiStatus)
            {
                ListLoginsInfo = new List<LoginsInfo>();
                if (listDbUsers != null)
                {
                    LoginsInfo loginInfo = null;

                    foreach (DbUsers dbUser in listDbUsers)
                    {
                        loginInfo = new LoginsInfo(dbUser);
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



        public class StatusRequestModel
        {
            public string UserName { get; set; }
            public string ComplaintId { get; set; }
            public int StatusId { get; set; }
            public string StatusComments { get; set; }

            public List<Picture> PicturesList { get; set; }
        }




        public class ComplaintsRequestModel
        {
            public string UserName { get; set; }
            public string Statuses { get; set; }
            public int StartRowIndex { get; set; }

        }

        public class StatusesModelResponse : ApiStatus
        {
            public List<DbStatuses> ListFilterStatus { get; set; }

            public List<DbStatuses> ListAssignableStatus { get; set; }
        }



        public class LoginsInfo
        {
            public string Username { get; set; }

            public string Info { get; set; }

            public int Role { get; set; }

            public string PhoneNo { get; set; }

            public string Name { get; set; }

            public string Designation { get; set; }

            public LoginsInfo(DbUsers dbUser)
            {
                string infoStr = "";
                this.Username = dbUser.Username;
                //infoStr = infoStr + DbCampaign.GetById(Convert.ToInt32(dbUser.Campaigns)).Campaign_Name+"_";
                //infoStr = infoStr + ((Config.Hierarchy)dbUser.Hierarchy_Id).GetDisplayName() + "_";
                //infoStr = infoStr + dbUser.Designation;
                infoStr = infoStr +
                          CMS.Utility.GetHierarchyValueName((CMS.Config.Hierarchy)dbUser.Hierarchy_Id,
                              CMS.Utility.GetIntByCommaSepStr(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)));
                //infoStr = infoStr + Username;
                Role = Convert.ToInt32(dbUser.SubRole_Id);
                PhoneNo = dbUser.Phone;
                Name = dbUser.Name;
                Designation = dbUser.Designation;
                this.Info = infoStr;
            }
        }
    }
}