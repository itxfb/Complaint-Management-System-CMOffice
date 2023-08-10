using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Handler.Permission
{
    public class PermissionHandler
    {
        public static bool IsPermissionAllowedInCampagin(Config.CampaignPermissions campaignPermission, CMSCookie cookie = null)
        {
            cookie = (cookie==null)? AuthenticationHandler.GetCookie(): cookie;
            if (cookie.Role == Config.Roles.Stakeholder)
            {
                List<int?> campaignIds = Utility.GetNullableIntList(cookie.Campaigns);

                List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.Campaign, campaignIds,
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

                List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, cookie.UserId,
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