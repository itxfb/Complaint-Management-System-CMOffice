using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace PITB.CMS_WindowService.Models.Custom
{
    public class SmsOverDueModel
    {
        public string UserName { get; set; }
        public string Date     { get; set; }

        public string HelplineName { get; set; }

        public string OverdueComplaintRegion { get; set; }

        public string TotalOverdue { get; set; }

        public string HighestOverdue { get; set; }

        public string LowestOverdue { get; set; }

        public List<string> ListRegionOverdue { get; set; }

        public string Region1 { get; set; }

        public string Region2 { get; set; }

        public string Overdue90PlusDays { get; set; }

        public string Overdue90Days { get; set; }

        public string Overdue30Days { get; set; }

        public string Overdue7Days { get; set; }

        public string TopSubCatHeading { get; set; }
        public List<string> ListTopSubcategories { get; set; }

        public string ResolutionReminder { get; set; }

        public string MsgFooter { get; set; }

        //public string TopSubcategory1 { get; set; }

        //public string TopSubcategory2 { get; set; }

        //public string TopSubcategory3 { get; set; }

//        M-Gov Punjab
//Date: “Jan 18, 2018”
//Education Helpline
//Overdue Complaints: Lahore District <District wise>
//Total Overdue (number)
//Highest Overdue Complaints (Elementary Male) <Wings Name>
//Lowest Overdue Complaints (Secondary Wing)
//Wing 1 (number)
//Wing 2 (number)
//…
//…
//<space>
//Overdue (90 days): number
//Overdue (30 days): number 
//Overdue (7 days): number
//<space>
//Top 3 sub-categories: <in descending order>
//Sub-category 1 (number)
//…
//…

    }
}
