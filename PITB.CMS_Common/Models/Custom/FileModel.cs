using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.Custom
{
    public class FileModel
    {
        public string Url { get; set; }

        public string OrignalFileName { get; set; }
        public string Extension { get; set; }

        public string ContentType { get; set; }

        public bool IsViewable { get; set; }

        public bool IsDownloadable { get; set; }
    }
}