using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.View
{
    public class VmComplaintTypeWiseData
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

        public string GetColorCode(int complaintId)
        {
            switch (complaintId)
            {
                case 0:
                    return "#00AA00";
                case 1:
                    return "#FFC000";
                case 2:
                    return "#92D050";
                case 3:
                    return "#00B050";
                case 4:
                    return "#33672C";
                case 5:
                    return "#293A56";
                case 6:
                    return "#C00000";
                case 7:
                    return "#FF6600";
                case 8:
                    return "#DB9100";
                case 9:
                    return "#D42C39";
                case 10:
                    return "#916331";
                case 11:
                    return "#37424A";
                case 12:
                    return "#92d050";
                case 13:
                    return "#f60";
                case 14:
                    return "#054913";
                case 15:
                    return "#00AA00";
                default:
                    return "";
            }
        }
        public string GetComplaintTypeName(int complaintId)
        {
            switch (complaintId)
            {
                case 0:
                    return "None";
                case 1:
                    return "Complaint";
                case 2:
                    return "Suggestion";
                case 3:
                    return "Inquiry";                
                default:
                    return "none";
            }
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

}