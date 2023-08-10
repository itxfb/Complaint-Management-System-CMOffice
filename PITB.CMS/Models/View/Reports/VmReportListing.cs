namespace PITB.CMS.Models.View.Reports
{
    public class VmReportListing
    {
        public string From { get; set; }
        public string To { get; set; }
        public string[] Campaigns { get; set; }
        public string[] Agents { get; set; }
    }
}