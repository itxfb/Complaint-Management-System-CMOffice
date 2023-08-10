namespace PITB.CMS_Common.Models.Public_Web.ViewModels
{
    public class VmSubmitVote
    {
        public string ComplaintHash { get; set; }
        public long ComplaintId { get { return (string.IsNullOrEmpty(ComplaintHash)) ? 0 : Utility.Decrypt(ComplaintHash); } }
        public Config.UserVote IsVoteUp { get { return (Config.UserVote) UserVote; }}
        public int UserVote { get; set; }


    }

    public class SubmiteVoteResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int UpVoteCount{ get; set; }
        public int DownVoteCount { get; set; }


    }
}