﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class SubmitStakeHolderLogin
    {
        public string Cnic { get; set; }
        public string Username { get; set; }
        public string ImeiNo { get; set; }

        public string fcm_key { get; set; }
    }
}