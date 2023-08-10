using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Models;
using PITB.CMS_WindowService.Models.Custom;


namespace PITB.CMS_WindowService.Handlers
{
    public class SchoolEducationSMSHandler
    {
        public static void SendPendingOverDueSMS(DateTime currDateTime)
        {
            try
            {

                int campaignId = (int) Config.Campaign.SchoolEducationEnhanced;
                int statusId = (int) Config.ComplaintStatus.UnsatisfactoryClosed;

                List<Tuple<Config.Hierarchy, int?, Config.ExcutionModel>> listHierarchyToDendMessage =
                    new List<Tuple<Config.Hierarchy, int?, Config.ExcutionModel>>();
                listHierarchyToDendMessage.Add(
                    new Tuple<Config.Hierarchy, int?, Config.ExcutionModel>(Config.Hierarchy.Province, 0,
                        new Config.ExcutionModel(Config.ExcutionSpan.Weekly, System.DayOfWeek.Monday)));
                listHierarchyToDendMessage.Add(
                    new Tuple<Config.Hierarchy, int?, Config.ExcutionModel>(Config.Hierarchy.Division, 0,
                        new Config.ExcutionModel(Config.ExcutionSpan.Weekly, System.DayOfWeek.Monday)));
                listHierarchyToDendMessage.Add(
                    new Tuple<Config.Hierarchy, int?, Config.ExcutionModel>(Config.Hierarchy.District, 20,
                        new Config.ExcutionModel(Config.ExcutionSpan.Daily)));
                listHierarchyToDendMessage.Add(
                    new Tuple<Config.Hierarchy, int?, Config.ExcutionModel>(Config.Hierarchy.District, 10,
                        new Config.ExcutionModel(Config.ExcutionSpan.Daily)));
                listHierarchyToDendMessage.Add(
                    new Tuple<Config.Hierarchy, int?, Config.ExcutionModel>(Config.Hierarchy.Tehsil, 0,
                        new Config.ExcutionModel(Config.ExcutionSpan.Daily)));
                listHierarchyToDendMessage.Add(
                    new Tuple<Config.Hierarchy, int?, Config.ExcutionModel>(Config.Hierarchy.UnionCouncil, 0,
                        new Config.ExcutionModel(Config.ExcutionSpan.Daily)));

                List<DbComplaint> listDbComplaints =
                    DbComplaint.GetByCampaignId((int) Config.Campaign.SchoolEducationEnhanced,
                        Config.ComplaintType.Complaint);
                listDbComplaints = listDbComplaints.Where(n => n.Complaint_Computed_Status_Id == statusId).ToList();
                //List<DbComplaint> listTempDbComplaints = null;

                List<DbUsers> listDbUsers = DbUsers.GetActiveUserAgainstParams(campaignId);
                List<DbUsers> listTempDbUsers = null;

                for (int i = 0; i < listHierarchyToDendMessage.Count - 1; i++)
                {

                    if ((listHierarchyToDendMessage[i].Item3.ExcutionSpan == Config.ExcutionSpan.Weekly &&
                         currDateTime.DayOfWeek == listHierarchyToDendMessage[i].Item3.ExcutionDay)
                        || listHierarchyToDendMessage[i].Item3.ExcutionSpan == Config.ExcutionSpan.Daily)
                    {
                        listTempDbUsers =
                            listDbUsers.Where(
                                n =>
                                    n.Hierarchy_Id == listHierarchyToDendMessage[i].Item1 &&
                                    n.User_Hierarchy_Id == listHierarchyToDendMessage[i].Item2)
                                .ToList();

                        SendMessageToHierarchy(listHierarchyToDendMessage, i, listDbComplaints, listTempDbUsers);
                    }

                }

            }
            catch (Exception ex)
            {
                
            }
        }


        public static void SendMessageToHierarchy(List<Tuple<Config.Hierarchy, int?, Config.ExcutionModel>> listHierarchyTuple, int index, List<DbComplaint> listDbComplaints, List<DbUsers> listDbUsers)
        {
            if (listDbUsers.Count == 0)
            {
                return;
            }
            Tuple<Config.Hierarchy, int?, Config.ExcutionModel> hierarchyTuple = listHierarchyTuple[index];
            Tuple<Config.Hierarchy, int?, Config.ExcutionModel> hierarchyTupleSubordinates = listHierarchyTuple[index + 1];


            int? subordinateHierarchyId = (int?) hierarchyTupleSubordinates.Item1;
            int? subordinateUserHierarchyId = (int?) hierarchyTupleSubordinates.Item2;
            

            //List<DbComplaint> listTempDbComplaintsSubordinates = listDbComplaints.Where(
            //            n => (n.MaxSrcId == subordinateHierarchyId && n.UserCategoryId1 < subordinateUserHierarchyId)||
            //                (n.MaxSrcId > subordinateHierarchyId)).ToList();

            List<DbComplaint> listTempDbComplaintsSubordinates = null;

            List<Tuple<int, int>> listRegionCountTuples = new List<Tuple<int, int>>();
            List<Tuple<int, int>> listOverdueCountTuples = new List<Tuple<int, int>>();
            List<Tuple<int, int>> listTopCategoryCountTuples = new List<Tuple<int, int>>();

            SmsOverDueModel smsOverdueModel = null;
            string smsStr = "";

            List<DbComplaint> listSubordinateComplaints = null;
            List<Tuple<int?, int?>> listTuple = null;
            
            List<DbDivision> listDivision = DbDivision.GetByProvinceId(1).Where(n=>n.Division_Id<10).ToList();
            List<DbDistrict> listDistricts =
                DbDistrict.GetByDivisionIdsStrAndGroupId(
                    Utility.GetCommaSepStrFromList(listDivision.Select(n => n.Division_Id).ToList()),null);
            List<DbTehsil> listTehsils = DbTehsil.GetByDistrictIdList(Utility.GetNullableIntList(listDistricts.Select(n => n.District_Id).ToList()) ,3);
            List<DbUnionCouncils> listUc = DbUnionCouncils.GetByTehsilIdsStr(Utility.GetCommaSepStrFromList(listTehsils.Select(n => n.Tehsil_Id).ToList()) ,4);

            //List<DbDepartment> listDepartment =
            //    DbDepartment.GetByCampaignAndGroupId((int)Config.Campaign.SchoolEducationEnhanced, null);

            //List<DbComplaintType> listComplaintType =
            //    DbComplaintType.GetByDepartmentIds(Utility.GetNullableIntList(listDepartment.Select(n => n.Id).ToList()));

            List<DbComplaintSubType> listComplaitSubType =
                DbComplaintSubType.GetAllSubTypesByDepartmentAndGroup((int)Config.Campaign.SchoolEducationEnhanced,null);
            
            string tempHierarchyName = "";
            string tempHierarchyVal = "";
            int tempCount = 0;
            List<DbUserWiseComplaints> listUserWiseComplaints = new List<DbUserWiseComplaints>();
            //string smsStr = null;

            foreach (DbUsers dbUser in listDbUsers)
            {
                try
                {

                    List<DbComplaint> listUserComplaints = BlSchool.GetComplaintsAgainstUser(dbUser, Config.StakeholderComplaintListingType.UptilMyHierarchy, Config.ComplaintType.Complaint, Config.SelectionType.Specific);
                    listSubordinateComplaints = listDbComplaints.Where(n => listUserComplaints.Any(x => x.Id == n.Id)).ToList();
                    listTempDbComplaintsSubordinates = listSubordinateComplaints.Select(n=>n).ToList();

                    smsOverdueModel = new SmsOverDueModel();
                    listTuple = null;

                    string hierarchyName = Enum.GetName(typeof (Config.Hierarchy), hierarchyTuple.Item1);
                    string hierarchyValName = Utility.GetHierarchyValueName(hierarchyTuple.Item1,
                        Utility.GetIntByCommaSepStr(DbUsers.GetHierarchy(hierarchyTuple.Item1, dbUser)));


                    if (dbUser.Hierarchy_Id == Config.Hierarchy.Province)
                    {
                        tempHierarchyName = "Division";
                        listSubordinateComplaints = listTempDbComplaintsSubordinates.Where(
                            n => n.Province_Id == Utility.GetIntByCommaSepStr(dbUser.Province_Id)).ToList();

                        listTuple =
                            listSubordinateComplaints.GroupBy(n => new {n.Division_Id})
                                .Select(m => new Tuple<int?, int?>(m.Key.Division_Id, m.Count()))
                                .Where(d => d.Item1 != null)
                                .OrderByDescending(s => s.Item2)
                                .ToList();

                        foreach (DbDivision dbDivision in listDivision.Where(n => n.Province_Id == Utility.GetIntByCommaSepStr(dbUser.Province_Id)))
                        {
                            if (listTuple.Where(n => n.Item1 == dbDivision.Division_Id).FirstOrDefault() == null)
                            {
                                listTuple.Add(new Tuple<int?, int?>(dbDivision.Division_Id, 0));
                            }
                        }

                        // Highest overdue complaints
                        //DbDivision dbDivision = listDivision.Where(n=>n.Division_Id == Convert.ToInt32(listTuple.FirstOrDefault().Item1)).FirstOrDefault();

                        //if (listTuple.FirstOrDefault() == null)
                        //{
                        //    tempHierarchyVal = "None";
                        //    tempCount = 0;
                        //}
                        //else
                        //{
                        //    tempHierarchyVal = listDivision.Where(n=>n.Division_Id == Convert.ToInt32(listTuple.FirstOrDefault().Item1)).FirstOrDefault().Division_Name;
                        //    tempCount = Convert.ToInt32(listTuple.FirstOrDefault().Item2);

                        //}
                        //smsOverdueModel.HighestOverdue = "Highest Overdue Complaints (" + tempHierarchyVal + " " +
                        //                                 tempHierarchyName + ")";

                        //smsOverdueModel.Region1 = tempHierarchyVal + " "+ tempHierarchyName + " ("+tempCount+")";


                        //// Lowest overdue complaints

                        //if (listTuple.LastOrDefault() == null)
                        //{
                        //    tempHierarchyVal = "None";
                        //    tempCount = 0;
                        //}
                        //else
                        //{
                        //    tempHierarchyVal = listDivision.Where(n=>n.Division_Id == Convert.ToInt32(listTuple.LastOrDefault().Item1)).LastOrDefault().Division_Name;
                        //    tempCount = Convert.ToInt32(listTuple.LastOrDefault().Item2);
                        //}
                        //smsOverdueModel.LowestOverdue = "Lowest Overdue Complaints (" + tempHierarchyVal + " " +
                        //                                 tempHierarchyName + ")";

                        //smsOverdueModel.Region2 = tempHierarchyVal + " "+ tempHierarchyName + " ("+tempCount+")";

                        CommonMsgCompiler(listSubordinateComplaints, smsOverdueModel, dbUser, hierarchyName,
                            hierarchyValName);

                        // List All Counts

                        smsOverdueModel.ListRegionOverdue = new List<string>();

                        for (int i = 0; i < listTuple.Count; i++)
                        {
                            //if(i != 0 && i != listTuple.Count - 1) // skip first and last
                            {
                                tempHierarchyVal =
                                    listDivision.Where(n => n.Division_Id == Convert.ToInt32(listTuple[i].Item1))
                                        .LastOrDefault()
                                        .Division_Name;
                                smsOverdueModel.ListRegionOverdue.Add(tempHierarchyVal + ": "+listTuple[i].Item2 );
                            }
                        }

                        // Overdue msgs

                        //smsOverdueModel.Overdue7Days ="Overdue (0-7 days): "+
                        //    listSubordinateComplaints.Where(n => ((TimeSpan) (DateTime.Now - n.MaxSrcIdDate)).TotalDays <= 7)
                        //        .Count();

                        //smsOverdueModel.Overdue30Days ="Overdue (7-30 days): "+
                        //    listSubordinateComplaints.Where(n => ((TimeSpan) (DateTime.Now - n.MaxSrcIdDate)).TotalDays <= 30 && ((TimeSpan) (DateTime.Now - n.MaxSrcIdDate)).TotalDays > 7)
                        //        .Count();

                        //smsOverdueModel.Overdue90Days ="Overdue (30-90 days): "+
                        //    listSubordinateComplaints.Where(n => ((TimeSpan) (DateTime.Now - n.MaxSrcIdDate)).TotalDays <= 90 && ((TimeSpan) (DateTime.Now - n.MaxSrcIdDate)).TotalDays > 30)
                        //        .Count();

                        //smsOverdueModel.Overdue90PlusDays ="Overdue (90+ days): "+
                        //    listSubordinateComplaints.Where(n => ((TimeSpan) (DateTime.Now - n.MaxSrcIdDate)).TotalDays >= 90)
                        //        .Count();

                        // DbDivision.GetByProvinceId(Utility.GetIntByCommaSepStr(dbUser.Province_Id)).ToList();




                        //listTuple =
                        //    listSubordinateComplaints.GroupBy(n => new {n.Complaint_SubCategory})
                        //        .Select(m => new Tuple<int?, int?>(m.Key.Complaint_SubCategory, m.Count()))
                        //        .Where(d=>d.Item1!=null)
                        //        .OrderByDescending(s=>s.Item2)
                        //        .ToList();



                        //smsOverdueModel.TopSubCatHeading = "Top 3 sub-categories: ";
                        //smsOverdueModel.ListTopSubcategories = new List<string>();

                        //for (int i = 0; i < listTuple.Count && i<3; i++)
                        //{
                        //    smsOverdueModel.ListTopSubcategories.Add(listComplaitSubType.Where(n=>n.Complaint_SubCategory==listTuple[i].Item1).FirstOrDefault().Name+" ("+listTuple[i].Item2+")");
                        //}

                    }



                    else if (dbUser.Hierarchy_Id == Config.Hierarchy.Division)
                    {
                        tempHierarchyName = "District";
                        listSubordinateComplaints = listTempDbComplaintsSubordinates.Where(
                            n => n.Division_Id == Utility.GetIntByCommaSepStr(dbUser.Division_Id)).ToList();

                        listTuple =
                            listSubordinateComplaints.GroupBy(n => new {n.District_Id})
                                .Select(m => new Tuple<int?, int?>(m.Key.District_Id, m.Count()))
                                .Where(d => d.Item1 != null)
                                .OrderBy(s => s.Item2)
                                .ToList();

                        foreach (DbDistrict dbDistrict in listDistricts.Where(n => n.Division_Id == Utility.GetIntByCommaSepStr(dbUser.Division_Id)))
                        {
                            if (listTuple.Where(n => n.Item1 == dbDistrict.District_Id).FirstOrDefault() == null)
                            {
                                listTuple.Add(new Tuple<int?, int?>(dbDistrict.District_Id, 0));
                            }
                        }
                        //listTuple.AddRange(listDistricts.Where(n=> !listTuple.Contains(n.District_Id)));

                        CommonMsgCompiler(listSubordinateComplaints, smsOverdueModel, dbUser, hierarchyName,
                            hierarchyValName);

                        // List All Counts

                        smsOverdueModel.ListRegionOverdue = new List<string>();

                        for (int i = 0; i < listTuple.Count; i++)
                        {
                            //if(i != 0 && i != listTuple.Count - 1) // skip first and last
                            {
                                tempHierarchyVal =
                                    listDistricts.Where(n => n.District_Id == Convert.ToInt32(listTuple[i].Item1))
                                        .LastOrDefault()
                                        .District_Name;
                                smsOverdueModel.ListRegionOverdue.Add(tempHierarchyVal + ": " + listTuple[i].Item2);
                            }
                        }


                    }



                    else if (dbUser.Hierarchy_Id == Config.Hierarchy.District)
                    {
                        tempHierarchyName = "Tehsil";
                        listSubordinateComplaints = listTempDbComplaintsSubordinates.Where(
                            n => n.District_Id == Utility.GetIntByCommaSepStr(dbUser.District_Id)).ToList();

                        listTuple =
                            listSubordinateComplaints.GroupBy(n => new {n.Tehsil_Id})
                                .Select(m => new Tuple<int?, int?>(m.Key.Tehsil_Id, m.Count()))
                                .Where(d => d.Item1 != null)
                                .OrderBy(s => s.Item2)
                                .ToList();

                        foreach (DbTehsil dbTehsil in listTehsils.Where(n => n.District_Id == Utility.GetIntByCommaSepStr(dbUser.District_Id)))
                        {
                            if (listTuple.Where(n => n.Item1 == dbTehsil.Tehsil_Id).FirstOrDefault() == null)
                            {
                                listTuple.Add(new Tuple<int?, int?>(dbTehsil.Tehsil_Id, 0));
                            }
                        }

                        CommonMsgCompiler(listSubordinateComplaints, smsOverdueModel, dbUser, hierarchyName,
                            hierarchyValName);

                        // List All Counts

                        smsOverdueModel.ListRegionOverdue = new List<string>();

                        for (int i = 0; i < listTuple.Count; i++)
                        {
                            //if(i != 0 && i != listTuple.Count - 1) // skip first and last
                            {
                                tempHierarchyVal =
                                    listTehsils.Where(n => n.Tehsil_Id == Convert.ToInt32(listTuple[i].Item1))
                                        .FirstOrDefault()
                                        .Tehsil_Name;
                                smsOverdueModel.ListRegionOverdue.Add(tempHierarchyVal + ": " + listTuple[i].Item2);
                            }
                        }
                    }



                    else if (dbUser.Hierarchy_Id == Config.Hierarchy.Tehsil)
                    {
                        tempHierarchyName = "Markaz";
                        listSubordinateComplaints = listTempDbComplaintsSubordinates.Where(
                            n => n.Tehsil_Id == Utility.GetIntByCommaSepStr(dbUser.Tehsil_Id)).ToList();

                        listTuple =
                            listSubordinateComplaints.GroupBy(n => new {n.UnionCouncil_Id})
                                .Select(m => new Tuple<int?, int?>(m.Key.UnionCouncil_Id, m.Count()))
                                .Where(d => d.Item1 != null)
                                .OrderBy(s => s.Item2)
                                .ToList();

                        foreach (DbUnionCouncils dbUc in listUc.Where(n => n.Tehsil_Id == Utility.GetIntByCommaSepStr(dbUser.Tehsil_Id)))
                        {
                            if (listTuple.Where(n => n.Item1 == dbUc.UnionCouncil_Id).FirstOrDefault() == null)
                            {
                                listTuple.Add(new Tuple<int?, int?>(dbUc.UnionCouncil_Id, 0));
                            }
                        }

                        CommonMsgCompiler(listSubordinateComplaints, smsOverdueModel, dbUser, hierarchyName,
                            hierarchyValName);

                        // List All Counts

                        smsOverdueModel.ListRegionOverdue = new List<string>();

                        for (int i = 0; i < listTuple.Count; i++)
                        {
                            //if(i != 0 && i != listTuple.Count - 1) // skip first and last
                            {
                                tempHierarchyVal =
                                    listUc.Where(n => n.UnionCouncil_Id == Convert.ToInt32(listTuple[i].Item1))
                                        .FirstOrDefault()
                                        .Councils_Name;
                                smsOverdueModel.ListRegionOverdue.Add(tempHierarchyVal + ": " + listTuple[i].Item2);
                            }
                        }
                    }


                    smsStr = CompileMessageInOneString(smsOverdueModel);
                    SmsModel smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, dbUser.Phone, smsStr,
                    (int)Config.MsgType.ToStakeholder,
                    (int)Config.MsgSrcType.WindowService, DateTime.Now, 1, (int)dbUser.Hierarchy_Id);



                    new Thread(delegate()
                    {
                        TextMessageHandler.SendMessageToPhoneNo(dbUser.Phone, smsStr, true, smsModel);
                    }).Start();

                }
                catch (Exception ex)
                {
                    
                    DbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex, Config.ServiceType.SendPendingOverDueSMS);
                }
            }

        }

        private static void CommonMsgCompiler(List<DbComplaint> listSubordinateComplaints, SmsOverDueModel smsOverdueModel, DbUsers dbUser, string hierarchyName, string hierarchyValName)
        {
            smsOverdueModel.Overdue7Days = "Overdue (0-7 days): " +
                      listSubordinateComplaints.Where(n => ((TimeSpan)(DateTime.Now - n.MaxSrcIdDate)).TotalDays <= 7)
                          .Count();

            smsOverdueModel.Overdue30Days = "Overdue (7-30 days): " +
                listSubordinateComplaints.Where(n => ((TimeSpan)(DateTime.Now - n.MaxSrcIdDate)).TotalDays <= 30 && ((TimeSpan)(DateTime.Now - n.MaxSrcIdDate)).TotalDays > 7)
                    .Count();

            smsOverdueModel.Overdue90Days = "Overdue (30-90 days): " +
                listSubordinateComplaints.Where(n => ((TimeSpan)(DateTime.Now - n.MaxSrcIdDate)).TotalDays <= 90 && ((TimeSpan)(DateTime.Now - n.MaxSrcIdDate)).TotalDays > 30)
                    .Count();

            smsOverdueModel.Overdue90PlusDays = "Overdue (90+ days): " +
                listSubordinateComplaints.Where(n => ((TimeSpan)(DateTime.Now - n.MaxSrcIdDate)).TotalDays >= 90)
                    .Count();

            smsOverdueModel.UserName = "M-Gov Punjab";//dbUser.Name.Trim() + "(" + dbUser.Designation + ")";
            smsOverdueModel.Date = "" + DateTime.Now.ToString("dd MMMM yyyy");
            smsOverdueModel.HelplineName = "Education Helpline";
            smsOverdueModel.OverdueComplaintRegion = hierarchyName + ": " + hierarchyValName+"\n" + "Overdue Complaints"  ;// + "<"+hierarchyName+" wise>";
            smsOverdueModel.TotalOverdue = "Total Overdue: " + listSubordinateComplaints.Count + "";
            //smsOverdueModel.ResolutionReminder = "Please ensure timely resolution of Overdue Complaints";
            smsOverdueModel.MsgFooter = "For details, login to \nwww.crm.punjab.gov.pk \nPowered by PITB for SED";
        }

        public static string CompileMessageInOneString(SmsOverDueModel smsOverdueModel)
        {
            string smsStr = "";
            smsStr = smsStr
                     + smsOverdueModel.UserName + "\n"
                     + smsOverdueModel.HelplineName + "\n\n"
                     + smsOverdueModel.OverdueComplaintRegion + "\n"
                     + smsOverdueModel.Date + "\n\n";

            foreach (string str in smsOverdueModel.ListRegionOverdue)
            {
                smsStr = smsStr + str + "\n";
            }
            smsStr = smsStr + "\n";
                     
                     //+ smsOverdueModel.HighestOverdue + "\n"
                     //+ smsOverdueModel.LowestOverdue + "\n";

            //smsStr = smsStr + "\n";
            smsStr = smsStr + smsOverdueModel.Overdue90PlusDays + "\n";
            smsStr = smsStr + smsOverdueModel.Overdue90Days + "\n";
            smsStr = smsStr + smsOverdueModel.Overdue30Days + "\n";
            smsStr = smsStr + smsOverdueModel.Overdue7Days + "\n";
            smsStr = smsStr + smsOverdueModel.TotalOverdue + "\n";
            smsStr = smsStr + "\n";

            
            //smsStr = smsStr + "\n";
            //smsStr = smsStr + smsOverdueModel.ResolutionReminder+"\n";
            smsStr = smsStr + smsOverdueModel.MsgFooter;

            //smsStr = smsStr + smsOverdueModel.TopSubCatHeading + "\n";

            //foreach (string str in smsOverdueModel.ListTopSubcategories)
            //{
            //    smsStr = smsStr + str + "\n";
            //}
            return smsStr;
        }
    }
}
