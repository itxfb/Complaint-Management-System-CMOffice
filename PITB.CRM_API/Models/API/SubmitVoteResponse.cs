using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API
{
    public class SubmitVoteResponse : ApiStatus
    {
        public Config.UserVote UserVote { get; set; }
        public int TotalUpVotes { get; set; }
        public int TotalDownVotes { get; set; }
    }
}