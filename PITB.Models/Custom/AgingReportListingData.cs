using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Models.Custom
{
    public class AgingReportListingData
    {

        public int ComplaintId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime MaxDateTime { get; set; }

        public DateTime ResolvedDateTime { get; set; }

        public int StatusId { get; set; }

        public float ResolvedTimePercentage
        {
            get
            {
                long resolvedTicks = (ResolvedDateTime - CreatedDateTime).Ticks;
                long maxTicks = (MaxDateTime - CreatedDateTime).Ticks;
                long ticksSpan = maxTicks - resolvedTicks;
                
                float percentage = (resolvedTicks / maxTicks) * 100;
                if (ticksSpan > 0) // if resolved in time
                {
                    percentage = Math.Abs(((float)(MaxDateTime-ResolvedDateTime).Ticks / maxTicks) * 100);
                }
                else
                {
                    percentage = -Math.Abs(((float)(resolvedTicks - maxTicks) / maxTicks) * 100);
                }
                return percentage;
            }
        }

    }
}