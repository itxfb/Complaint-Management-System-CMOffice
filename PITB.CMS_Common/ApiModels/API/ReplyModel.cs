﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class ReplyModel
    {
        public string CallerNo { get; set; }

        public string MsgTxt { get; set; }

        public DateTime CreaterDateTime { get; set; }

        public int GroupId { get; set; }
    }
}