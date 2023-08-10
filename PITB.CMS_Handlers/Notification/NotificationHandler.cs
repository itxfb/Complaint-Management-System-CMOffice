using System;
using System.Collections.Generic;
using System.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Handlers.Business;
using PITB.CMS_Handlers.DB.Repository;
using PITB.CMS_Models.DB;
using PITB.CMS_Models.View;

namespace PITB.CMS_Handlers.Notification
{
    public class NotificationHandler
    {
        public static List<DbNotificationLogs> GetNotificationCount(Config.NotificationType notificationType, int campaignId, Config.NotificationStatus notificationStatus)
        {
            DbUsers dbUser = Utility.GetUserFromCookie();
            string from = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            string to = @DateTime.Now.ToString("yyyy-MM-dd");
            List<DbPermissionsAssignment> listPermissionsAssignment = RepoDbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int) Config.PermissionsType.User,
                (int) dbUser.User_Id, (int) Config.Permissions.StatusesForComplaintListing);
            List<DbStatus> listDbStatus = BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns),dbUser.User_Id, listPermissionsAssignment);
            List<VmStakeholderComplaintListing> listVmComplaints = null;
            //List<DbUserWiseNotification> listDbUserWiseNotification = null;
            List<DbNotificationLogs> listDbNotifications = null;
            if ((int) Config.Campaign.SchoolEducationEnhanced == Utility.GetIntByCommaSepStr(dbUser.Campaigns))
            {
                listVmComplaints =
                    BlSchool.GetStakeHolderServerSideListDenormalized(
                        from,
                        to,
                        null,
                        dbUser.Campaigns,
                        dbUser.Categories,
                        Utility.GetCommaSepStrFromList(listDbStatus.Select(n => n.Complaint_Status_Id).ToList()),
                        Config.TransferStr,
                        (int)Config.ComplaintType.Complaint,
                        (Config.StakeholderComplaintListingType.AssignedToMe),
                        "ComplaintsAssignedToUser").ToList<VmStakeholderComplaintListing>();

                //listNotificationLogs = DbNotificationLogs.Get(campaignId,
                //    (int)notificationType, (int)notificationStatus);
                
            }
            else
            {
                listVmComplaints =
                    BlComplaints.GetStakeHolderServerSideListDenormalized(
                        from,
                        to,
                        null,
                        dbUser.Campaigns,
                        dbUser.Categories,
                        Utility.GetCommaSepStrFromList(listDbStatus.Select(n => n.Complaint_Status_Id).ToList()),
                        Config.TransferStr,
                        (int)Config.ComplaintType.Complaint,
                        (Config.StakeholderComplaintListingType.AssignedToMe),
                        "ComplaintsAssignedToUser").ToList<VmStakeholderComplaintListing>();

                
            }
            listDbNotifications = BlNotification.GetUserWiseNotification(campaignId,
                    (int)notificationType, listVmComplaints.Select(n => n.ComplaintId).ToList());

            //listComplaintIds = BlNotification.GetSubtraction(listDbUserWiseNotification, listVmComplaints);

            return listDbNotifications;
        }

        /*
        private static List<DbNotificationLogs> GetIntersection(List<DbNotificationLogs> listNotificationLogs, List<VmStakeholderComplaintListing> listComplaints)
        {
            List<int> listComplaintIds = listComplaints.Select(n => n.ComplaintId).ToList();
            //listNotificationLogs.RemoveAll(x => listComplaintIds.Contains(x.Id));
            listNotificationLogs = listNotificationLogs.Where(n => listComplaintIds.Contains((int)n.TypeId)).ToList();
            return listNotificationLogs;
        }
        */ 

        /*
        public static int GetUserNotificationCount(Config.NotificationType notificationType, int campaignId, Config.NotificationStatus notificationStatus)
        {
            DbUsers dbUser = Utility.GetUserFromCookie();
            string from = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            string to = @DateTime.Now.ToString("yyyy-MM-dd");
            List<DbPermissionsAssignment> listPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.User,
                (int)dbUser.User_Id, (int)Config.Permissions.StatusesForComplaintListing);
            List<DbStatus> listDbStatus = BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.User_Id, listPermissionsAssignment);
            List<VmStakeholderComplaintListing> listVmComplaints = null;
            List<DbNotificationLogs> listNotificationLogs = null;
            if ((int)Config.Campaign.SchoolEducationEnhanced == Utility.GetIntByCommaSepStr(dbUser.Campaigns))
            {
                listVmComplaints =
                    BlSchool.GetStakeHolderServerSideListDenormalized(
                        from,
                        to,
                        null,
                        dbUser.Campaigns,
                        dbUser.Categories,
                        Utility.GetCommaSepStrFromList(listDbStatus.Select(n => n.Complaint_Status_Id).ToList()),
                        Config.TransferStr,
                        (int)Config.ComplaintType.Complaint,
                        (Config.StakeholderComplaintListingType.AssignedToMe),
                        "ComplaintsAssignedToUser").ToList<VmStakeholderComplaintListing>();

                listNotificationLogs = DbNotificationLogs.Get(campaignId,
                    (int)notificationType, (int)notificationStatus);

            }
            else
            {
                listVmComplaints =
                    BlComplaints.GetStakeHolderServerSideListDenormalized(
                        from,
                        to,
                        null,
                        dbUser.Campaigns,
                        dbUser.Categories,
                        Utility.GetCommaSepStrFromList(listDbStatus.Select(n => n.Complaint_Status_Id).ToList()),
                        Config.TransferStr,
                        (int)Config.ComplaintType.Complaint,
                        (Config.StakeholderComplaintListingType.AssignedToMe),
                        "ComplaintsAssignedToUser").ToList<VmStakeholderComplaintListing>();

                listNotificationLogs = DbNotificationLogs.Get(campaignId,
                    (int)notificationType, (int)notificationStatus);

            }
            listNotificationLogs = GetIntersection(listNotificationLogs, listVmComplaints);

            return listNotificationLogs.Count;
        }
        */

        
    }
}