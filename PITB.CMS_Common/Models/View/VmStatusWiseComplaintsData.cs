using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.EC2;

namespace PITB.CMS_Common.Models.View
{
    public class VmStatusWiseComplaintsData
    {
        public List<VmUserWiseStatus> ListUserWiseData { get; set; }
        public List<string> AllStatusList
        {
            get
            {
                return ListUserWiseData.First().ListVmStatusWiseCount.Select(m => m.StatusString).ToList();
            }
        }
        public List<string> AllStatusColorList
        {
            get
            {
                List<int> temp = ListUserWiseData.First().ListVmStatusWiseCount.Select(m => m.StatusId).ToList();
                List<string> tempColor = new List<string>();

                for (int i = 0; i < temp.Count; i++)
                {
                    tempColor.Add(GetColorCode(temp[i]));
                }
                return tempColor;
            }
        }

        public string GetColorCode(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    //return "#ffc000";
                    return "#FFC000";
                case 2:
                    //return "#92d050";
                    return "#92D050";
                case 3:
                    //return "#00b050";
                    return "#00B050";
                case 4:
                    return "#7FB3D5";
                case 5:
                    return "#99A3A4";
                case 6:
                    //return "#c00000";
                    return "#C00000";
                case 7:
                    //return "#ff6600";
                    return "#FF6600";
                case 8:
                    return "#414143";
                case 9:
                    return "#007ACC";
                case 10:
                    return "#68217A";
                case 11:
                    //return "#37424a";
                    return "#37424A";

                case 5837:
                    //return "#37424a";
                    return "#92d050";

                case 5838:
                    return "#f60";

                case 100:
                    //return "#37424a";
                    return "#054913";

                case 101:
                    
                    return "#00AA00";

                default:
                    return "";
            }

//            a.       Pending (Fresh) = #FFC000
//b.      Resolved (Unverified) = #92D050
//c.       Resolved (Verified) = #00B050
//d.      Pending (Overdue) = #C00000
//e.      Pending (Reopen) = #FF6600
//f.        Closed (Verified) = #37424A
        }
        public string GetStatusName(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Pending (Fresh)";
                case 2:
                    return "Resolved (Unverified)";
                case 3:
                    return "Resolved (Verified)";
                case 4:
                    return "Not Applicable";
                case 5:
                    return "Fake";
                case 6:
                    return "Pending (Overdue)";//"Unsatisfactory Closed";
                case 7:
                    return "Pending (Reopened)";
                case 8:
                    return "Resolved";
                case 9:
                    return "Not Applicable (Verified)";
                case 10:
                    return "Fake (Verified)";
                case 11:
                    return "Closed (Verified)";
                default:
                    return "none";
            }
            //1 Pending (Fresh)
            //2 Resolved (Unverified)
            //3 Resolved (Verified)
            //4 Not Applicable
            //5 Fake
            //6 Unsatisfactory Closed
            //7 Pending (Reopened)
            //8 Resolved
            //9 Not Applicable (Verified)
            //10 Fake (Verified)
            //11 Closed (Verified)
        }

        public static VmStatusWiseComplaintsData SortStatusWiseData(List<int> listStatusWiseData, VmStatusWiseComplaintsData vmStatusWiseComplaintData, bool isReverse)
        {
            List<int> listOrdered = listStatusWiseData.ToList();
            if (isReverse)
            {
                listOrdered.Reverse();
            }
            List<VmStatusCount> listVmStatusWiseCountOrdered = new List<VmStatusCount>();
            //List<VmStatusCount> listVmStatusWiseCountUnOrdered = new List<VmStatusCount>();
            VmStatusCount tempVmStatusCount = null;

            foreach (VmUserWiseStatus vmUserWiseStatus in vmStatusWiseComplaintData.ListUserWiseData)
            {
                listVmStatusWiseCountOrdered = new List<VmStatusCount>();
                foreach (int statusId in listOrdered)
                {
                    tempVmStatusCount = vmUserWiseStatus.ListVmStatusWiseCount.Where(n => n.StatusId == statusId).FirstOrDefault();
                    if (tempVmStatusCount != null)
                    {
                        listVmStatusWiseCountOrdered.Add(tempVmStatusCount);
                        vmUserWiseStatus.ListVmStatusWiseCount.Remove(tempVmStatusCount);
                    }
                }

                listVmStatusWiseCountOrdered = listVmStatusWiseCountOrdered.Concat(vmUserWiseStatus.ListVmStatusWiseCount).ToList();
 
                vmUserWiseStatus.ListVmStatusWiseCount = listVmStatusWiseCountOrdered;

            }
          
            return vmStatusWiseComplaintData;

        }

    }


    public class VmUserWiseStatus
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public List<VmStatusCount> ListVmStatusWiseCount { get; set; }

        public int TotalStatusWiseCount
        {
            get
            {
                int count = 0;
                foreach (VmStatusCount vmStatusCount in ListVmStatusWiseCount)
                {
                    count += vmStatusCount.Count;
                }
                return count;
            }
            
        }
    }

    public class VmStatusCount
    {
        public int StatusId { get; set; }

        public string StatusString { get; set; }

        public int Count { get; set; }

        public VmStatusCount()
        {

        }

        public VmStatusCount(VmStatusCount vmStatusCount)
        {
            this.StatusId = vmStatusCount.StatusId;
            this.StatusString = vmStatusCount.StatusString;
            this.Count = vmStatusCount.Count;
        }

        public VmStatusCount(int statusId, string statusString, int count)
        {
            this.StatusId = statusId;
            this.StatusString = statusString;
            this.Count = count;
        }
    }


}