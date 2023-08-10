using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.View
{
    public class VmComplaintTypeWisePieChart
    {
        public string HierarchyId { get; set; }
        public List<VmComplaintTypeWiseCount> ListComplaintTypeWiseCount { get; set; }

        public List<string> AllStatusColorList
        {
            get
            {
                List<int> temp = ListComplaintTypeWiseCount.Select(m => m.Id).ToList();
                List<string> tempColor = new List<string>();

                for (int i = 0; i < temp.Count; i++)
                {
                    tempColor.Add(GetColorCode(i));
                }
                return tempColor;
            }
        }

        public string GetColorCode(int complaintId)
        {
            switch (complaintId)
            {
                case 0:
                    return "#4472C4";
                case 1:
                    return "#5B9BD5";
                case 2:
                    return "#ED7D31";
                case 3:
                    return "#A5A5A5";
                case 4:
                    return "#FFC000";
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
    }
    public class VmComplaintTypeWiseCount
    {
        public int Id { get; set; }
        public string name { get; set; }

        public double y { get; set; }
    }
}