using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PITB.CMS_Common.Helper;

namespace PITB.CMS_Common.Models.Public_Web.ViewModels
{
    public class VmNewsFeed
    {
        public VmNewsFeed()
        {
            ListComplaints = new List<ComplaintData>();
        }
        public int TotalComplaints
        {
            get { return (ListComplaints.Count); }
        }

        public List<ComplaintData> ListComplaints { get; set; }


    }

    public class ComplaintData
    {

        public ComplaintData()
        {
            SocialSharedData = new SocialShareData();
        }
        [JsonIgnore]
        public Int64 ComplaintId { get; set; }

        [JsonIgnore]
        public int CampaignId { get; set; }

        public string ComplaintHash
        {
            get { return Utility.Encrypt(ComplaintId); }
        }

        public Int64 DecryptedComplaintId
        {
            get { return Utility.Decrypt(ComplaintHash); }
        }

        public DateTime Created_DateTime { get; set; }

        public string TimeElapsed
        {
            get
            {
                TimeSpan diff = DateTime.Now - Created_DateTime;
                if (diff.TotalHours < 24)
                {
                    return string.Format("{0} ago",Utility.CalculateTurnRoundTime(TimeSpan.FromTicks(diff.Ticks)));
                }
                else
                {
                    return Created_DateTime.ToString("dd MMM yyyy HH:mm");
                }
            }
        }

        public string Description { get; set; }
        public string Status { get
        {
            return ((Config.PublicComplaintStatus) StatusId).ToString().ToSentenceCase();
        }}
        public int StatusId { get; set; }
        public string StatusChangedText { get; set; }
        public DateTime StatusChangeDateTime { get; set; }
        public string StatusChangedByName { get; set; }
        public List<string> StatusChangeAttachments  { get; set; }
        public string Category { get; set; }
        public List<string> ImageList { get; set; }
        public Location Location { get; set; }
        public string NearestLocation { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        public string UC { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public SocialShareData SocialSharedData { get; set; }
    }

    public class SocialShareData
    {
        public string UserId { get; set; }

        public string FullName
        {
            get { return string.IsNullOrEmpty(FirstName) ? "Anonymous": FirstName + " " + LastName; }
        }

        [JsonIgnore]
        public string FirstName { get; set; }
        [JsonIgnore]
        public string LastName { get; set; }
        public string PostId { get; set; }
        public DateTime DateTime { get; set; }
        public string Provider { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        public string ProfilePhoto
        {
            get
            {
                string url = "https://static.xx.fbcdn.net/rsrc.php/v3/yo/r/UlIqmHJn-SK.gif";
                switch (ExternalProvider)
                {
                    case Config.ExternalProvider.Facebook:
                        url = string.Format("https://graph.facebook.com/{0}/picture?type=square", UserId);
                        break;
                    case Config.ExternalProvider.Google:
                        break;
                    case Config.ExternalProvider.Twitter:
                        break;
                }
                return url;
            }
        }

        public Config.ExternalProvider ExternalProvider
        {
            get
            {
                return (string.IsNullOrEmpty(Provider)
                    ? Config.ExternalProvider.None
                    : Utility.ParseEnum<Config.ExternalProvider>(Provider));
            }
        }
    }
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}