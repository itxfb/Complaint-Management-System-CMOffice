using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class BaseMobileApiModel
    {
        public string lattitude { get; set; }

        public string longitude { get; set; }

        public string imei_number { get; set; }

        public DateTime Created_DateTime { get; set; }
    }
}