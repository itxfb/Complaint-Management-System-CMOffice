using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Message
{
    public class VmMassMessageReply
    {
        [Required]
        public string ReplyMessageStr { get; set; }

        public string CommaSeperatedIds { get; set; }
    }
}