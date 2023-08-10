using PITB.CMS_Common.ApiModels.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class SubmitStatusModel
    {
        public string UserName { get; set; }
        public string ComplaintId { get; set; }
        public int StatusId { get; set; }
        public string StatusComments { get; set; }

        public List<Picture> PicturesList { get; set; }
    }
}