using System;

namespace PITB.CMS_Models.Custom
{
    public class DbToModel
    {
        public class CampaignStatusWiseCount
        {
            public int CampaignId { get; set; }

            public int StatusId { get; set; }

            public int Count { get; set; }
        }
    }
}