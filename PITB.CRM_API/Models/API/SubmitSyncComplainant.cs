﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class SubmitSyncComplainant
    {
        public string Cnic { get; set; }
        public string MobileNo { get; set; }

        public string ImeiNo { get; set; }
    }

    public class LwmcSubmitSyncComplainant
    {
        public string Cnic { get; set; }
        public string ImeiNo { get; set; }

        
    }
}