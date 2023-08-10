using System.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Handler.Complaint;
using PITB.CMS.Handler.StakeHolder;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.Custom.DataTable;
using PITB.CMS.Models.Custom.Reports;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Handler.DataTableJquery;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using PITB.CMS.Helper;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;

namespace PITB.CMS.Handler.Business
{
    public class BlLwmc : BlComplaints
    {
        public static DataTable GetStakeHolderServerSideListDenormalized(string from, string to, DataTableParamsModel dtModel, string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            DbUsers dbUser = null;
            if (userId == -1)
            {
                dbUser = Utility.GetUserFromCookie();
            }
            else
            {
                dbUser = DbUsers.GetActiveUser(userId);
            }
            if (dtModel != null)
            {
                Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
                //dictOrderQuery.Add("Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate))");
                //List<string> prefixStrList = new List<string> { "a", "a", "a", "a", "a", "a", "a", "a", "a" };


                List<string> prefixStrList = new List<string>
					{
						"complaints",
						"complaints",
						"complaints",
						"complaints",
						"complaints",
						"complaints",
						"complaints",
						"complaints",
						"complaints",
						"complaints"
					};
                // for joins
                //List<string> prefixStrList = new List<string> { "complaints", "campaign", "districts", "tehsil", "personInfo", "complaints", "complaintType", "Statuses", "complaints" };
                //dictOrderQuery.Add("Complaint_Category_Name", "complaintType.name");
                //dictOrderQuery.Add("Complaint_Computed_Status", "Statuses.Status");
                DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList, dictOrderQuery);

                Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
                //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
                dictFilterQuery.Add("complaints.Created_Date",
                    "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

                // for joins
                //dictFilterQuery.Add("complaintType.Complaint_Category_Name", "complaintType.name Like '%_Value_%'");
                //dictFilterQuery.Add("Statuses.Complaint_Computed_Status", "Statuses.[Status] Like '%_Value_%'");
                DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixStrList,
                    dictFilterQuery);
                //return GetComplaintsOfStakeholderServerSide(from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);

            }
            ListingParamsModelBase paramsModel = SetStakeholderListingParams(dbUser, from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
            string queryStr = StakeholderListingLogic.GetListingQuery(paramsModel);
            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }

        public struct StateInfoArray
        {
            public StateInfo[] info;
            public EventWaitHandle manualEvent;
            public StateInfoArray(StateInfo[] info, EventWaitHandle reset)
            {
                manualEvent = reset;
                this.info = info;
            }
            public StateInfoArray(StateInfo[] info)
            {
                manualEvent = new ManualResetEvent(false);
                this.info = info;
            }
        }
        public struct StateInfo
        {
            public DbAttachments[] attachments;
            public VmStakeholderComplaintListing listing;
            public AutoResetEvent reset;
            public StateInfo(DbAttachments[] attachments, VmStakeholderComplaintListing listing)
            {
                this.listing = listing;
                this.attachments = attachments;
                this.reset = null;
            }
            public StateInfo(DbAttachments[] attachments, VmStakeholderComplaintListing listing,AutoResetEvent reset)
            {
                this.listing = listing;
                this.attachments = attachments;
                this.reset = reset;
            }
        }
        public static void SetPictureData(List<VmStakeholderComplaintListing> complaints,int val = 1)
        {
            try
            {
                ComplaintsCount = complaints.Count;
                lstcomplaints = complaints;
                CallbackCounter = 0;
                offset = 0;
                switch (val)
                {
                    case 1:
                        GetSequentialData(complaints);
                        break;
                    case 2:
                        GetSynchronousDelegateData(complaints);
                        break;
                    case 3:
                        GetAsynchronousDelegateData(complaints);
                        break;
                    case 4:
                        GetMultiThreadedData(complaints);
                        break;
                    case 5:
                        GetThreadPoolData(complaints);
                        break;
                    case 6:
                        GetTaskBasedData(complaints);
                        break;
                    case 7:
                        GetDataParallelismBasedData(complaints);
                        break;
                    case 8:
                        GetAwaitAsyncBasedData(complaints);
                        break;
                    case 9:
                        GetStepWaitAsyncDelegateData();
                        break;
                    default:
                        GetSequentialData(complaints);
                        break;
                }
                //GetSequentialData(complaints);
                //GetSynchronousDelegateData(complaints);
                //GetAsynchronousDelegateData(complaints);
                //GetMultiThreadedData(complaints);
                //GetThreadPoolData(complaints);
                //GetTaskBasedData(complaints);
                //GetDataParallelismBasedData(complaints);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private delegate void PictureDataDelegate(StateInfo info);
        private delegate void AsynchronousDelegatePictureData(object info);
        private static void GetAwaitAsyncBasedData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                StateInfo stateInfo;
                for (int i = 0; i < complaints.Count; i++)
                {
                    int Id = complaints[i].ComplaintId;
                    DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                    DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                    for (int j = 0; j < attachments.Count(); j++)
                    {
                        custom_attachment[j] = new DbAttachments();
                        custom_attachment[j].ReferenceType = attachments[j].ReferenceType;
                        custom_attachment[j].Source_Url = attachments[j].Source_Url;
                        custom_attachment[j].FileExtension = attachments[j].FileExtension;
                    }
                    PictureDataDelegate myFunction = new PictureDataDelegate(GetDelegatePictureData);

                    stateInfo = new StateInfo(custom_attachment, complaints[i]);
                     myFunction(stateInfo);

                }
                Debug.WriteLine("Exiting Picture Function");
            }
        }
        private static void GetSynchronousDelegateData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                StateInfo stateInfo;
                for (int i = 0; i < complaints.Count; i++)
                {
                    int Id = complaints[i].ComplaintId;
                    DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                    DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                    for (int j = 0; j < attachments.Count(); j++)
                    {
                        custom_attachment[j] = new DbAttachments();
                        custom_attachment[j].ReferenceType = attachments[j].ReferenceType;
                        custom_attachment[j].Source_Url = attachments[j].Source_Url;
                        custom_attachment[j].FileExtension = attachments[j].FileExtension;
                    }
                    PictureDataDelegate myFunction = new PictureDataDelegate(GetDelegatePictureData);
                    
                    stateInfo = new StateInfo(custom_attachment, complaints[i]);
                    myFunction(stateInfo);
                    
                }
                Debug.WriteLine("Exiting Picture Function");
            }
        }
        private const int ThreadCount = 1;
        private static int offset = 0;
        private static int ComplaintsCount = 0;
        private static int CallbackCounter = 0;
        private static List<VmStakeholderComplaintListing> lstcomplaints = null;

        private static void GetStepWaitAsyncDelegateData()
        {
            while (offset < ComplaintsCount)
            {
                InitaiteThread();
            }
        }
        private static void InitaiteThread()
        {
            try
            {
                threadCompleteCount = 0;
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    int q = 0;
                    StateInfo[] my_states = null;
                    int j = 0;
                    int m = 0;
                    j = offset;
                    if (offset < ComplaintsCount)
                    {
                        if (offset + ThreadCount <= ComplaintsCount)
                        {
                            q = offset + ThreadCount;
                        }
                        else
                        {
                            q = ComplaintsCount;
                        }
                        my_states = new StateInfo[q - j];
                        CallbackCounter = q - j;
                        IAsyncResult[] ar = new IAsyncResult[q - j];
                        AutoResetEvent[] wh = new AutoResetEvent[q - j];
                        Thread[] th = new Thread[q - j];
                        if (j <= ComplaintsCount)
                        {
                            for (m = 0; j < q; j++, m++)
                            {
                                int Id = lstcomplaints[j].ComplaintId;
                                DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                                DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                                for (int k = 0; k < attachments.Count(); k++)
                                {
                                    custom_attachment[k] = new DbAttachments();
                                    custom_attachment[k].ReferenceType = attachments[k].ReferenceType;
                                    custom_attachment[k].Source_Url = attachments[k].Source_Url;
                                    custom_attachment[k].FileExtension = attachments[k].FileExtension;
                                }
                                wh[m] = new AutoResetEvent(false);
                                if (custom_attachment != null && lstcomplaints[j] != null)
                                {
                                    my_states[m] = new StateInfo(custom_attachment, lstcomplaints[j],wh[m]);
                                }
                                else
                                {
                                    my_states[m] = new StateInfo();
                                }
                                AsynchronousDelegatePictureData myFunction = new AsynchronousDelegatePictureData(GetStepWaitAsyncDelegatePictureData);
                                th[m] = new Thread(new ParameterizedThreadStart(GetStepWaitAsyncDelegatePictureData));
                                th[m].Start(my_states[m]);
                                
                                //ar[m] = myFunction.BeginInvoke(my_states[m], new AsyncCallback(ThreadCallCompleted), null);
                                //wh[m] = ar[m].AsyncWaitHandle;
                            }
                            WaitHandle.WaitAll(wh);
                        }
                    }
                }
            }catch(Exception ex){
                Debug.WriteLine(ex.Message);
            }
        }

        static int threadCompleteCount = 0;
        private static void ThreadCallCompleted(IAsyncResult ar)
        {
            Debug.WriteLine("DataCompleted() invoked on thread {0}", Thread.CurrentThread.ManagedThreadId);
            AsyncResult res = (AsyncResult)ar;
            AsynchronousDelegatePictureData del = (AsynchronousDelegatePictureData)res.AsyncDelegate;
            del.EndInvoke(ar);
            threadCompleteCount++;
            //int value = Interlocked.Decrement(ref CallbackCounter);
            //if (value == 0 && offset<=ComplaintsCount)
            if(threadCompleteCount==ThreadCount)
            {
                //Monitor.Enter(MonitorLock);
                offset = offset + ThreadCount;
                //Monitor.Exit(MonitorLock);
                InitaiteThread();
            }
            
        }
        private static void GetAsynchronousDelegateData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                int threadCounter = (int)Math.Ceiling(complaints.Count / 63d);
                IAsyncResult[] ar = new IAsyncResult[63];
                WaitHandle[] wh = new WaitHandle[63];  
                int loopCounter = 0;
                for (int i = 0; i < 63; i++)
                {
                    StateInfo[] my_states = null;
                    int q = 0;
                    int j = 0;
                    int m = 0;
                    if (loopCounter + threadCounter <= complaints.Count)
                    {
                        q = loopCounter + threadCounter;
                        my_states = new StateInfo[threadCounter];
                    }
                    else
                    {
                        q = complaints.Count;
                        my_states = new StateInfo[loopCounter + threadCounter - complaints.Count];
                    }
                    j = loopCounter;
                    if (j <= complaints.Count)
                    {
                        for (m = 0; j < q; j++, m++)
                        {
                            int Id = complaints[j].ComplaintId;
                            DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                            DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                            for (int k = 0; k < attachments.Count(); k++)
                            {
                                custom_attachment[k] = new DbAttachments();
                                custom_attachment[k].ReferenceType = attachments[k].ReferenceType;
                                custom_attachment[k].Source_Url = attachments[k].Source_Url;
                                custom_attachment[k].FileExtension = attachments[k].FileExtension;
                            }
                            if (custom_attachment != null && complaints[j] != null)
                            {
                                my_states[m] = new StateInfo(custom_attachment, complaints[j]);
                            }
                            else
                            {
                                my_states[m] = new StateInfo();
                            }
                        }
                    }
                    StateInfoArray arr = new StateInfoArray(my_states);
                    loopCounter += threadCounter;
                    AsynchronousDelegatePictureData myFunction = new AsynchronousDelegatePictureData(GetThreadPoolPictureData);
                    ar[i] = myFunction.BeginInvoke(arr, new AsyncCallback(DataCompleted), null);
                    wh[i] = ar[i].AsyncWaitHandle;
                }
                WaitHandle.WaitAll(wh);
                Debug.WriteLine("Exiting Picture Function");
            }
        }
        private static void GetMultiThreadedData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                StateInfo stateInfo;
                Thread[] tArr = new Thread[complaints.Count];
                int threadCounter = (int)Math.Ceiling(complaints.Count / 63d);
                AutoResetEvent[] waitHandles = new AutoResetEvent[63];
                int loopCounter = 0;
                for (int i = 0; i < 63; i++)
                {
                    StateInfo[] my_states = null;
                    int q = 0;
                    int j = 0;
                    int m = 0;
                    if (loopCounter + threadCounter <= complaints.Count)
                    {
                        q = loopCounter + threadCounter;
                        my_states = new StateInfo[threadCounter];
                    }
                    else
                    {
                        q = complaints.Count;
                        my_states = new StateInfo[loopCounter + threadCounter - complaints.Count];
                    }
                    j = loopCounter;
                    if (j <= complaints.Count)
                    {
                        for (m = 0; j < q; j++, m++)
                        {
                            int Id = complaints[j].ComplaintId;
                            DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                            DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                            for (int k = 0; k < attachments.Count(); k++)
                            {
                                custom_attachment[k] = new DbAttachments();
                                custom_attachment[k].ReferenceType = attachments[k].ReferenceType;
                                custom_attachment[k].Source_Url = attachments[k].Source_Url;
                                custom_attachment[k].FileExtension = attachments[k].FileExtension;
                            }
                            if (custom_attachment != null && complaints[j] != null)
                            {
                                my_states[m] = new StateInfo(custom_attachment, complaints[j]);
                            }
                            else
                            {
                                my_states[m] = new StateInfo();
                            }
                        }
                    }
                    waitHandles[i] = new AutoResetEvent(false);
                    StateInfoArray arr = new StateInfoArray(my_states, waitHandles[i]);
                    loopCounter += threadCounter;
                    Thread t = new Thread(new ThreadStart(PrintThreadId));
                    t.IsBackground = true;
                    t.Name = "Name" + i;
                    t.Start();

                    tArr[i] = new Thread(new ParameterizedThreadStart(GetThreadPoolPictureData));
                    tArr[i].Start(arr);
                }
                WaitHandle.WaitAll(waitHandles);
                Debug.WriteLine("Exiting Picture Function");
            }
        }
        private static void PrintThreadId()
        {
            Debug.WriteLine("Thread Id: {0}", Thread.CurrentThread.ManagedThreadId);
        }
        private static bool isDone = false;
        private static void DataCompleted(IAsyncResult ar)
        {
            Debug.WriteLine("DataCompleted() invoked on thread {0}", Thread.CurrentThread.ManagedThreadId);
            AsyncResult res = (AsyncResult)ar;
            //StateInfo obj = (StateInfo)ar.AsyncState;
            PictureDataDelegate del = (PictureDataDelegate)res.AsyncDelegate;
            del.EndInvoke(ar);
            isDone = true;
        }
        private static void GetDelegatePictureData(StateInfo info)
        {
            Debug.WriteLine("Data() invoked on thread {0}", Thread.CurrentThread.ManagedThreadId);
            StateInfo data = (StateInfo)info;
            foreach (DbAttachments row in data.attachments)
            {
                if (row.FileExtension == ".jpg" || row.FileExtension == ".jpeg" || row.FileExtension == ".bmp" || row.FileExtension == ".png")
                {
                    if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                    {
                        data.listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                    else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                    {
                        data.listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                }
            }
        }
        private static void GetSequentialData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                StateInfo stateInfo;
                for (int i = 0; i < complaints.Count; i++)
                {
                    int Id = complaints[i].ComplaintId;
                    DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                    DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                    for (int j = 0; j < attachments.Count(); j++)
                    {
                        custom_attachment[j] = new DbAttachments();
                        custom_attachment[j].ReferenceType = attachments[j].ReferenceType;
                        custom_attachment[j].Source_Url = attachments[j].Source_Url;
                        custom_attachment[j].FileExtension = attachments[j].FileExtension;
                    }
                    stateInfo = new StateInfo(custom_attachment, complaints[i]);
                    GetSequentialPictureData(stateInfo);
                }
                Debug.WriteLine("Exiting Picture Function");
            }
        }

        private static object MonitorLock = new object();
        private static void GetDataParallelismBasedData(List<VmStakeholderComplaintListing> complaints)
        {
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                Parallel.For(0, complaints.Count, options, (i) =>
                {
                    int Id = complaints[i].ComplaintId;
                    Monitor.Enter(MonitorLock);
                    DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                    DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                    for (int j = 0; j < attachments.Count(); j++)
                    {
                        custom_attachment[j] = new DbAttachments();
                        custom_attachment[j].ReferenceType = attachments[j].ReferenceType;
                        custom_attachment[j].Source_Url = attachments[j].Source_Url;
                        custom_attachment[j].FileExtension = attachments[j].FileExtension;
                    }
                    Monitor.Exit(MonitorLock);
                    StateInfo stateInfo;
                    stateInfo = new StateInfo(custom_attachment, complaints[i]);
                    GetParallelPictureData(stateInfo);
                });
            } 
        }
        private static void GetThreadPoolData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                int threadCounter = (int)Math.Ceiling(complaints.Count / 63d);
                ManualResetEvent[] resetEvents = new ManualResetEvent[63];
                for (int z = 0; z < 63; z++)
                {
                    resetEvents[z] = new ManualResetEvent(false);
                }
                int loopCounter = 0;
                for (int i = 0; i < 63; i++)
                {
                    StateInfo[] my_states = null;
                    int q = 0;
                    int j = 0;
                    int m = 0;
                    if (loopCounter + threadCounter <= complaints.Count)
                    {
                        q = loopCounter + threadCounter;
                        my_states = new StateInfo[threadCounter];
                    }
                    else
                    {
                        q = complaints.Count;
                        my_states = new StateInfo[loopCounter + threadCounter - complaints.Count];
                    }
                    j = loopCounter;
                    if (j <= complaints.Count)
                    {
                        for (m = 0; j < q; j++, m++)
                        {
                            int Id = complaints[j].ComplaintId;
                            DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                            DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                            for (int k = 0; k < attachments.Count(); k++)
                            {
                                custom_attachment[k] = new DbAttachments();
                                custom_attachment[k].ReferenceType = attachments[k].ReferenceType;
                                custom_attachment[k].Source_Url = attachments[k].Source_Url;
                                custom_attachment[k].FileExtension = attachments[k].FileExtension;
                            }
                            if (custom_attachment != null && complaints[j] != null)
                            {
                                my_states[m] = new StateInfo(custom_attachment, complaints[j]);
                            }
                            else
                            {
                                my_states[m] = new StateInfo();
                            }
                        }
                    }
                    StateInfoArray arr = new StateInfoArray(my_states, resetEvents[i]);
                    loopCounter += threadCounter;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(GetThreadPoolPictureData), arr);
                }
                WaitHandle.WaitAll(resetEvents);
                Debug.WriteLine("Exiting Picture Function");
            }
        }
        private static void GetTaskBasedData(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                StateInfo stateInfo;
                Task[] tasks = new Task[complaints.Count];
                for (int i = 0; i < complaints.Count; i++)
                {

                    int Id = complaints[i].ComplaintId;
                    DbAttachments[] attachments = db.DbAttachments.Where(x => x.Complaint_Id == Id).ToArray<DbAttachments>();
                    DbAttachments[] custom_attachment = new DbAttachments[attachments.Count()];
                    for (int j = 0; j < attachments.Count(); j++)
                    {
                        custom_attachment[j] = new DbAttachments();
                        custom_attachment[j].ReferenceType = attachments[j].ReferenceType;
                        custom_attachment[j].Source_Url = attachments[j].Source_Url;
                        custom_attachment[j].FileExtension = attachments[j].FileExtension;
                    }

                    stateInfo = new StateInfo(custom_attachment, complaints[i]);

                    tasks[i] = Task.Factory.StartNew(GetTaskBasedPictureData, stateInfo,TaskCreationOptions.None);
                }
                Task.WaitAll(tasks);
                Debug.WriteLine("Exiting Picture Function");
            }
        }
        private static object threadlock = new object();
        private static void GetSequentialPictureData(object info)
        {
            StateInfo data = (StateInfo)info;
            foreach (DbAttachments row in data.attachments)
            {
                if (row.FileExtension == ".jpg" || row.FileExtension == ".jpeg" || row.FileExtension == ".bmp" || row.FileExtension == ".png")
                {
                    if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                    {
                        data.listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                    else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                    {
                        data.listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                }
            }
            if (data.reset != null)
                data.reset.Set();
        }
        private static void GetThreadPoolPictureData(object info)
        {
            StateInfoArray data = (StateInfoArray)info;
            for (int i = 0; i < data.info.Length; i++)
            {
                if (data.info[i].listing != null && data.info[i].attachments != null)
                {
                    foreach (DbAttachments row in data.info[i].attachments)
                    {
                        if (row.FileExtension == ".jpg" || row.FileExtension == ".jpeg" || row.FileExtension == ".bmp" || row.FileExtension == ".png")
                        {

                            if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                            {
                                data.info[i].listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                            }
                            else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                            {
                                data.info[i].listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                            }
                        }
                    }
                }
            }
            data.manualEvent.Set();
        }

        private static void GetStepWaitAsyncDelegatePictureData(object info)
        {
            StateInfo data = (StateInfo)info;
            foreach (DbAttachments row in data.attachments)
            {
                if (row.FileExtension == ".jpg" || row.FileExtension == ".jpeg" || row.FileExtension == ".bmp" || row.FileExtension == ".png")
                {
                    if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                    {
                        data.listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                    else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                    {
                        data.listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                }
            }
            if (data.reset != null)
                data.reset.Set();
            //threadCompleteCount++;
            int value = Interlocked.Increment(ref threadCompleteCount);
            if (threadCompleteCount == ThreadCount)
            {
                Interlocked.Add(ref offset, ThreadCount);// offset = offset + ThreadCount;
                InitaiteThread();
            }

        }
        private static void GetTaskBasedPictureData(object info)
        {
            StateInfo data = (StateInfo)info;
            foreach (DbAttachments row in data.attachments)
            {
                if (row.FileExtension == ".jpg" || row.FileExtension == ".jpeg" || row.FileExtension == ".bmp" || row.FileExtension == ".png")
                {
                    if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                    {
                        data.listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                    else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                    {
                        data.listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                }
            }
        }

        private static void GetParallelPictureData(object info)
        {
            StateInfo data = (StateInfo)info;
            Parallel.ForEach(data.attachments, row =>
            {
                if (row.FileExtension == ".jpg" || row.FileExtension == ".jpeg" || row.FileExtension == ".bmp" || row.FileExtension == ".png")
                {
                    if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                    {
                        data.listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                    else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                    {
                        data.listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                    }
                }
            });
            if (data.reset != null)
                data.reset.Set();
        }
        public static void GetComplaintAssignedToUser(List<VmStakeholderComplaintListing> complaints)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                List<DbUsers> users = db.DbUsers.Where(x => x.Campaign_Id == (int)Config.Campaign.FixItLwmc).ToList<DbUsers>();

                foreach (VmStakeholderComplaintListing listing in complaints)
                {
                    List<DbUsers> listCurrentDbUsers = UsersHandler.GetUsersPresentForCurrentHierarchy2(users,
                     (Config.Hierarchy)listing.Complaint_Computed_Hierarchy_Id,
                     GetHierarchyVal((Config.Hierarchy)listing.Complaint_Computed_Hierarchy_Id, listing),
                     listing.Complaint_Computed_User_Hierarchy_Id,
                     listing.userCategoryId1,
                     listing.userCategoryId2);
                    if (listCurrentDbUsers != null && listCurrentDbUsers.Count > 0)
                        listing.Complaint_Stakeholder_Name = listCurrentDbUsers.FirstOrDefault().Name.Trim();
                }
            }
        }
        public static int? GetHierarchyVal(Config.Hierarchy hierarchyId, VmStakeholderComplaintListing listing)
        {
            int hierarchyVal = 0;
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    hierarchyVal = listing.Province_Id;
                    break;

                case Config.Hierarchy.Division:
                    hierarchyVal = listing.Division_Id;
                    break;

                case Config.Hierarchy.UnionCouncil:
                    hierarchyVal = listing.UnionCouncil_Id;
                    break;

                case Config.Hierarchy.District:
                    hierarchyVal = listing.District_Id;
                    break;

                case Config.Hierarchy.Tehsil:
                    hierarchyVal = listing.Tehsil_Id;
                    break;
                case Config.Hierarchy.Ward:
                    hierarchyVal = listing.Ward_Id;
                    break;
            }
            return hierarchyVal;
        }
        public static ListingParamsModelBase SetStakeholderListingParams(DbUsers dbUser, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = "complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status,complaints.RefField1 as RefField1,Latitude,Longitude,LocationArea,Computed_Remaining_Total_Time, CNFP_Feedback_Id,CNFP_Feedback_Value, CNFP_Feedback_Ref_Id,CNFP_Is_FeedbackGiven,CNFP_Feedback_Comments,complaints.Computed_Overdue_Days,CASE when complaints.StatusReopenedCount IS null THEN 0 ELSE  StatusReopenedCount end StatusReopenedCount,";



            //CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsModelBase paramsModel = new ListingParamsModelBase();


            if (spType == "ExcelReport")
            {
                extraSelection = @"CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10)) AS [Complaint_No],
					 Computed_Remaining_Total_Time,
                     complaints.Id As [ComplaintId], 
					 C.Person_Name as [Person_Name],
					 C.Cnic_No as [Cnic No],
					 CASE C.Gender WHEN 1 THEN 'MALE' ELSE 'FEMALE' END AS Gender,
					 complaints.Person_District_Name as [Caller District],
					 CONVERT(VARCHAR(10),complaints.Created_Date,120) Date,
					 C.Mobile_No as [Mobile No],
					 C.Person_Address as [Person Address],
                     complaints.UserCategoryId1, 
                     complaints.UserCategoryId2, 
                     complaints.Complaint_Computed_Status as [Complaint_Computed_Status],
                     complaints.Complaint_Computed_Status_Id as [Complaint_Computed_Status_Id],
                     complaints.Complaint_Computed_Hierarchy_Id, 
                     complaints.Complaint_Computed_User_Hierarchy_Id, 
                     complaints.FollowupCount,

                    Pr.Id as [Province_Id],Pr.Province_Name as [Province_Name],
                    Div.Id as [Division_Id],Div.Division_Name as [Division_Name],
                    D.id as [District_Id], D.District_Name as [District_Name],					
					t.Id as [Tehsil_Id], t.Tehsil_Name as [Tehsil_Name],
                    u.UcNo AS [Uc_No],
                    u.Id as [UnionCouncil_Id], u.Councils_Name as [UnionCouncil_Name],
                    
					B.Name Category  ,
					F.Name as [Sub Category],
					complaints.Complaint_Remarks as [Complaint Remarks],
					complaints.Agent_Comments as [Details],
					P.[Status],
					complaints.Complaint_Computed_Hierarchy as [Escalation_Level],
					complaints.Created_Date as [Created_Date],
                    complaints.Created_Date as [Created Date],
                    complaints.StatusChangedDate_Time As [Status_Changed_Date]";
                    
                paramsModel.InnerJoinLogic = @"INNER JOIN PITB.Complaints_Type B ON complaints.Complaint_Category=B.Id
					INNER JOIN PITB.Complaints_SubType F ON complaints.Complaint_SubCategory=F.Id
					INNER JOIN PITB.Person_Information C ON complaints.Person_Id=C.Person_id
                    INNER JOIN PITB.Provinces Pr ON complaints.Province_Id=Pr.id
                    INNER JOIN PITB.Divisions Div ON complaints.Division_Id=Div.id					
                    INNER JOIN PITB.Districts D ON complaints.District_Id=D.id
					INNER JOIN PITB.Tehsil T ON complaints.Tehsil_Id=t.Id
                    INNER JOIN PITB.Union_Councils U ON complaints.UnionCouncil_Id = U.Id
					INNER JOIN PITB.Statuses P ON p.Id=complaints.Complaint_Computed_Status_Id";
            }


            if (dtParams != null)
            {
                paramsModel.StartRow = dtParams.Start;
                paramsModel.EndRow = dtParams.End;
                paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
                paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
                paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;
            }
            paramsModel.From = fromDate;
            paramsModel.To = toDate;
            paramsModel.Campaign = campaign;
            paramsModel.Category = category;
            paramsModel.Status = complaintStatuses;
            paramsModel.TransferedStatus = commaSeperatedTransferedStatus;
            paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
            paramsModel.UserHierarchyId = Convert.ToInt32(dbUser.Hierarchy_Id);
            paramsModel.UserDesignationHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);
            paramsModel.ListingType = Convert.ToInt32(listingType);
            paramsModel.ProvinceId = dbUser.Province_Id;
            paramsModel.DivisionId = dbUser.Division_Id;
            paramsModel.DistrictId = dbUser.District_Id;

            paramsModel.Tehsil = dbUser.Tehsil_Id;
            paramsModel.UcId = dbUser.UnionCouncil_Id;
            paramsModel.WardId = dbUser.Ward_Id;

            paramsModel.UserId = dbUser.User_Id;
            paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            paramsModel.ListUserCategory = UserCategoryModel.GetListUserCategoryModel(dbUser.ListDbUserCategory);
            paramsModel.CheckIfExistInSrcId = 0;
            paramsModel.CheckIfExistInUserSrcId = 0;
            paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;
            return paramsModel;
        }


    }
}