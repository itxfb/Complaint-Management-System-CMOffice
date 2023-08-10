using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using PITB.CMS_Common.Models;
using PITB.CMS_Common;
using PITB.CMS_Common.WindowServiceHandlers;
using PITB.CMS_Common.Handler.StakeHolder;
using PITB.CMS_Common.Handler.Business;

namespace PITB.CMS_WindowService
{
    public partial class Service1 : ServiceBase
    {
        private Timer Schedular;
        private bool isRunning;
        
        public Service1()
        {
            InitializeComponent();
            //OnStart(null);
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            FunctionScheduler.ScheduleFunctions();
            InitializeLicense();
        }

        protected override void OnStop()
        {
            //System.IO.File.Create(Environment.CurrentDirectory + "OnStop.txt");
        }



        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback)); // schedularCallBackWill Be called after timer
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                //this.WriteToFile("Simple Service Mode: " + mode + " {0}");

                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;

                if (mode.ToUpper() == "DAILY")
                {
                    //Get the Scheduled Time from AppSettings.
                    scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }

                if (mode.ToUpper() == "INTERVAL")
                {
                    //Get the Interval in Minutes from AppSettings.
                    double intervalMinutes = Convert.ToDouble(ConfigurationManager.AppSettings["IntervalMinutes"]);

                    //Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);

                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                //InsertDateWiseLog(DateTime.Now, 0, "Exception in Function ScheduleService : StackTrace = " + ex.StackTrace.ToString());
                //DBCroServiceErrors.Insert(20, "Function : Service1 ScheduleService  InnerException" + Utility.GetExceptionMessage(ex));
                using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
                {
                    serviceController.Stop();
                }
            }
        }


        public void InitializeLicense()
        {
            string licenseName = "80;100-BOARD";//... PRO license name
            string licenseKey = "907EF913D80CB02635FA9D305F006C50";//... PRO license key


            if (Z.BulkOperations.LicenseManager.ValidateLicense())
            {
                //Z.BulkOperations

            }
            else
            {
                Z.BulkOperations.LicenseManager.AddLicense(licenseName, licenseKey);
            }
        }


        public static bool IsServiceRunning = false;
        private void SchedularCallback(object e)
        {
            try
            {
                if (!IsServiceRunning)
                {
                    IsServiceRunning = true;
                    DateTime startUtilityDateTime = DateTime.Now;
                    //SmsHandler.SendStatusWiseStatsIndividualCampaign();
                    //SmsHandler.SendStatusWiseStatsCumulativeCampaign();

                    //SchoolEducationHandler.SendMessageToSchoolEducationUsers();
                    //SchoolEducationHandler.SendMessageToAllUsersForDepartment();

                    List<string> listFunctions = ConfigurationManager.AppSettings["FunctionsToExecute"].ToString().Split(',').ToList();

                    if (listFunctions.Where(n => n.Equals("SynRegionAndShoolsDataMain")).FirstOrDefault() != null)
                    {
                        DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain",
                            "Start SchoolEducationHandler.SynRegionAndShoolsDataMain()",
                            Config.ServiceType.SyncSchoolDataMain);
                        SchoolEducationHandler.SynRegionAndShoolsDataMain();
                        DbWindowServiceError.SaveErrorLog(2, "End SyncRegionAndSchoolDataMain",
                            "End SchoolEducationHandler.SynRegionAndShoolsDataMain()",
                            Config.ServiceType.SyncSchoolDataMain);

                    } // --- open comments

                    if (listFunctions.Where(n => n.Equals("MarkComplaintToOriginUserIfPresent")).FirstOrDefault() != null)
                    {
                        DbWindowServiceError.SaveErrorLog(2, "Start MarkComplaintToOriginUserIfPresent",
                            "Start SchoolEducationHandler.MarkComplaintToOriginUserIfPresent()",
                            Config.ServiceType.MarkComplaintToOriginUserIfPresent);
                        SchoolEducationHandler.MarkComplaintToOriginUserIfPresent();
                        DbWindowServiceError.SaveErrorLog(2, "End MarkComplaintToOriginUserIfPresent",
                            "End SchoolEducationHandler.MarkComplaintToOriginUserIfPresent()",
                            Config.ServiceType.MarkComplaintToOriginUserIfPresent);
                    }

                    if (listFunctions.Where(n => n.Equals("SycUserWiseComplaints")).FirstOrDefault() != null)
                    {
                        DbWindowServiceError.SaveErrorLog(2, "Start SycUserWiseComplaints",
                            "Start SchoolEducationHandler.SycUserWiseComplaints()",
                            Config.ServiceType.SycUserWiseComplaints);
                        SchoolEducationHandler.SycUserWiseComplaints_2();
                        DbWindowServiceError.SaveErrorLog(2, "End SycUserWiseComplaints",
                            "Start SchoolEducationHandler.SycUserWiseComplaints()",
                            Config.ServiceType.SycUserWiseComplaints);
                    }

                    if (listFunctions.Where(n => n.Equals("SendPendingOverDueSMS")).FirstOrDefault() != null)
                    {

                        DbWindowServiceError.SaveErrorLog(2, "Start SendPendingOverDueSMS",
                            "Start SchoolEducationHandler.SendPendingOverDueSMS()",
                            Config.ServiceType.SendPendingOverDueSMS);
                        SchoolEducationSMSHandler.SendPendingOverDueSMS(startUtilityDateTime);
                        DbWindowServiceError.SaveErrorLog(2, "End SendPendingOverDueSMS",
                            "Start SchoolEducationHandler.SendPendingOverDueSMS()",
                            Config.ServiceType.SendPendingOverDueSMS);
                    }


                    if (listFunctions.Where(n => n.Equals("DumpUserWiseSupervisorMapping")).FirstOrDefault() != null)
                    {

                        DbWindowServiceError.SaveErrorLog(2, "Start DumpUserWiseSupervisorMapping",
                            "Start SchoolEducationUsersHandler.DumpUserWiseSupervisorMapping(2, 47, (int) Config.Hierarchy.UnionCouncil);",
                            Config.ServiceType.SendPendingOverDueSMS);

                        SchoolEducationUsersHandler.DumpUserWiseSupervisorMapping(2, 47,
                            (int) Config.Hierarchy.UnionCouncil);

                        DbWindowServiceError.SaveErrorLog(2, "End DumpUserWiseSupervisorMapping",
                           "Start SchoolEducationUsersHandler.DumpUserWiseSupervisorMapping(2, 47, (int) Config.Hierarchy.UnionCouncil);",
                           Config.ServiceType.SendPendingOverDueSMS);
                    }
                    //----- end open
                     

                    if (startUtilityDateTime.DayOfWeek == System.DayOfWeek.Monday)
                    {
                        //SchoolEducationHandler.SendMessageToSecretarySchoolEducation();
                    }
                    //MainHandler.Instance.StartPushingData();

                    this.ScheduleService();
                    IsServiceRunning = false;
                }
            }
            catch (Exception ex)
            {
                IsServiceRunning = false;
                this.ScheduleService();
                DbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex, Config.ServiceType.MainServiceError);
            }
        }

        public static void CallInfinitely()
        {
            System.Threading.Thread.Sleep(5000);
            Debug.WriteLine("Main hn munna kaka");
        }

        public static void StartImageTransfer(Dictionary<string, string> dictParam)
        {
            if (dictParam["IsLocked"] == "true") return;

            dictParam["IsLocked"] = "true";
            DateTime startUtilityDateTime = DateTime.Now;
            DbWindowServiceError.SaveErrorLog(2, "Start StartImageTransfer",
                            "Start StartImageTransfer()",
                            Config.ServiceType.ImageTransfer);
            ImageTransferHandler.Start();
            DbWindowServiceError.SaveErrorLog(2, "End StartImageTransfer",
                "Start StartImageTransfer",
                Config.ServiceType.ImageTransfer);
            //dictParam["IsLocked"] = "false";
        }
        public static void SyncDataMain(Dictionary<string,string> dictParam)
        {
            if (dictParam["IsLocked"] == "true") return;

            dictParam["IsLocked"] = "true";
            string subFunct = ConfigurationManager.AppSettings["SubFunctionsToExecute"].ToString();
            Dictionary<string,string> dict = Utility.ConvertCollonFormatToDict(subFunct);
            if (dict.ContainsKey("SyncDataMain"))
            {

                List<string> listFunctions = dict["SyncDataMain"].Split(',').ToList();

                //if (listFunctions.Where(n => n.Equals("SynRegionAndShoolsDataMain")).FirstOrDefault() != null)
                //{
                //    DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain",
                //        "Start SchoolEducationHandler.SynRegionAndShoolsDataMain()",
                //        Config.ServiceType.SyncSchoolDataMain);
                //    SchoolEducationHandler.SynRegionAndShoolsDataMain();
                //    DbWindowServiceError.SaveErrorLog(2, "End SyncRegionAndSchoolDataMain",
                //        "End SchoolEducationHandler.SynRegionAndShoolsDataMain()",
                //        Config.ServiceType.SyncSchoolDataMain);

                //} // --- open comments

                //if (listFunctions.Where(n => n.Equals("MarkComplaintToOriginUserIfPresent")).FirstOrDefault() != null)
                //{
                //    DbWindowServiceError.SaveErrorLog(2, "Start MarkComplaintToOriginUserIfPresent",
                //        "Start SchoolEducationHandler.MarkComplaintToOriginUserIfPresent()",
                //        Config.ServiceType.MarkComplaintToOriginUserIfPresent);
                //    SchoolEducationHandler.MarkComplaintToOriginUserIfPresent();
                //    DbWindowServiceError.SaveErrorLog(2, "End MarkComplaintToOriginUserIfPresent",
                //        "End SchoolEducationHandler.MarkComplaintToOriginUserIfPresent()",
                //        Config.ServiceType.MarkComplaintToOriginUserIfPresent);
                //}

                if (listFunctions.Where(n => n.Equals("SycUserWiseComplaints")).FirstOrDefault() != null)
                {
                    DbWindowServiceError.SaveErrorLog(2, "Start SycUserWiseComplaints",
                        "Start SchoolEducationHandler.SycUserWiseComplaints()",
                        Config.ServiceType.SycUserWiseComplaints);
                    SchoolEducationHandler.SycUserWiseComplaints_2();
                    DbWindowServiceError.SaveErrorLog(2, "End SycUserWiseComplaints",
                        "Start SchoolEducationHandler.SycUserWiseComplaints()",
                        Config.ServiceType.SycUserWiseComplaints);
                }
            }
            dictParam["IsLocked"] = "false";
        }

        public static void SendPendingOverDueSMS(Dictionary<string, string> dictParam)
        {
            //return;
            if (dictParam["IsLocked"] == "true") return;

            dictParam["IsLocked"] = "true";
            DateTime startUtilityDateTime = DateTime.Now;
            DbWindowServiceError.SaveErrorLog(2, "Start SendPendingOverDueSMS",
                            "Start SchoolEducationHandler.SendPendingOverDueSMS()",
                            Config.ServiceType.SendPendingOverDueSMS);
            SchoolEducationSMSHandler.SendPendingOverDueSMS(startUtilityDateTime);
            DbWindowServiceError.SaveErrorLog(2, "End SendPendingOverDueSMS",
                "Start SchoolEducationHandler.SendPendingOverDueSMS()",
                Config.ServiceType.SendPendingOverDueSMS);
            dictParam["IsLocked"] = "false";
        }

        public static void SendStatusWiseStatsforCampaignList(Dictionary<string, string> dictParam)
        {
            //return;
            if (dictParam["IsLocked"] == "true") return;

            dictParam["IsLocked"] = "true";
            DateTime startUtilityDateTime = DateTime.Now;
            DbWindowServiceError.SaveErrorLog(2, "Start SendStatusWiseStatsforCampaignList",
                            "Start SmsHandler.SendStatusWiseStatsforCampaignList()",
                            Config.ServiceType.SendPendingOverDueSMS);
            SmsHandler.SendStatusWiseStatsforCampaignList(new Dictionary<int, string>(){ {83,"03007769960,03314714621"}}, new List<int>(){1,6,7,8});
            DbWindowServiceError.SaveErrorLog(2, "End SendPendingOverDueSMS",
                "Start SmsHandler.SendStatusWiseStatsforCampaignList()",
                Config.ServiceType.SendPendingOverDueSMS);
            dictParam["IsLocked"] = "false";
        }

        public static void SendSmsToExecutives(Dictionary<string, string> dictParam)
        {
            //return;
            if (dictParam["IsLocked"] == "true") return;

            dictParam["IsLocked"] = "true";
            DateTime startUtilityDateTime = DateTime.Now;
            DbWindowServiceError.SaveErrorLog(2, "Start SendSmsToExecutives",
                            "Start SendSmsToExecutives()",
                            Config.ServiceType.SendSmsToExecutives);
            try
            {
                BlExecutive.SendSmsToExecutives(DateTime.Parse("11/28/2018"), DateTime.Today.AddDays(-1));
                BlExecutive.SendHierarchyMessages(DateTime.Parse("11/28/2018"), DateTime.Today.AddDays(-1));
            }
            catch (Exception ex)
            {
                DbWindowServiceError.SaveErrorLog(2, " SendSmsToExecutives", "Exception in BlExecutive send sms = " + ex.Message + " == " + ex.StackTrace.ToString(), Config.ServiceType.SendSmsToExecutives);

            }
            DbWindowServiceError.SaveErrorLog(2, "End SendSmsToExecutives",
                "Start SendSmsToExecutives()",
                Config.ServiceType.SendSmsToExecutives);
            dictParam["IsLocked"] = "false";
        }


    }
}
