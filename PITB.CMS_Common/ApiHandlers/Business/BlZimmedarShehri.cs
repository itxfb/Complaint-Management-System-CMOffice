using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.ApiHandlers.Messages;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.ApiHandlers.Translation;
using PITB.CMS_Common.Handler.Messages;

namespace PITB.CMS_Common.ApiHandlers.Business
{
    public class BlZimmedarShehri
    {
        public static ZimmedarShehriModel.LoginResponseModel SubmitStakeholderLogin(ZimmedarShehriModel.LoginRequest submitStakeHolderLoginRequest, Config.PlatformID platformId)
        {

            DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLoginRequest.Username, submitStakeHolderLoginRequest.Password);
            bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            DbUsers dbUserTemp = null;

            if (isUsernamePresent) // username and cnic present
            {
                string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLoginRequest.Username);
                if (imeiNo == null) // if imei not registered then register it
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbUsers> listDbUser = new List<DbUsers>();
                    listDbUser.Add(user); //DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    foreach (DbUsers dbUser in listDbUser)
                    {
                        dbUser.Imei_No = submitStakeHolderLoginRequest.ImeiNo;
                        db.DbUsers.Attach(dbUser);
                        db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                        db.SaveChanges();
                        dbUserTemp = dbUser;
                    }
                    if (dbUserTemp != null)
                    {
                        TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                    }

                    return new ZimmedarShehriModel.LoginResponseModel(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLoginRequest.Username, submitStakeHolderLoginRequest.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //---- end old code ----
                    List<DbUsers> listDbUser = new List<DbUsers>();
                    listDbUser.Add(user); //DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    return new ZimmedarShehriModel.LoginResponseModel(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new ZimmedarShehriModel.LoginResponseModel(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "This user is already registered on another device"));
                }
            }
            else // if username not present
            {
                return new ZimmedarShehriModel.LoginResponseModel(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
            }

        }


        public static ZimmedarShehriModel.LoginResponseModel SubmitStakeholderLoginImeiNoRestriction(ZimmedarShehriModel.LoginRequest submitStakeHolderLogin, Config.PlatformID platformId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password, db).FirstOrDefault();
            bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            DbUsers dbUserTemp = null;

            // For IOS platform
            /*if (platformId == Config.PlatformID.IOS)
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbUsers> listDbUser = DbUsers.GetByCnic("3520256571439", db);
                foreach (DbUsers dbUser in listDbUser)
                {
                    dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                    db.DbUsers.Attach(dbUser);
                    db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    dbUserTemp = dbUser;
                }
                if (dbUserTemp != null)
                {
                    TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                }

                return new ResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
            }
             */
            // End for IOS platform


            if (isUsernamePresent) // username and cnic present
            {
                string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLogin.Username);
                //if (imeiNo == null) // if imei not registered then register it
                {

                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //listDbUser.Add(user);
                    //---- end old code ----

                    List<DbUsers> listDbUser = null;//DbUsers.GetByCnic(user.Cnic, db);
                    if (user.Cnic == null)
                    {
                        listDbUser = new List<DbUsers>();
                        listDbUser.Add(user);
                        //listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    }
                    else
                    {
                        listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    }
                    //listDbUser.Add(user);

                    for (int i = 0; i < listDbUser.Count; i++)
                    {
                        listDbUser[i].Imei_No = submitStakeHolderLogin.ImeiNo;
                        db.DbUsers.Attach(listDbUser[i]);
                        db.Entry(listDbUser[i]).Property(x => x.Imei_No).IsModified = true;

                        dbUserTemp = listDbUser[i];
                    }
                    db.SaveChanges();
                    if (dbUserTemp != null)
                    {
                        TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                    }

                    return new ZimmedarShehriModel.LoginResponseModel(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                /*else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //---- end old code ----
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    return new SEResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "This user is already registered on another device"));
                }*/
            }
            else // if username not present
            {
                return new ZimmedarShehriModel.LoginResponseModel(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
            }

        }


        public static ZimmedarShehriModel.StatusesModelResponse GetStakeholderValidStatuses(string userName, Config.Language language,
            Config.AppID appId, Config.PlatformID platformId, int appVersionId)
        {
            ZimmedarShehriModel.StatusesModelResponse zhStatusModel = new ZimmedarShehriModel.StatusesModelResponse();


            DbUsers dbUser = DbUsers.GetByUserName(userName);
            if (dbUser != null)
            {
                //DbUsersVersionMapping.Update_AddVersion(Config.UserType.Resolver, dbUser.Id, (int)platformId, (int)appId, appVersionId);

                //List<DbStatuses> listDbStatuses = DbStatuses.GetByCampaignId(Convert.ToInt32(dbUser.Campaigns));

                // Status filter 
                List<DbPermissionsAssignment> listDbPermissionsAssignment = DbPermissionsAssignment
                    .GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, dbUser.Id,
                        (int)Config.Permissions.StatusesForComplaintListing);

                List<DbStatus> listDbStatuses =
                    GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.Id,
                        listDbPermissionsAssignment);

                Dictionary<string, TranslatedModel> translationDict =
                    DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(
                        DbTranslationMapping.GetAllTranslation());
                listDbStatuses.GetTranslatedList<DbStatus>("Status", translationDict, language);

                //StatusList statusList = new StatusList(listDbStatuses);

                zhStatusModel.ListFilterStatus = listDbStatuses;
                //return statusList;


                // Status compaint assigned to me
                DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                    ((int)Config.PermissionsType.User, dbUser.Id, (int)Config.Permissions.StakeholderStatusesOnStatusChangeView
                    ).FirstOrDefault();
                //DbPermissions dbPermission =
                //DbPermissions.GetPermissionsByPermissionAndType(
                // (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
                if (dbPermissionAssignment != null)
                {
                    List<DbStatus> listDbStatus =
                        DbStatus.GetByStatusIds(
                            Utility.GetIntList(dbPermissionAssignment.Permission_Value));
                    zhStatusModel.ListAssignableStatus = listDbStatus;
                    SetAlteredStatus(zhStatusModel.ListAssignableStatus);
                    SetAlteredStatus(zhStatusModel.ListFilterStatus);
                }
                else
                {
                    zhStatusModel.ListAssignableStatus = listDbStatuses;
                }
            }
            zhStatusModel.Message = "Success";
            zhStatusModel.Status = Config.ResponseType.Success.ToString();
            return zhStatusModel;
        }

        public static void SetAlteredStatus(List<DbStatus> listDbStatuses)
        {
            foreach (DbStatus dbStatus in listDbStatuses)
            {
                dbStatus.Status = GetAlterStatusStr(dbStatus.Status);
            }

        }

        public static string GetAlterStatusStr(string statusStr)
        {
            if (statusStr == Config.UnsatisfactoryClosedStatus)
            {
                return Config.SchoolEducationUnsatisfactoryStatus;
            }
            return statusStr;
        }

        public static List<DbStatus> GetStatusListByCampaignIdsAndPermissions(List<int> listCampaignIds, int userId, List<DbPermissionsAssignment> listPermissions)
        {
            DbPermissionsAssignment dbPermission = listPermissions
                                    .FirstOrDefault(
                                        n =>
                                            n.Type == (int)Config.PermissionsType.User &&
                                            n.Type_Id == userId &&
                                            n.Permission_Id == (int)Config.Permissions.StatusesForComplaintListing);

            List<DbStatus> listDbStatuses = null;

            if (dbPermission == null)
            {
                listDbStatuses = DbStatus.GetByCampaignIds(listCampaignIds);
            }
            else
            {
                List<int> listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                List<Config.ComplaintStatus> statusList = listStatuses.Select(status => (Config.ComplaintStatus)(status)).ToList();
                listDbStatuses = DbStatus.GetByStatusIds(statusList);
            }

            return listDbStatuses;
        }
    }
}