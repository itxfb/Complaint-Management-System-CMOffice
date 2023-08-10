﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class ComplaintsForStakeholderFacebookPost
    {
        public int ComplaintId { get; set; }
        public string AttachmentPath { get; set; }

        public string LoadedAttachment { get; set; }
        public string ComplaintStatus { get; set; }
        public void LoadAttachment()
        {
            LoadedAttachment = Utility.GetBase64FromUrl(AttachmentPath);
        }

        public bool IsAlreadyShared { get; set; }
        public string PostId { get; set; }
        public string StakeholderComments { get; set; }


    }
}