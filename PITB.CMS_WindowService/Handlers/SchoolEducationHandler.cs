using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.StakeHolder;
using PITB.CMS_Common.Handler.Sync.SchoolEducation;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace PITB.CMS_WindowService.Handlers
{
    public class SchoolEducationHandler
    {

        public static void SynRegionAndShoolsDataMain()
        {
            try
            {
                string mode = ConfigurationManager.AppSettings["SyncMode"].ToString();
                if (mode.Equals("Interval"))
                {
                    //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 0", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 0", Config.ServiceType.SyncSchoolDataMain);
                    SyncSchoolDataHandler.SynRegionAndShoolsDataMain(
                        DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["SyncDaysInterval"])),
                        DateTime.Now);
                }
                else if (mode.Equals("DateRange"))
                {
                    SyncSchoolDataHandler.SynRegionAndShoolsDataMain(DateTime.ParseExact(ConfigurationManager.AppSettings["SyncMinDate"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                        DateTime.ParseExact(ConfigurationManager.AppSettings["SyncMaxDate"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            catch (Exception ex)
            {
                DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 1", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 1", Config.ServiceType.SyncSchoolDataMain);
            }
        }

        public static void SycUserWiseComplaints()
        {
            try
            {
            DBContextHelperLinq db = new DBContextHelperLinq();
            
            int campaignId = (int) Config.Campaign.SchoolEducationEnhanced;
            List<DbComplaint> listDbComplaints =
                DbComplaint.GetByCampaignId(db, (int) Config.Campaign.SchoolEducationEnhanced,
                    Config.ComplaintType.Complaint);

            List<DbUsers> listDbUsers = DbUsers.GetActiveUserAgainstParams(campaignId);
            List<DbComplaint> listUserComplaints = null;
            List<DbUserWiseComplaints> listDbUserWiseComplaintsToInsert = new List<DbUserWiseComplaints>();
            
            List<DbUserWiseComplaints> listUserWiseComplaintsCurrent = DbUserWiseComplaints.GetUserWiseComplaints(db,
                campaignId, new List<int?> {(int) Config.ComplaintType.Complaint},
                new List<int?>
                {
                    (int) Config.StakeholderComplaintListingType.AssignedToMe,
                    (int) Config.StakeholderComplaintListingType.UptilMyHierarchy
                });
            DbUsers dbUser = null;

            // ----- altering code -------
            //List<int> listComplaintIds = new List<int> { 206000 };
            //listDbComplaints = listDbComplaints.Where(n => listComplaintIds.Contains(n.Id)).ToList();

            //List<int> listUserIds = new List<int> { 1189, 1190, 1191, 1455, 10771, 11078, 11239, 11242, 11910, 20602, 1455, 10771 };
            //List<int> listUserIds = new List<int> { 1455, 10771 };
            //listDbUsers = listDbUsers.Where(n => listUserIds.Contains(n.User_Id)).ToList();

            //----------------------------

                //listDbUsers = listDbUsers.Take(10).ToList();

            for (int i = 0; i < listDbUsers.Count; i++)
            {
                try
                {


                    dbUser = listDbUsers[i];
                    listUserComplaints = BlSchool.GetComplaintsAgainstUser(dbUser,
                        Config.StakeholderComplaintListingType.AssignedToMe, Config.ComplaintType.Complaint,
                        Config.SelectionType.Specific);
                    listDbUserWiseComplaintsToInsert.AddRange(
                        UserWiseComplaintsHandler.GetUserWiseComplaints(dbUser, listUserComplaints,
                            (int) Config.Campaign.SchoolEducationEnhanced, Config.ComplaintType.Complaint,
                            Config.StakeholderComplaintListingType.AssignedToMe));

                    listUserComplaints = BlSchool.GetComplaintsAgainstUser(dbUser,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, Config.ComplaintType.Complaint,
                        Config.SelectionType.Specific);
                    listDbUserWiseComplaintsToInsert.AddRange(
                        UserWiseComplaintsHandler.GetUserWiseComplaints(dbUser, listUserComplaints,
                            (int) Config.Campaign.SchoolEducationEnhanced, Config.ComplaintType.Complaint,
                            Config.StakeholderComplaintListingType.UptilMyHierarchy));
                }
                catch (Exception ex)
                {
                    DbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex,
                        Config.ServiceType.SycUserWiseComplaints);
                }
            }
            // ----- altering code -------
            //listUserWiseComplaintsCurrent = listUserWiseComplaintsCurrent.Where(n => n.Complaint_Id == 206000).ToList();
            //listDbUserWiseComplaintsToInsert = listDbUserWiseComplaintsToInsert.Where(n => n.Complaint_Id == 206000).ToList();
            //----------------------------

            DbUserWiseComplaints.SyncUserWiseComplaints(db, listUserWiseComplaintsCurrent,
                listDbUserWiseComplaintsToInsert);
            

                MarkIsAssigneePresent(db, listDbComplaints, listDbUserWiseComplaintsToInsert);
                db.SaveChanges();
                //Handler.Complaint.UserWiseComplaintsHandler.GetUserWiseComplaints(dbUser,listSubordinateComplaints,(int)Config.Campaign.SchoolEducationEnhanced,)       
            }
            catch (Exception ex)
            {
                //throw;
            }
        }



        private static void MarkIsAssigneePresent( DBContextHelperLinq db, List<DbComplaint> listDbComplaints, List<DbUserWiseComplaints> listDbUserWiseComplaints)
        {
            DbComplaint dbComplaint = null;
            DbUserWiseComplaints dbUserWiseComplaints = null;

            for (int i = 0; i < listDbComplaints.Count; i++)
            {
                dbComplaint = listDbComplaints[i];
                dbUserWiseComplaints = listDbUserWiseComplaints.Where(
                    n =>
                        n.Complaint_Id == dbComplaint.Id && n.Complaint_Type == (int) Config.ComplaintType.Complaint &&
                        n.Complaint_Subtype == (int) Config.StakeholderComplaintListingType.AssignedToMe)
                    .FirstOrDefault();
                if (dbUserWiseComplaints == null)
                {
                    dbComplaint.Is_AssigneePresent = false;
                }
                else
                {
                    dbComplaint.Is_AssigneePresent = true;
                }
            }
            db.DbComplaints.Attach(dbComplaint);
            db.Entry(dbComplaint).Property(n => n.Is_AssigneePresent).IsModified = true;
        }


        public static void SendMessageToSchoolEducationUsers()
        {
            List<DbComplaint> listDbComplaint =
                DbComplaint.GetByCampaignId((int) Config.Campaign.SchoolEducationEnhanced);

            listDbComplaint = listDbComplaint.Where(n => n.Computed_Remaining_Time_To_Escalate != "None").ToList();
            List<Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping>> listComplaintMapping =
                new List<Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping>>();

            Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping> compaintMapping = null;
            foreach (DbComplaint dbComplaint in listDbComplaint)
            {
                TimeSpan totalTimeSpan = (TimeSpan) (dbComplaint.MaxSrcIdDate - dbComplaint.Created_Date);
                TimeSpan remainingTimeSpan =
                    Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
                var schoolMapping = DbSchoolsMapping.GetById((int) dbComplaint.TableRowRefId);
                compaintMapping = new Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping>(dbComplaint,
                    totalTimeSpan, remainingTimeSpan, schoolMapping);
                listComplaintMapping.Add(compaintMapping);
            }
            SendMessageAccordingToRules(listComplaintMapping);
        }

        public static void SendMessageAccordingToRules(
            List<Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping>> listComplaintMapping)
        {
            try
            {
                // Adding extra check
                if (listComplaintMapping != null && listComplaintMapping.Count > 0)
                {
                    TimeSpan item2Ts = new TimeSpan(4, listComplaintMapping[0].Item2.Hours,
                        listComplaintMapping[0].Item2.Minutes, listComplaintMapping[0].Item2.Seconds);

                    TimeSpan item3Ts = new TimeSpan(2, listComplaintMapping[0].Item3.Hours,
                        listComplaintMapping[0].Item3.Minutes, listComplaintMapping[0].Item3.Seconds);
                    listComplaintMapping[0] =
                        new Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping>(listComplaintMapping[0].Item1,
                            item2Ts, item3Ts, listComplaintMapping[0].Item4);
                }

                // End Adding extra check

                foreach (
                    Tuple<DbComplaint, TimeSpan, TimeSpan, DbSchoolsMapping> complaintMapping in listComplaintMapping)
                {

                    if (complaintMapping.Item2.Days >= 4 && complaintMapping.Item2.Days <= 10)
                    {
                        if (complaintMapping.Item3.Days == 2 || complaintMapping.Item3.Days == 0)
                        {
                            SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                        }
                    }
                    else if (complaintMapping.Item2.Days == 14)
                    {
                        if (complaintMapping.Item3.Days == 7 || complaintMapping.Item3.Days == 0)
                        {
                            SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                        }
                    }
                    else if (complaintMapping.Item2.Days == 20)
                    {
                        if (complaintMapping.Item3.Days == (20 - 15) || complaintMapping.Item3.Days == 0)
                        {
                            SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                        }
                    }
                    else if (complaintMapping.Item2.Days == 30)
                    {
                        if (complaintMapping.Item3.Days == (30 - 21) || complaintMapping.Item3.Days == 0)
                        {
                            SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                        }
                    }
                    else if (complaintMapping.Item2.Days == 120)
                    {
                        if (complaintMapping.Item3.Days <= 30)
                        {
                            if ((complaintMapping.Item3.Days%7) == 0)
                            {
                                SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                            }
                        }
                    }
                    else if (complaintMapping.Item2.Days == 180)
                    {
                        if (complaintMapping.Item3.Days <= 60)
                        {
                            if ((complaintMapping.Item3.Days%7) == 0)
                            {
                                SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                            }
                        }
                        else if (complaintMapping.Item3.Days == 90)
                        {
                            SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                        }
                    }
                    else if (complaintMapping.Item2.Days == 365)
                    {
                        if (complaintMapping.Item3.Days <= 90)
                        {
                            if ((complaintMapping.Item3.Days%7) == 0)
                            {
                                SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                            }
                        }
                        else if (complaintMapping.Item3.Days == 182)
                        {
                            SendMessageToStakeholder(complaintMapping.Item1, complaintMapping.Item4);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void SendMessageToStakeholder(
            /*int complaintId, int campaignId, int categoryId, int subCategoryId*/
            DbComplaint dbComplaint, DbSchoolsMapping dbSchools)
        {
            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            DbComplaintType dbComplaintType = DbComplaintType.GetById((int) dbComplaint.Complaint_Category);
            DbComplaintSubType dbComplaintSubType = DbComplaintSubType.GetById((int) dbComplaint.Complaint_SubCategory);
            TimeSpan span = Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
            DateTime dueDate = DateTime.Now.Add(span);
            string msg = "Reminder: Your action is required ASAP to resolve by " + dueDate.ToShortDateString() + "\n" +
                         dbComplaintType.Name + ":" + dbComplaintSubType.Name + " at " + dbSchools.school_emis_code +
                         " " + dbSchools.school_name + "" + "\n" +
                         "To update status of complaint, visit: crm.punjab.gov.pk";

            SmsModel smsModel = new SmsModel((int) Config.Campaign.SchoolEducationEnhanced,
                Config.SchoolEducationSecretaryPhoneNo, msg,
                (int) Config.MsgType.ToStakeholder,
                (int) Config.MsgSrcType.WindowService, DateTime.Now, 1, (int) Config.MsgTags.SchoolEducationStakeholder);

            List<DbUsers> listDbUsers =
                TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(dbComplaint, msg, true, smsModel);
            //List<DbUsers> listDbUsers = new List<DbUsers>();
            /*if (listDbUsers.Count == 0)
            {
                DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
                DbUsers currUser = null;
                int hierarchyIdValue = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
                listDbUsers = UsersHandler.FindUserUpperThanCurrentHierarchy(null, dbComplaint, campaignId, (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id,
                    hierarchyIdValue, true);

                TextMessageHandler.SendMessageToUsersList(listDbUsers, msg);
            }*/


            PrepareMessageToStakeholderSupervisor(dbComplaint, (int) dbComplaint.Compaign_Id, dbComplaintType,
                dbComplaintSubType,
                dbSchools);
        }

        private static void PrepareMessageToStakeholderSupervisor(DbComplaint dbComplaint, int campaignId,
            DbComplaintType dbComplaintType, DbComplaintSubType dbComplaintSubType, DbSchoolsMapping dbSchools)
        {
            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            DbUsers currUser = null;
            int hierarchyIdValue = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
            List<DbUsers> listDbUsers = UsersHandler.FindUserUpperThanCurrentHierarchy(null, dbComplaint, campaignId,
                (Config.Hierarchy) dbComplaint.Complaint_Computed_Hierarchy_Id,
                hierarchyIdValue, true);


            if (listDbUsers.Count > 0)
            {
                string designation = "", hierarchyValue;
                DbUsers dbUser = listDbUsers.FirstOrDefault();
                //int hierarchyVal = Convert.ToInt32(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser).Split(',')[0]);
                //string hierarchyString = Utility.GetHierarchyValue((Config.Hierarchy)dbUser.Hierarchy_Id, hierarchyVal);

                int hierarchyVal =
                    Convert.ToInt32(
                        DbComplaint.GetHierarchyIdValueAgainstHierarchyId(
                            (Config.Hierarchy) dbComplaint.Complaint_Computed_Hierarchy_Id, dbComplaint));
                string hierarchyString =
                    Utility.GetHierarchyValueName((Config.Hierarchy) dbComplaint.Complaint_Computed_Hierarchy_Id,
                        hierarchyVal);

                TimeSpan span = Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
                DateTime dueDate = DateTime.Now.Add(span);

                designation = "" + dbUser.Designation_abbr + "-" + hierarchyString + "";
                string msg = "Reminder: Please follow up with " + designation + " on resolution of: \n" +
                             dbComplaintType.Name + ":" + dbComplaintSubType.Name + " at " +
                             dbSchools.school_emis_code + " " + dbSchools.school_name + " by " +
                             dueDate.ToShortDateString() + "\n" +
                             "To view details, please visit: www.crm.punjab.gov.pk";

                SmsModel smsModel = new SmsModel((int) Config.Campaign.SchoolEducationEnhanced,
                    Config.SchoolEducationSecretaryPhoneNo, msg,
                    (int) Config.MsgType.ToStakeholder,
                    (int) Config.MsgSrcType.WindowService, DateTime.Now, 1,
                    (int) Config.MsgTags.SchoolEducationStakeholderSupervisor);

                TextMessageHandler.SendMessageToUsersList(listDbUsers, msg, true, smsModel);
            }
        }

        public static void SendMessageToSecretarySchoolEducation()
        {
            List<DbComplaint> listDbComplaints =
                DbComplaint.GetByCampaignId((int) Config.Campaign.SchoolEducationEnhanced,
                    Config.ComplaintType.Complaint);
            listDbComplaints =
                listDbComplaints.Where(
                    n => Config.ListSchoolEducationSecretaryDistricts.Contains((int) n.District_Id))
                    .ToList();
            //listDbComplaints =
            //    listDbComplaints.Where(
            //        n => n.Complaint_Computed_Status_Id == (int) Config.ComplaintStatus.UnsatisfactoryClosed
            //             || n.Complaint_Computed_Status_Id == (int) Config.ComplaintStatus.PendingReopened).ToList();

            List<DbComplaint> listDbComplaintToSendMessage = new List<DbComplaint>();
            List<DbComplaint> listDBComplaintDistrictWise = null;
            float percPendingOverdue = 0, percPendingReopen = 0;
            string msg = "", msgHeader = "", districtName = "";
            var districtWiseGroup = listDbComplaints.GroupBy(n => n.District_Id);
            msgHeader = msgHeader + "Education Helpline (Weekly Update) \nTop 3 complaint categories:\n";

            //foreach (var districtGroup in districtWiseGroup)
            foreach (var districtGroup in Config.ListSchoolEducationSecretaryDistricts)
            {
                districtName = DbDistrict.GetById((int) districtGroup).District_Name;

                listDBComplaintDistrictWise = listDbComplaints.Where(
                    n =>
                        //Config.ListSchoolEducationSecretaryDistricts.Contains((int) n.District_Id) &&
                        n.District_Id == districtGroup).ToList();

                var orderByComplaintCat =
                    listDBComplaintDistrictWise.GroupBy(n => new {n.Complaint_Category, n.Complaint_SubCategory})
                        .OrderByDescending(n => n.Key.Complaint_Category)
                        .ThenByDescending(n => n.Key.Complaint_SubCategory)
                        .ToList()
                        .Take(3);
                msgHeader = msgHeader + districtName + ": ";
                int i = 0;
                foreach (var complaintCat in orderByComplaintCat)
                {
                    msgHeader = msgHeader + DbComplaintType.GetById((int) complaintCat.Key.Complaint_Category).Name +
                                " | " + DbComplaintSubType.GetById((int) complaintCat.Key.Complaint_SubCategory).Name;

                    i++;

                    if (i < orderByComplaintCat.Count())
                    {
                        msgHeader = msgHeader + ", ";
                    }
                    else
                    {
                        msgHeader = msgHeader + "\n";
                    }

                }
                if (orderByComplaintCat.Count() == 0)
                {
                    msgHeader = msgHeader + "\n";
                }

                List<DbComplaint> listPendingOverDue =
                    listDBComplaintDistrictWise.Where(
                        n => n.Complaint_Computed_Status_Id == (int) Config.ComplaintStatus.UnsatisfactoryClosed)
                        .ToList();

                List<DbComplaint> listPendingReopen =
                    listDBComplaintDistrictWise.Where(
                        n => n.Complaint_Computed_Status_Id == (int) Config.ComplaintStatus.PendingReopened)
                        .ToList();
                if (listDBComplaintDistrictWise.Count != 0)
                {
                    percPendingOverdue = ((float) listPendingOverDue.Count*100)/listDBComplaintDistrictWise.Count;
                    percPendingReopen = ((float) listPendingReopen.Count*100)/listDBComplaintDistrictWise.Count;
                }
                else
                {
                    percPendingOverdue = 0;
                    percPendingReopen = 0;
                }


                msg = msg + @"CEO " + districtName + ": " +
                      Math.Round(percPendingOverdue, 1, MidpointRounding.AwayFromZero) + "% " + "pending (overdue) | " +
                      Math.Round(percPendingReopen, 1, MidpointRounding.AwayFromZero) + "% " + "pending (re-opened)\n";

                //listDbComplaints.AddRange(listDBComplaintDistrictWise);
            }
            msg = msgHeader + "\n\n" + msg;
            //TextMessageHandler.SendMessageToPhoneNo("03458554214", msg);

            SmsModel smsModel = new SmsModel((int) Config.Campaign.SchoolEducationEnhanced,
                Config.SchoolEducationSecretaryPhoneNo, msg,
                (int) Config.MsgType.ToStakeholder,
                (int) Config.MsgSrcType.WindowService, DateTime.Now, 1, (int) Config.MsgTags.SchoolEducationSecretary);
            TextMessageHandler.SendMessageToPhoneNo(Config.SchoolEducationSecretaryPhoneNo, msg, true, smsModel);

        }

        public static void SendMessageToAllUsersForDepartment()
        {
            int campaignId = (int) Config.Campaign.SchoolEducationEnhanced;
            List<DbUsers> listDbUsers = DbUsers.GetUsersAgainstCampaigns(new List<int> {campaignId});
            //List<DbComplaint> listComplaints = DbComplaint.GetByCampaignId(campaignId);
            List<DbDepartment> listDepartments = DbDepartment.GetByCampaignAndGroupId(campaignId, null);

            List<DbComplaintType> listComplaintTypes =
                DbComplaintType.GetByDepartmentIds(
                    Utility.GetNullableIntList(listDepartments.Select(n => n.Id).ToList()));
            string commaSeperatedCategories =
                Utility.GetCommaSepStrFromList(listComplaintTypes.Select(n => n.Complaint_Category).ToList());
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            List<DbStatus> listDbStatuses = null;
            string commaSeperatedStatuses = null;

            string msgStr = "", hierarchyTypeStr = "", hierarchyNameStr = "", statsStr = "";
            string spType = "DashboardLabelsStausWise";
            List<Tuple<DbDepartment, int>> listDepT = new List<Tuple<DbDepartment, int>>();
            Tuple<DbDepartment, int> depT = null;
            int departmentId = -1;
            int statusCount = 0;
            foreach (DbDepartment dbDepartment in listDepartments)
            {
                listDepT.Add(new Tuple<DbDepartment, int>(dbDepartment, 0));
            }
            foreach (DbUsers dbUser in listDbUsers)
            {
                try
                {


                    msgStr = "";
                    commaSeperatedCategories = dbUser.Categories.Replace(" ", "");

                    listDbPermissionsAssignment = DbPermissionsAssignment
                        .GetListOfPermissionsByTypeTypeIdAndPermissionId(
                            (int) Config.PermissionsType.User, dbUser.User_Id,
                            (int) Config.Permissions.StatusesForComplaintListing);

                    listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);
                    //BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                    //    listDbPermissionsAssignment);

                    commaSeperatedStatuses = string.Join(",",
                        listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());
                    /*
                listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, dbUser.User_Id, (int)Config.Permissions.StatusesForComplaintListingAll);

                listDbStatuses = BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.User_Id,
                    listDbPermissionsAssignment, Config.Permissions.StatusesForComplaintListingAll);

                commaSeperatedStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());
                */
                    //List<string> listHierarchyVal = new List<string>{dbUser.Province_Id, dbUser.Division_Id, dbUser.District_Id, dbUser.Tehsil_Id, dbUser.UnionCouncil_Id};
                    List<Tuple<int, string>> listHierarchyTuple = Utility.GetHierarchyMappingListByUser(dbUser,
                        Convert.ToInt32(dbUser.Hierarchy_Id), false);
                    hierarchyTypeStr = "";
                    hierarchyNameStr = "";
                    foreach (Tuple<int, string> hierarchyTuple in listHierarchyTuple)
                    {
                        hierarchyTypeStr = hierarchyTypeStr + ((Config.Hierarchy) hierarchyTuple.Item1).ToString() +
                                           "/";
                        hierarchyNameStr = hierarchyNameStr +
                                           Utility.GetHierarchyValueName((Config.Hierarchy) hierarchyTuple.Item1,
                                               Utility.GetIntByCommaSepStr(hierarchyTuple.Item2)) + "/";
                    }
                    msgStr =
                        //"Following is the SMS Content Format to be sent on daily basis to the SED Officials: \n" +
                        "Daily Complaints Resolution SMS \n";
                    hierarchyTypeStr = hierarchyTypeStr.TrimEnd('/') + ": ";
                    hierarchyNameStr = "<" + hierarchyNameStr.TrimEnd('/') + ">";
                    msgStr = msgStr + hierarchyTypeStr + hierarchyNameStr;
                    msgStr = msgStr + "\n\nTotal/Resolved/Overdue \n\n";
                    /*
                List<VmStakeholderComplaintDashboard> listDashboard = BlSchool.GetStakeHolderServerSideListDenormalized(
                "1970-01-01",
                DateTime.Now.ToString("yyyy-MM-dd"),
                null,
                campaignId.ToString(),
                commaSeperatedCategories,
                commaSeperatedStatuses,
                "1,0",
                (int)Config.ComplaintType.Complaint,
                Config.StakeholderComplaintListingType.UptilMyHierarchy,
                spType,
                dbUser.User_Id).ToList<VmStakeholderComplaintDashboard>();
                */
                    VmStatusWiseComplaintsData statusWiseData = BlComplaints.GetCategoryWiseDashboardData(
                        dbUser.User_Id, "1970-01-01", DateTime.Now.ToString("yyyy-MM-dd"), campaignId,
                        Config.CategoryType.Main, -1);
                    //msgStr = msgStr + "Total/Resolved/Overdue \n";
                    int resolvedCount = 0, overdue = 0;
                    foreach (VmUserWiseStatus vmUserWiseStatus in statusWiseData.ListUserWiseData)
                    {
                        resolvedCount = 0;
                        overdue = 0;
                        statsStr = "";
                        statsStr = statsStr + vmUserWiseStatus.Name + ": ";
                        resolvedCount = vmUserWiseStatus.ListVmStatusWiseCount.Where(
                            n => n.StatusId == (int) Config.ComplaintStatus.ResolvedUnverified)
                            .FirstOrDefault()
                            .Count;

                        overdue = vmUserWiseStatus.ListVmStatusWiseCount.Where(
                            n => n.StatusId == (int) Config.ComplaintStatus.UnsatisfactoryClosed)
                            .FirstOrDefault()
                            .Count;

                        msgStr = msgStr + statsStr + (resolvedCount + overdue) + "/" + resolvedCount + "/" + overdue +
                                 " \n";
                    }

                    msgStr = msgStr + "\n\nPlease ensure timely resolution of all complaints.\n" +
                             "(School Education Department)";

                    string msgStrToSend = msgStr;

                    new Thread(delegate()
                    {
                        TextMessageHandler.SendMessageToPhoneNo("03214226005", msgStrToSend);
                        //TextMessageHandler.SendMessageToPhoneNo(dbUser.Phone, msgStr);
                    }).Start();


                    /*
                foreach (VmStakeholderComplaintDashboard vmDashboard in listDashboard)
                {
                    departmentId = Convert.ToInt32(listComplaintTypes.Where(n => n.Complaint_Category == vmDashboard.Id).FirstOrDefault().DepartmentId);
                    depT = listDepT.Where(n => n.Item1.Id == departmentId).FirstOrDefault();
                    if (depT != null)
                    {
                        statusCount = statusCount + vmDashboard.Count;
                        depT = new Tuple<DbDepartment, int>(depT.Item1, statusCount);
                        
                    }
                }
                */
                    //string messageStr = "Dear Sir/Madam, \n" +
                    //       "Govt. of Punjab is launching an Education Hotline (042-111-11-20-20) for parents. Complaints related to schools coming under your Markaz will be assigned to you. \n " +
                    //       "As a test case, a complaint has been assigned to you. Please download the application from “https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver” and resolve the complaint using your login ID/password (already provided). In case of technical issues, " +
                    //       "please email at hotline@sed.punjab.gov.pk. \n" +
                    //       "School Education Department, Government of the Punjab";


                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            //DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);


        }



        public static void MarkComplaintToOriginUserIfPresent()
        {
            try
            {
                int campaignId = (int) Config.Campaign.SchoolEducationEnhanced;
                DBContextHelperLinq db = new DBContextHelperLinq();
                db.Configuration.AutoDetectChangesEnabled = false;
                List<int> listStatus = new List<int>
                {
                    (int) Config.ComplaintStatus.PendingFresh,
                    (int) Config.ComplaintStatus.PendingReopened,
                    (int) Config.ComplaintStatus.UnsatisfactoryClosed
                };
                List<DbComplaint> listDbComplaint = DbComplaint.GetByOriginAndAssigneePresence(db, campaignId,
                    Config.ComplaintType.Complaint, listStatus, false, false);

                List<DbUsers> listDbUsers =
                    UsersHandler.GetUsersHierarchyMapping(Convert.ToInt32(campaignId));
                VmComplaint vmComplaint = null;
                List<AssignmentModel> assignmentModelList = null;
                DbSchoolsMapping dbSchools = null;
                Config.Hierarchy hierarchyId = Config.Hierarchy.UnionCouncil;
                int? userHierarchyVal = null;
                int? userCategoryId1 = null;
                int? userCategoryId2 = null;
                int? userCategoryId3 = null;

                //int i = 0;
                DbComplaint dbComplaint = null;
                for (int i=0; i<listDbComplaint.Count; i++)
                {
                    try
                    {
                        dbComplaint = listDbComplaint[i];
                        //vmComplaint = new VmComplaint();
                        //vmComplaint.Province_Id = dbComplaint.Province_Id;
                        //vmComplaint.Division_Id = dbComplaint.Division_Id;
                        //vmComplaint.District_Id = dbComplaint.District_Id;
                        //vmComplaint.Tehsil_Id = dbComplaint.Tehsil_Id;
                        //vmComplaint.UnionCouncil_Id = dbComplaint.UnionCouncil_Id;
                        //dbSchools = DbSchoolsMapping.GetById((int)dbComplaint.TableRowRefId);
                        //if (dbComplaint.Id == 198753)
                        //if (dbComplaint.Id == 213528 || dbComplaint.Id == 211975)
                        {


                            dbSchools = DbSchoolsMapping.GetById(Convert.ToInt32(dbComplaint.TableRowRefId));

                            assignmentModelList =
                                AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory(
                                    (int) dbComplaint.Compaign_Id,
                                    (int) dbComplaint.Complaint_Category, (int) dbComplaint.Complaint_SubCategory, true,
                                    dbSchools.School_Type, null);
                            hierarchyId = (Config.Hierarchy) assignmentModelList[0].SrcId;
                            userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);

                            BlSchool.AssignValuesAgainstAssignmentMatrix(listDbUsers, assignmentModelList, dbSchools,
                                ref hierarchyId,
                                ref userHierarchyVal, ref userCategoryId1, ref userCategoryId2, ref userCategoryId3);

                            List<DbUsers> listCurrentDbUsers =
                                UsersHandler.GetUsersPresentForCurrentHierarchy2(listDbUsers,
                                    hierarchyId,
                                    DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint, hierarchyId),
                                    userHierarchyVal, userCategoryId1, userCategoryId2);
                            //BlSchool.EvaluateAssignmentMartix(vmComplaint, listDbUsers, assignmentModelList, dbSchools, hierarchyId, userHierarchyVal, ref userCategoryId1, ref userCategoryId2, ref userCategoryId3, 0, null);

                            ReEvaluationAssignmentModel reEvaluationAssignmentModel = BlSchool.ReEvaluateEscallation(dbComplaint, dbSchools);
                            if (reEvaluationAssignmentModel.HasAssignmentChanged)
                            {
                                db.Entry(dbComplaint).State = EntityState.Modified;
                                BlSchool.SaveOrignalHierarchyLogInDb(db, dbComplaint);
                                DbComplaintsOriginLog dbComplaintOriginLog = null;


                                new Thread(delegate()
                                {
                                    BlSchool.SendMessageToStakeholder(DbComplaint.GetByComplaintId(dbComplaint.Id), dbSchools);
                                }).Start();
                            }

                            /*
                            if (listCurrentDbUsers.Count > 0) // if User exist 
                            {
                                //if (dbComplaint.Id == 198739)
                                {
                                    //ReEvaluationAssignmentModel reEvaluationAssignmentModel = BlSchool.ReEvaluateEscallation(dbComplaint, dbSchools);
                                    BlSchool.AssignComplaintToOrignalHierarchy(db, dbComplaint);
                                    //db.SaveChanges();
                                }
                            }
                            else // if user does not exist then assign to upper user
                            {
                                //DbComplaint dbComplaintToCompare = null;
                                
                                ReEvaluationAssignmentModel reEvaluationAssignmentModel = BlSchool.ReEvaluateEscallation(dbComplaint, dbSchools);
                                if (reEvaluationAssignmentModel.HasAssignmentChanged)
                                {
                                    db.Entry(dbComplaint).State = EntityState.Modified;
                                    BlSchool.SaveOrignalHierarchyLogInDb(db, dbComplaint);
                                    DbComplaintsOriginLog dbComplaintOriginLog = null;

                                }

                            }*/
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        DbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex,
                            Config.ServiceType.MarkComplaintToOriginUserIfPresent);

                    }
                }
                db.SaveChanges();

                db.Configuration.AutoDetectChangesEnabled = true;
            }
            catch (Exception ex)
            {
                //throw;
            }

        }
    }
}
