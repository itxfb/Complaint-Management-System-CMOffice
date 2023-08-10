using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;

namespace PITB.CMS_Common.Handler.Permission
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


        public static bool IsPermissionAllowedInUser(Config.Permissions userPermission, int userId, Config.Roles roleId/*, DbUsers dbUser*//*, CMSCookie cookie=null*/)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            //cookie = (cookie == null) ? AuthenticationHandler.GetCookie() : cookie;
            if (roleId == Config.Roles.Stakeholder)
            {
                //int campaignId = Convert.ToInt32(cookie.Campaigns);

                List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, userId,
                    (int)userPermission);

                if (listPermissions.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static Dictionary<string, string> GetUserPermissionDict(Config.Permissions userPermission, int userId)
        {
            return Utility.ConvertCollonFormatToDict(GetUserPermissionValue(userPermission, userId));
        }

        public static string GetUserPermissionValue(Config.Permissions userPermission, int userId)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            //cookie = (cookie == null) ? AuthenticationHandler.GetCookie() : cookie;
            //if (cookie.Role == Config.Roles.Stakeholder)
            //{
                //int campaignId = Convert.ToInt32(cookie.Campaigns);

                List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, userId,
                    (int)userPermission);

                if (listPermissions.Count > 0)
                {
                    return listPermissions.First().Permission_Value;
                }
                return null;
            //}

        }

    }
}