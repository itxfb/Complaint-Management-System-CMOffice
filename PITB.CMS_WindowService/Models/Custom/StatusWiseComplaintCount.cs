using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_WindowService.Models.Custom
{
    public class StatusWiseComplaintCount
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; }

        public int ComplaintCount { get; set; }
    }
}
