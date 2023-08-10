using System.Reflection.Emit;
using System.Web.Mvc;
using Amazon.AutoScaling.Model;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.View.Agent;
using Controller = PITB.CMS.Controllers.Controller;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Handler.Configuration;


namespace PITB.CMS.Handler.Business
{
    public class BlView
    {
        private static BlView _instance;

        public static BlView Instance
        {
            get
            {
                if (_instance == null)
                {
                    return new BlView();
                }
                return _instance;
            }
        }

        public string GetConfiguredView(string defaultViewStr, string specificViewConfig, string pageName)
        {
            string viewPage = null;
            Utility.ConvertCollonFormatToDict(ConfigurationHandler.GetConfiguration(specificViewConfig)).TryGetValue(pageName,out viewPage);
            if (viewPage != null)
            {
                return viewPage;
            }
            else
            {
                Utility.ConvertCollonFormatToDict(ConfigurationHandler.GetConfiguration(defaultViewStr)).TryGetValue(pageName, out viewPage);
                return viewPage;
            }
        }

        //public string GetConfiguredView(int roleId,int campaignId, string pageName)
        //{
        //    Dictionary<string,string> paramDict = new Dictionary<string, string>()
        //    {
                
        //    };
        //    string configuration = ConfigurationHandler.GetConfiguration("Pages", roleId, campaignId.ToString(),
        //        Config.ConfigType.Config);
        //    Dictionary<string, string> dictPages = null;
        //    if (configuration != null)
        //    {
        //        dictPages = Utility.ConvertCollonFormatToDict(configuration);
        //        string pageView = null;
        //        if (dictPages.TryGetValue(pageName, out pageView))
        //        {
        //            return pageView;
        //        }
        //    }
            
        //    return pageView;
        //}

        public ActionResult GetComplaintDetail(int complaintId, string viewType, Controller ctrlRef)
        {
            DbComplaint dbComplaint = DbComplaint.GeByComplaintIdAllColumnsIncluded(complaintId);
            if (viewType == "StakeholderComplaintDetail")
            {
                VmComplaintDetail vmComplaintDetail = BlComplaints.GetComplaintDetail(dbComplaint);
                return ctrlRef.ReturnView("_ComplaintDetail",null, vmComplaintDetail);
            }
            else if (viewType == "AgentComplaintDetail")
            {
                if (dbComplaint.Compaign_Id == (int) Config.Campaign.Police)
                {
                    VmComplaintPoliceDetailAgent vmComplaintPoliceDetail =
                        new VmComplaintPoliceDetailAgent(dbComplaint,BlPolice.GetComplaintDetail(dbComplaint));

                    return ctrlRef.ReturnView("~/Views/Agent/Police/_AgentPoliceDetail.cshtml",null, vmComplaintPoliceDetail);
                }
                if (dbComplaint.Compaign_Id == (int)Config.Campaign.DcoOffice)
                {
                    VmComplaintZimmedarShehriDetailAgent vmComplaintDetail = BlZimmedarShehri.GetStakeholderComplaintDetail(complaintId, VmStakeholderComplaintDetail.DetailType.All);

                    return ctrlRef.ReturnView("~/Views/Agent/ZimmedarShehri/_AgentZimmedarShehriDetail.cshtml", null, vmComplaintDetail);
                }
                else
                {
                    VmComplaintDetailAgent vmComplaintDetail =
                        new VmComplaintDetailAgent(BlComplaints.GetComplaintDetail(dbComplaint));
                    return ctrlRef.ReturnView("~/Views/Agent/_AgentDetail.cshtml", null, vmComplaintDetail);
                }
            }
            
            return null;
        }

        public string SetMasterPageInCookie(CMSCookie cookie = null, string masterpage=null)
        {
            bool isCookieNull = false;
            if (cookie == null)
            {
                isCookieNull = true;
                cookie = AuthenticationHandler.GetCookie();
            }
            else
            {
                isCookieNull = false;
            }
            cookie.MasterPage = masterpage==null ? GetMasterPage(cookie) : masterpage;
            if (isCookieNull)
            {
                new AuthenticationHandler().SaveCookie(cookie);
            }
            //new AuthenticationHandler().SaveCookie(cookie);
            return cookie.MasterPage;
        }

        public string GetMasterPageFromCookie()
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            return cookie.MasterPage;
        }

        private string GetMasterPage(CMSCookie cookie)
        {
            //cookie = AuthenticationHandler.GetCookie();
            string masterPage = null;

            //------------------


            if (cookie.Role == Config.Roles.Stakeholder)
            {
                if (cookie.Campaigns == Convert.ToInt32(Config.Campaign.SchoolEducationEnhanced).ToString())
                {
                    if (cookie.SubRoles == Config.SubRoles.SDU)
                    {
                        masterPage = "~/Views/Shared/SchoolEducation/_SduSchoolEducationStakeholderLayout.cshtml";
                    }
                    else if (cookie.UserId == 67818 || cookie.UserId == 71266)
                    {
                        masterPage = "~/Views/Shared/SchoolEducation/_SchoolEducationPMIULayout.cshtml";
                    }
                    else
                    {
                        masterPage = "~/Views/Shared/SchoolEducation/_SchoolEducationStakeholderLayout.cshtml";
                    }
                }
                else
                {
                    masterPage = "~/Views/Shared/_StakeholderLayout.cshtml";
                }
            }
            if (cookie.Role == Config.Roles.Executive)
            {
                masterPage = "~/Views/Shared/Executive/_ExecutiveDashboardLayout.cshtml";
            }
            //------------------



            if (cookie.Role == Config.Roles.Agent || cookie.Role == Config.Roles.AgentSuperVisor)
            {
                if (cookie.Campaigns == ((int)Config.Campaign.Police).ToString())
                {
                    masterPage = Config.MasterPageAgentPolice;
                }
                else if (cookie.Campaigns == ((int)Config.Campaign.SpecialEducation).ToString())
                {
                    masterPage = Config.MasterPageAgentSpecialEducation;
                }
                else
                {
                    masterPage = Config.DefaultMasterPageAgent;
                    //"~/Views/Shared/_AgentPoliceLayout.cshtml";
                }
            }

            return masterPage;
        }

        //public void SetMasterPage(Controller ctrlRef)
        //{
        //    CMSCookie cookie = AuthenticationHandler.GetCookie();


        //    //------------------


        //    if (cookie.Role == Config.Roles.Stakeholder)
        //    {
        //        if (cookie.Campaigns == Convert.ToInt32(Config.Campaign.SchoolEducationEnhanced).ToString())
        //        {
        //            if (cookie.SubRoles == Config.SubRoles.SDU)
        //            {
        //                ctrlRef.ViewBag.Layout = "~/Views/Shared/SchoolEducation/_SduSchoolEducationStakeholderLayout.cshtml";
        //            }
        //            else if (cookie.UserId == 67818)
        //            {
        //                ctrlRef.ViewBag.Layout = "~/Views/Shared/SchoolEducation/_SchoolEducationPMIULayout.cshtml";
        //            }
        //            else
        //            {
        //                ctrlRef.ViewBag.Layout = "~/Views/Shared/SchoolEducation/_SchoolEducationStakeholderLayout.cshtml";
        //            }
        //        }
        //        else
        //        {
        //            ctrlRef.ViewBag.Layout = "~/Views/Shared/_StakeholderLayout.cshtml";
        //        }
        //    }
        //    if (cookie.Role == Config.Roles.Executive)
        //    {
        //        ctrlRef.ViewBag.Layout = "~/Views/Shared/Executive/_ExecutiveDashboardLayout.cshtml";
        //    }
        //    //------------------



        //    if (cookie.Role == Config.Roles.Agent || cookie.Role == Config.Roles.AgentSuperVisor)
        //    {
        //        if (cookie.Campaigns == ((int) Config.Campaign.Police).ToString())
        //        {
        //            ctrlRef.ViewBag.Layout = Config.MasterPageAgentPolice;
        //        }
        //        else if (cookie.Campaigns == ((int) Config.Campaign.SpecialEducation).ToString())
        //        {
        //            ctrlRef.ViewBag.Layout = Config.MasterPageAgentSpecialEducation;
        //        }
        //        else
        //        {
        //            ctrlRef.ViewBag.Layout = Config.DefaultMasterPageAgent;
        //                //"~/Views/Shared/_AgentPoliceLayout.cshtml";
        //        }
        //    }
        //}

        public ActionResult GetAgentListView(Controller ctrlRef,  string viewType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            //return ctrlRef.ReturnView("~/Views/Agent/SearchList.cshtml");
            if (viewType == "AgentListingComplaintsAssignedToMe")// Complaints Assigned to me
            {
                if (cookie.Campaigns ==  ((int)Config.Campaign.Police).ToString())
                {
                    //ctrlRef.ViewBag.Layout = Config.MasterPageAgentPolice;
                    return ctrlRef.ReturnView("~/Views/Agent/Police/AgentPoliceComplaintListingMy.cshtml");
                }
                else
                {

                    return ctrlRef.ReturnView("~/Views/Agent/Index.cshtml");
                }
            }
            else if (viewType == "AgentListingComplaintsAll") // Complaints All
            {
                if (cookie.Campaigns == ((int)Config.Campaign.Police).ToString())
                {
                    //ctrlRef.ViewBag.Layout = Config.MasterPageAgentPolice; 
                    return ctrlRef.ReturnView("~/Views/Agent/Police/AgentPoliceComplaintListingAll.cshtml");
                }
                else
                {
                    return ctrlRef.ReturnView("~/Views/Agent/ComplaintListingAll.cshtml");
                }
            }
            return null;
        }

        //public ActionResult GetAgentSearchView(Controller ctrlRef)
        //{
        //    CMSCookie cookie = AuthenticationHandler.GetCookie();
        //    //return ctrlRef.ReturnView("~/Views/Agent/SearchList.cshtml");
            
        //    if (cookie.Campaigns==Config.Campaign.Police.ToString())
        //    {
        //        return ctrlRef.ReturnView("~/Views/Agent/SearchList.cshtml");
        //    }
        //    else
        //    {
        //        ctrlRef.ViewBag.Layout = Config.MasterPageAgentPolice;//"~/Views/Shared/_AgentPoliceLayout.cshtml";
        //        return ctrlRef.ReturnView("~/Views/Agent/Police/AgentPoliceSearchList.cshtml");
        //    }
        //    return null;
        //}

        public ActionResult GetListingView(int ComplaintType, string viewType, Controller ctrlRef)
        {
            string configListingType = null;
            if (viewType == "ComplaintsMy")
            {
                CMSCookie cookie = AuthenticationHandler.GetCookie();
                ctrlRef.ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
                if (!cookie.Campaigns.Contains(","))
                {
                    ctrlRef.ViewBag.LogoUrl =
                        DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                    ctrlRef.ViewBag.CampaignName = DbCampaign.GetCampaignShortNameById(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                }
                else
                {
                    ctrlRef.ViewBag.LogoUrl = null;
                    ctrlRef.ViewBag.CampaignName = null;
                }
                //ctrlRef.ViewBag.LogoUrl =
                //    DbCampaign.GetLogoUrlByCampaignId(
                //        Utility.GetIntByCommaSepStr(AuthenticationHandler.GetCookie().Campaigns));
                List<SelectListItem> listCampaings = ctrlRef.ViewBag.Campaigns;

                ctrlRef.ViewBag.ComplaintTypeList =
                     BlComplaintType.GetUserCategoriesAgainstCampaign(listCampaings);
               

                ctrlRef.ViewBag.ListPermissions = AuthenticationHandler.GetCookie().ListPermissions;
                ctrlRef.ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
                //}
                List<int> campaignIds = new List<int>
                {
                    (int) Config.Campaign.SchoolEducationEnhanced,
                    (int) Config.Campaign.ZimmedarShehri,
                    (int) Config.Campaign.DcoOffice,
                    (int) Config.Campaign.Police,
                    (int) Config.Campaign.FixItLwmc

                };

                //CMSCookie cookie = AuthenticationHandler.GetCookie();
                List<SelectListItem> listStatuses = null;
                
                if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIds))
                    // if is school education new campaign
                {
                    if (Utility.IsElementPresentInString(cookie.Campaigns, (int) Config.Campaign.SchoolEducationEnhanced))
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                                Config.Permissions.StatusesForComplaintListing);
                        listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus,
                            Config.SchoolEducationUnsatisfactoryStatus);
                        ctrlRef.ViewBag.StatusList = listStatuses;

                        if (cookie.SubRoles == Config.SubRoles.SDU)
                        {
                            return
                                ctrlRef.ReturnView(
                                    "~/Views/Stakeholder/SchoolEducation/SDU/SDUSchoolEducationComplaintListingsServerSide.cshtml");
                        }
                        else
                        {
                            return
                                ctrlRef.ReturnView(
                                    "~/Views/Stakeholder/SchoolEducation/SchoolEducationComplaintListingsServerSide.cshtml");
                        }
                    }
                    else if (Utility.IsElementPresentInString(cookie.Campaigns, (int) Config.Campaign.ZimmedarShehri)||
                        Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.DcoOffice))
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                                Config.Permissions.StatusesForComplaintListing);
                        ctrlRef.ViewBag.StatusList = listStatuses;

                        return
                            ctrlRef.ReturnView(
                                "~/Views/Stakeholder/ZimmedarShehri/ZimmerdarShehriComplaintListingsServerSide.cshtml");
                    }
                    else if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.Police))
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                                Config.Permissions.StatusesForComplaintListing);
                        ctrlRef.ViewBag.StatusList = listStatuses;

                        return
                            ctrlRef.ReturnView(
                                "~/Views/Stakeholder/Police/PoliceComplaintListingsServerSide.cshtml");
                    }
                    else if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.FixItLwmc))
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                                Config.Permissions.StatusesForComplaintListing);
                        ctrlRef.ViewBag.StatusList = listStatuses;

                        return
                            ctrlRef.ReturnView(
                                "~/Views/Stakeholder/LWMC/LwmcComplaintListingsServerSide.cshtml");
                    }
                    
                }
                if (Utility.IsAllMultipleElementsPresentInString(cookie.Campaigns, "69,68,72,73,74"))
                    {
                        ctrlRef.ViewBag.LogoUrl = null;
                        ctrlRef.ViewBag.CampaignName = "Health";
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                                Config.Permissions.StatusesForComplaintListing);
                        ctrlRef.ViewBag.DepartmentList = BlDepartment.GetDepartmentsOfCampaignOfUser(listCampaings);
                        List<SelectListItem> listDepartments = ctrlRef.ViewBag.DepartmentList;
                        ctrlRef.ViewBag.ComplaintTypeList =
                             BlComplaintType.GetUserCategoriesAgainstCampaignWithDepartment(listCampaings, listDepartments);
                    ctrlRef.ViewBag.StatusList = listStatuses;

                        return
                            ctrlRef.ReturnView(
                                "~/Views/Stakeholder/Health/HealthComplaintListingsServerSide.cshtml");
                    }
                    listStatuses =
                        BlCommon.GetStatusListByCampaignIds(
                            listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                            Config.Permissions.StatusesForComplaintListing);
                    ctrlRef.ViewBag.StatusList = listStatuses;
                    configListingType = "ComplaintListingListingMine";
                    string viewPage = BlView.Instance.GetConfiguredView(string.Format("Type::Config___Module::Pages___RoleId::{0}", (int)cookie.Role),
                   string.Format("Type::Config___Module::Pages___RoleId::{0}___Campaign::{1}", (int)cookie.Role, Utility.GetCommaSepStrFromList(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList())), configListingType);
                    return ctrlRef.ReturnView(viewPage); 
                   //return ctrlRef.ReturnView("~/Views/Stakeholder/ComplaintListingsServerSide.cshtml");
            }




            else if (viewType == "ComplaintsLowerHierarchy")
            {
                CMSCookie cookie = AuthenticationHandler.GetCookie();
                List<SelectListItem> listStatuses = null;
                ctrlRef.ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
                if (!cookie.Campaigns.Contains(","))
                {
                    ctrlRef.ViewBag.LogoUrl =
                        DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                    ctrlRef.ViewBag.CampaignName = DbCampaign.GetCampaignShortNameById(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                }
                else
                {
                    ctrlRef.ViewBag.LogoUrl = null;
                    ctrlRef.ViewBag.CampaignName = null;
                }
                List<SelectListItem> listCampaings = ctrlRef.ViewBag.Campaigns;
                ctrlRef.ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ctrlRef.ViewBag.Campaigns);
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListingAll);
                
                ctrlRef.ViewBag.StatusList = listStatuses;
                ctrlRef.ViewBag.ListPermissions = AuthenticationHandler.GetCookie().ListPermissions;

                ctrlRef.ViewBag.ComplaintTypeList =
                                    BlComplaintType.GetUserCategoriesAgainstCampaign(listCampaings);
               

                List<int> campaignIdsSmartLahore = new List<int> { 35, 36, 39 };
                //List<int> campaignIds = new List<int> { (int)Config.Campaign.SchoolEducationEnhanced, (int)Config.Campaign.ZimmedarShehri };

                if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.SchoolEducationEnhanced))
                // if is school education new campaign
                {
                    listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                    return ctrlRef.ReturnView("~/Views/Stakeholder/SchoolEducation/SchoolEducationComplaintListingsLowerHierarchyServerSide.cshtml");
                }
                if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.ZimmedarShehri) ||
                        Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.DcoOffice))
                {
                    listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.UnsatisfactoryClosedStatus);
                    return ctrlRef.ReturnView("~/Views/Stakeholder/ZimmedarShehri/ZimmedarShehriComplaintListingsLowerHierarchyServerSide.cshtml");
                }
                if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.Police))
                {
                    //listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.UnsatisfactoryClosedStatus);
                    return ctrlRef.ReturnView("~/Views/Stakeholder/Police/PoliceComplaintListingsLowerHierarchyServerSide.cshtml");
                }
                if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.FixItLwmc))
                {
                    //listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.UnsatisfactoryClosedStatus);
                    return ctrlRef.ReturnView("~/Views/Stakeholder/LWMC/LwmcComplaintListingsLowerHierarchyServerSide.cshtml");
                }
                else if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIdsSmartLahore)) // if campaign is smart lahore
                {
                    listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                    return ctrlRef.ReturnView("~/Views/Stakeholder/ComplaintListingsSmartLahoreLowerHierarchy.cshtml");
                }else if (Utility.IsAllMultipleElementsPresentInString(cookie.Campaigns, "69,68,72,73,74"))
                {
                    ctrlRef.ViewBag.LogoUrl = null;
                    ctrlRef.ViewBag.CampaignName = "Health";
                    listStatuses =
                        BlCommon.GetStatusListByCampaignIds(
                            listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                            Config.Permissions.StatusesForComplaintListing);
                    ctrlRef.ViewBag.StatusList = listStatuses;
                    ctrlRef.ViewBag.DepartmentList = BlDepartment.GetDepartmentsOfCampaignOfUser(listCampaings);
                    List<SelectListItem> listDepartments = ctrlRef.ViewBag.DepartmentList;
                    ctrlRef.ViewBag.ComplaintTypeList =
                         BlComplaintType.GetUserCategoriesAgainstCampaignWithDepartment(listCampaings, listDepartments);
                    return
                        ctrlRef.ReturnView(
                            "~/Views/Stakeholder/Health/HealthComplaintListingsLowerHierarchyServerSide.cshtml");
                }

                else
                {
                    listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.UnsatisfactoryClosedStatus);
                    configListingType = "ComplaintListingListingAll";
                    string viewPage = BlView.Instance.GetConfiguredView(string.Format("Type::Config___Module::Pages___RoleId::{0}", (int)cookie.Role),
                   string.Format("Type::Config___Module::Pages___RoleId::{0}___Campaign::{1}", (int)cookie.Role, Utility.GetCommaSepStrFromList(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList())), configListingType);
                    return ctrlRef.ReturnView(viewPage); 
                    //return ctrlRef.ReturnView("~/Views/Stakeholder/ComplaintListingsLowerHierarchyServerSide.cshtml");
                }
                
            }
            else if (viewType == "ComplaintsSuggestion")
            {
                CMSCookie cookie = AuthenticationHandler.GetCookie();
                ctrlRef.ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
                //AuthenticationHandler.GetCookie().Campaigns;
                if (!cookie.Campaigns.Contains(","))
                {
                    ctrlRef.ViewBag.LogoUrl =
                        DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                }
                else
                {
                    ctrlRef.ViewBag.LogoUrl = null;
                }
                List<SelectListItem> listCampaings = ctrlRef.ViewBag.Campaigns;
                ctrlRef.ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ctrlRef.ViewBag.Campaigns, (Config.ComplaintType)ComplaintType);
                //ViewBag.StatusList = BlCommon.GetStatusListByCampaignId(Convert.ToInt32(listCampaings[0].Value));

                //List<int> campaignIds = new List<int> { (int)Config.Campaign.SchoolEducationEnhanced };

                
                if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.SchoolEducationEnhanced))
                // if is school education new campaign
                {
                    if (cookie.SubRoles == Config.SubRoles.SDU)
                    {
                        return ctrlRef.ReturnView("~/Views/Stakeholder/SchoolEducation/SDU/SDUSchoolEducationSuggestionListingsServerSide.cshtml");
                    }
                    else
                    {
                        return ctrlRef.ReturnView("~/Views/Stakeholder/SchoolEducation/SchoolEducationSuggestionListingsServerSide.cshtml");
                    }
                }
                else if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.ZimmedarShehri) ||
                        Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.DcoOffice))
                // if is school education new campaign
                {
                    return ctrlRef.ReturnView("~/Views/Stakeholder/ZimmedarShehri/ZimmedarShehriSuggestionListingsServerSide.cshtml");
                }
                else if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.Police))
                // if is school education new campaign
                {
                    return ctrlRef.ReturnView("~/Views/Stakeholder/Police/PoliceSuggestionListingsServerSide.cshtml");
                }
                else
                {
                    configListingType = "ComplaintListingListingSuggestion";
                    string viewPage = BlView.Instance.GetConfiguredView(string.Format("Type::Config___Module::Pages___RoleId::{0}", (int)cookie.Role),
                   string.Format("Type::Config___Module::Pages___RoleId::{0}___Campaign::{1}", (int)cookie.Role, Utility.GetCommaSepStrFromList(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList())), configListingType);
                    return ctrlRef.ReturnView(viewPage); 
                    //return ctrlRef.ReturnView("~/Views/Stakeholder/SuggestionListingsServerSide.cshtml");
                }
            }
            else if (viewType == "ComplaintsInquiry")
            {
                CMSCookie cookie = AuthenticationHandler.GetCookie();
                ctrlRef.ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
                //AuthenticationHandler.GetCookie().Campaigns;
                if (!cookie.Campaigns.Contains(","))
                {
                    ctrlRef.ViewBag.LogoUrl =
                        DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                }
                else
                {
                    ctrlRef.ViewBag.LogoUrl = null;
                }
                ctrlRef.ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
                //ctrlRef.ViewBag.LogoUrl = DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
                List<SelectListItem> listCampaings = ctrlRef.ViewBag.Campaigns;
                ctrlRef.ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ctrlRef.ViewBag.Campaigns, (Config.ComplaintType)ComplaintType);
                //ViewBag.StatusList = BlCommon.GetStatusListByCampaignId(Convert.ToInt32(listCampaings[0].Value));

                List<int> campaignIds = new List<int> { (int)Config.Campaign.SchoolEducationEnhanced };

                //CMSCookie cookie = AuthenticationHandler.GetCookie();
                if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIds))
                // if is school education new campaign
                {
                    if (cookie.SubRoles == Config.SubRoles.SDU)
                    {
                        return ctrlRef.ReturnView("~/Views/Stakeholder/SchoolEducation/SDU/SDUSchoolEducationInquiryListingsServerSide.cshtml");
                    }
                    else
                    {
                        return ctrlRef.ReturnView("~/Views/Stakeholder/SchoolEducation/SchoolEducationInquiryListingsServerSide.cshtml");
                    }
                }
                else if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.ZimmedarShehri) ||
                        Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.DcoOffice))
                // if is school education new campaign
                {
                    return ctrlRef.ReturnView("~/Views/Stakeholder/ZimmedarShehri/ZimmedarShehriInquiryListingsServerSide.cshtml");
                }
                else if (Utility.IsElementPresentInString(cookie.Campaigns, (int)Config.Campaign.Police))
                // if is school education new campaign
                {
                    return ctrlRef.ReturnView("~/Views/Stakeholder/Police/PoliceInquiryListingsServerSide.cshtml");
                }
                else
                {
                    configListingType = "ComplaintListingListingInquiry";
                    string viewPage = BlView.Instance.GetConfiguredView(string.Format("Type::Config___Module::Pages___RoleId::{0}", (int)cookie.Role),
                   string.Format("Type::Config___Module::Pages___RoleId::{0}___Campaign::{1}", (int)cookie.Role, Utility.GetCommaSepStrFromList(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList())), configListingType);
                    return ctrlRef.ReturnView(viewPage); 
                    //return ctrlRef.ReturnView("~/Views/Stakeholder/InquiryListingsServerSide.cshtml");
                }
            }
            return null;
        }
    }
}