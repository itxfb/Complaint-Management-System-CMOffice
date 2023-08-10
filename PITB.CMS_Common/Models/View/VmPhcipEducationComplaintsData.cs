using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models.View
{
    public class VmPhcipEducationComplaintsData
    {

        public VmPhcipEducationComplaintsData()
        {
            ComplaintsStats = new List<VmPhcipComplaintsStats>();
            ComplaintsSourceList = new List<VmPhcipComplaintsSource>();
            ComplaintsByDistrict = new List<VmPhcipComplaintsByDistrict>();
            ColorsList = new List<string>();
        }


        public List<VmPhcipComplaintsStats> ComplaintsStats { get; set; }
        public List<VmPhcipComplaintsByDistrict> ComplaintsByDistrict { get; set; }
        public List<VmPhcipComplaintsSource> ComplaintsSourceList { get; set; }
        public List<string> ColorsList { get; set; }



        [DisplayName("New")]
        public int TotalNew { get { return ComplaintsStats.Sum(s => s.New); } }
        [DisplayName("Resolved")]
        public int TotalResolved { get { return ComplaintsStats.Sum(s => s.Resolved); } }
        [DisplayName("Overdue")]
        public int TotalOverdue { get { return ComplaintsStats.Sum(s => s.Overdue); } }
        [DisplayName("Reopened")]
        public int TotalReopened { get { return ComplaintsStats.Sum(s => s.Reopened); } }
        [DisplayName("Closed")]
        public int TotalClosed { get { return ComplaintsStats.Sum(s => s.Closed); } }


        public string GetColorCode(string status)
        {
            switch (status)
            {
                case "Resolved":
                case "Hotline":
                    return "#3498db";
                case "New":
                case "Webportal":
                    return "#eb984e";
                case "Overdue":
                    return "#21618c";
                case "Reopened":
                    return "#f1c40f";
                case "Closed":
                    return "#95a5a6";
                default:
                    return "";
            }
        }
    }



    public class VmPhcipComplaintsByDistrict
    {
        public int Id { get; set; }
        public string District { get; set; }
        public int Count { get; set; }
    }
    public class VmPhcipComplaintsStats
    {
        public string Complaints_Category { get; set; }
        public int New { get; set; }
        public int Resolved { get; set; }
        public int Overdue { get; set; }
        public int Reopened { get; set; }
        public int Closed { get; set; }
    }
    public class VmPhcipComplaintsSource
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}
