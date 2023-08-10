using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Handlers.Authentication;
using PITB.CMS_Handlers.DB.Repository;
using PITB.CMS_Models.Custom;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.Permission
{
    public class PermissionHandler
    {
        public static bool IsPermissionAllowedInCampagin(Config.CampaignPermissions campaignPermission, CMSCookie cookie = null)
        {
            cookie = (cookie==null)? AuthenticationHandler.GetCookie(): cookie;
            if (cookie.Role == Config.Roles.Stakeholder)
            {
                List<int?> campaignIds = Utility.GetNullableIntList(cookie.Campaigns);

                List<DbPermissionsAssignment> listPermissions = RepoDbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.Campaign, campaignIds,
                    (int) campaignPermission);

                if (listPermissions.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }


        public static bool IsPermissionAllowedInUser(Config.Permissions userPermission, CMSCookie cookie=null)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            cookie = (cookie == null) ? AuthenticationHandler.GetCookie() : cookie;
            if (cookie.Role == Config.Roles.Stakeholder)
            {
                //int campaignId = Convert.ToInt32(cookie.Campaigns);

                List<DbPermissionsAssignment> listPermissions = RepoDbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, cookie.UserId,
                    (int)userPermission);

                if (listPermissions.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}