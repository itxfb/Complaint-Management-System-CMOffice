namespace PITB.CMS_Common.Models.View.Reports
{
    public class VmReportListing
    {
        public string From { get; set; }
        public string To { get; set; }
        public string[] Campaigns { get; set; }
        public string[] Agents { get; set; }
        public string[] Departments { get; set; }
    }
}