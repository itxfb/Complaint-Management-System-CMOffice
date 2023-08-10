using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PITB.CMS_WindowService
{
    public class FunctionScheduler
    {
        //private string str = "FunctionName::asdasd__TimeToExec::23:00__TimeFormat::Daily|";
        private static List<Timer> listTimer = null;
        public static void ScheduleFunctions()
        {
            listTimer = new List<Timer>();
            List<string> listFunctionConfig = ConfigurationManager.AppSettings["FunctionsConfig"].ToString().Split('|').ToList();
            int c = 0;
            foreach (string functionConfig in listFunctionConfig)
            {
                Dictionary<string, string> dictFunctionConfig = Utility.ConvertCollonFormatToDict(functionConfig);
                dictFunctionConfig["IsLocked"] = "false";
                dictFunctionConfig["dueTime"] = "0";
                int dueTime = GetDueTime(dictFunctionConfig);
                SetTimerFunctionCallback(dictFunctionConfig, dueTime);
                c++;
            }
        }

        //public static void ScheduleFunction(Dictionary<string, string> dict, float dueTimeMilliseconds)
        public static void ScheduleFunction(object obj)
        {
           Dictionary<string, string> dict = (Dictionary<string, string>) (obj);
            //Debug.WriteLine("Munnay kakay");
            //if (dict["IsLocked"] == "true")
            //{
            //    return;
            //}
            if (dict["FunctionName"] == "SyncDataMain")
            {
                //dict["IsLocked"] = "true";
                new Thread(delegate()
                {
                     Service1.SyncDataMain(dict);
                   // Service1.CallInfinitely();
                }).Start();
               
            }
            else if (dict["FunctionName"] == "StartImageTransfer")
            {
                //dict["IsLocked"] = "true";
                new Thread(delegate()
                {
                    Service1.StartImageTransfer(dict);
                    // Service1.CallInfinitely();
                }).Start();

            }
            else if (dict["FunctionName"] == "SendPendingOverDueSMS")
            {
                new Thread(delegate()
                {
                    Service1.SendPendingOverDueSMS(dict);
                }).Start();
            }
            else if (dict["FunctionName"] == "SendSmsToExecutives")
            {
                new Thread(delegate()
                {
                    Service1.SendSmsToExecutives(dict);
                }).Start();
            }
            else if (dict["FunctionName"] == "SendStatusWiseStatsforCampaignList")
            {
                //dict["IsLocked"] = "true";
                new Thread(delegate()
                {
                    Service1.SendStatusWiseStatsforCampaignList(dict);
                    // Service1.CallInfinitely();
                }).Start();

            }
            int dueTime = GetDueTime(dict);
            SetTimerFunctionCallback(dict, dueTime);
        }

        private static void SetTimerFunctionCallback(Dictionary<string, string> dict, int dueTimeMilliseconds)
        {
            //Timer timer = new Timer(new TimerCallback(ScheduleFunction),dict,dueTimeMilliseconds,0);
            Timer timer = null;
            timer = new Timer(new TimerCallback(y =>
            {
                try
                {
                    ScheduleFunction(dict);
                    // here i want to dispose this timer
                    listTimer.Remove(timer);
                    timer.Dispose();
                }
                catch
                {
                }
            }));
            listTimer.Add(timer);
            timer.Change(dueTimeMilliseconds, Timeout.Infinite);
            
        }

        private static int GetDueTime(Dictionary<string, string> dictFunctionConfig)
        {
            DateTime scheduledTime = DateTime.MinValue; 
            string frequency = dictFunctionConfig["Frequency"];
                
            if (frequency == "Daily")
            {
                scheduledTime = DateTime.Parse(dictFunctionConfig["TimeToExec"]);
                if (DateTime.Now > scheduledTime)
                {
                    //If Scheduled Time is passed set Schedule for the next day.
                    scheduledTime = scheduledTime.AddDays(1);
                }
            }

            if (frequency == "Interval")
            {
                double intervalMinutes = Convert.ToDouble(dictFunctionConfig["TimeToExec"]);

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
            return dueTime;
        }
    }
}
