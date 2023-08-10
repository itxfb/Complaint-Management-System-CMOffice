
namespace PITB.CMS_Common.ApiModels.API
{
    public class IncomingSocialSubmitModel
    {
        public long ComplaintId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public string UserProvider { get; set; }
        public string CnicNo { get; set; }
        public string ContactNo { get; set; }
        public string PostId { get; set; }
        public Config.UserVote UserVote { get; set; }
        public Config.ExternalProvider ExternalProvider
        {
            get { return Utility.ParseEnum<Config.ExternalProvider>(this.UserProvider); }
        }
    }
}