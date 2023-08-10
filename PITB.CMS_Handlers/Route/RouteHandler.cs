using PITB.CMS_Common;
using PITB.CMS_Handlers.Authentication;
using PITB.CMS_Handlers.Business;
using PITB.CMS_Handlers.DB.Repository;
using PITB.CMS_Handlers.Permission;
using PITB.CMS_Models.Custom;
using PITB.CMS_Models.DB;
using System.Collections.Generic;
using System.Web.Routing;

namespace PITB.CMS_Handlers.Route
{
    public class RouteHandler
    {
        public static Config.Url GetUrl(byte roleId)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();


            Dictionary<string,string> dictRouteConfig = Utility.ConvertCollonFormatToDict(cookie.UrlConfig);

            

            RouteValueDictionary dict = new RouteValueDictionary();
            Config.Url url;

            if (dictRouteConfig.Count > 0)
            {
                url = new Config.Url { Action = dictRouteConfig["Action"], Controller = dictRouteConfig["Controller"], ParamsDict = dict };
                return url;
            }
            
            switch (roleId)
            {
                case 0://When an error occurs
                    url = new Config.Url { Action = "Logoff", Controller = "account", ParamsDict = dict };
                    break;
                
                case (byte)Config.Roles.Agent: //Agent
                    //if (cookie.Campaigns == Config.Campaign.Police.ToString())
                    //{
                    //    url = new Config.Url { Action = "Search", Controller = "Complaint", ParamsDict = dict }; //Operational
                    //}
                    url = new Config.Url { Action = "Search", Controller = "Complaint", ParamsDict = dict }; //Operational
                    break;
                
                case (byte)Config.Roles.AgentSuperVisor: //Agent Supervisor
                    url = new Config.Url { Action = "Search", Controller = "Complaint", ParamsDict = dict }; //Operational
                    break;
                case (byte)Config.Roles.PriviledgedUser: //Agent Supervisor
                    url = new Config.Url { Action = "UserReports", Controller = "Export", ParamsDict = dict }; //Operational
                    break;
                case (byte)Config.Roles.Stakeholder: //Stakeholder

                    /*if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowOnlyComplaintsAllInResolver))
                    {
                        url = new Config.Url { Action = "StakeholderComplaintsListingLowerHierarchyServerSide", Controller = "Complaint" };
                    }
                    if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.StakeholderLandOnDashboard))
                    {
                        url = new Config.Url { Action = "StakeholderComplaintsListingLowerHierarchyServerSide", Controller = "Complaint" };
                    }
                    else */
                    int campaignId = Utility.GetIntByCommaSepStr(cookie.Campaigns);
                    
                    if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.StakeholderLandOnDashboard))
                    {
                        
                        dict.Add("campaignId", cookie.Campaigns.Split(',')[0]);
                        url = new Config.Url { Action = "dashboard", Controller = "Report", ParamsDict = dict};
                    }
                    else if (Utility.GetIntByCommaSepStr(cookie.Campaigns) == (int)Config.Campaign.SchoolEducationEnhanced)
                    {
                        dict.Add("campaignId", (int)Config.Campaign.SchoolEducationEnhanced);

                        if (BlUsers.IsCookieHaveMultipleRoles())
                        {
                            url = new Config.Url { Action = "Roles", Controller = "Account", ParamsDict = dict }; //Operational
                        }
                        else
                        {
                            if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowSpecificListingPageToUser))
                            {
                                List<DbPermissionsAssignment> permission = RepoDbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.User, cookie.UserId, (int)Config.Permissions.ShowSpecificListingPageToUser);

                                if (permission != null && permission.Count > 0)
                                {
                                    var selectedPermission = permission[0];
                                    string permissionValue = selectedPermission.Permission_Value;
                                    Dictionary<string, string> permDict = Utility.ConvertCollonFormatToDict(permissionValue);
                                    var actionName = permDict["ActionName"];
                                    var controllerName = permDict["ControllerName"];
                                    url = new Config.Url { Action = actionName, Controller = controllerName, ParamsDict = dict };
                                }
                                else
                                {
                                    url = new Config.Url { Action = "DashboardMain", Controller = "Report", ParamsDict = dict };
                                }
                            }
                            else
                            {
                                url = new Config.Url { Action = "DashboardMain", Controller = "Report", ParamsDict = dict };
                            }
                        }
                        
                    }
                    else if (campaignId == (int) Config.Campaign.WasaNew)
                    {
                        dict.Add("campaignId", (int)Config.Campaign.WasaNew);
                        url = new Config.Url { Action = "StakeholderComplaintsListingServerSide", Controller = "WasaComplaint", ParamsDict = dict };
                    }
                    else if (campaignId == (int)Config.Campaign.FixItLwmc && cookie.Hierarchy_Id==Config.Hierarchy.District)
                    {
                        //dict.Add("campaignId", (int)Config.Campaign.WasaNew);
                        url = new Config.Url { Action = "StakeholderComplaintsListingLowerHierarchyServerSide", Controller = "Complaint", ParamsDict = dict };
                    }
                    else
                    {
                        url = new Config.Url { Action = "StakeholderComplaintsListingServerSide", Controller = "Complaint", ParamsDict = dict }; //Operational
                    }
                    
                    
                    break;
                
                case (byte)Config.Roles.AdminStakeholder: //Admin Stakeholder
                    url = new Config.Url { Action = "AddEditUser", Controller = "AdminStakeholder", ParamsDict = dict }; //Operational
                    break;

                
                case (byte)Config.Roles.AdminCampaign: //Admin Stakeholder
                    url = new Config.Url { Action = "ViewCampaign", Controller = "AdminCampaign", ParamsDict = dict }; //Operational
                    break;

                case (byte)Config.Roles.Executive:
                    url = new Config.Url { Action = "Dashboard", Controller = "Executive", ParamsDict = dict };
                    break;
                default:
                    url = new Config.Url { Action = "Logoff", Controller = "account", ParamsDict = dict };
                    break;

            }
            return url;
        }
    }
}