using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.ApiModels.API
{
    public class SubmitVoteResponse : ApiStatus
    {
        public Config.UserVote UserVote { get; set; }
        public int TotalUpVotes { get; set; }
        public int TotalDownVotes { get; set; }
    }
}