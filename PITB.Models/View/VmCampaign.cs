namespace PITB.CMS_Models.View
{
    public class VmCampaign
    {
        public int Id { get; set; }

        public string Campaign_Name { get; set; }

        public int? Province_Id { get; set; }

        public string Campaign_HelpLine { get; set; }

        public int? District_Id { get; set; }

        public int? Campaign_Type { get; set; }
        public string LogoUrl { get; set; }
        public int PersonId { get; set; }
        public int CampaignId { get; set; }
    }
}