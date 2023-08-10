using System.Collections.Generic;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.ApiModels.API
{
    public class GetComplainantComplaintModel : ApiStatus
    {
        public int TotalComplaints { get; set; }
        public List<ComplainantComplaintModel> ListComplaint { get; set; }
    }

    public class TempComplainatModel
    {
        public int TotalComplaints { get; set; }
        public List<DbComplaint> listDbComplaints { get;set; }
    }

    public class ComplainantComplaintModel
    {
       
        //public string imei_number { get; set; }
        //public List<LogHistory> ListLogHistory { get; set; }

        public LogHistory LastStatus { get; set; }

        public int complaintID { get; set; }
        public int campaignID { get; set; }

        public string campaignName { get; set; }

        public int departmentId { get; set; }
        public int provinceID { get; set; }

        public string provinceName { get; set; }

        public int districtID { get; set; }

        public string districtName { get; set; }

        public int tehsilID { get; set; }

        public string tehsilName { get; set; }

        public int ucID { get; set; }

        public string ucName { get; set; }

        public int wardID { get; set; }

        public int categoryID { get; set; }

        public string categoryName { get; set; }
        public string categoryNameUrdu { get; set; }


        public int subCategoryID { get; set; }

        public string subCategoryName { get; set; }
        public string subCategoryNameUrdu { get; set; }

        public string comment { get; set; }

        public string date { get; set; }

        //public TimeSpan time { get; set; }

        //public string lattitude { get; set; }

        //public string longitude { get; set; }


        // Person Information

        public string cnic { get; set; }

        public string personName { get; set; }

        public string personContactNumber { get; set; }

        public int statusId { get; set; }

        public string statusStr { get; set; }

        public List<Picture> ListPicturesComplaintsUrl { get; set; }

        public List<Video> ListVideoComplaintsUrl { get; set; }

        //public string video { get; set; }

        //public string videoFileExtension { get; set; }

        public List<Picture> ListPicturesStatusUrl { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public Config.UserVote UserVote { get; set; }
        public string ComplainantRemarks { get; set; }
        public int ComplainantSatisfactionStatus { get; set; }
        public string StakeHolderRemarks { get; set; }
        public string StakeHolderRemarksDate { get; set; }
        public double DistanceFromYourLocation { get; set; }
        public string LocationArea { get; set; }

        public string ComputedRemainingTotalTime { get; set; }
        private string _ExternalUserId { get; set; }
        [JsonIgnore]

        public string ExternalUserId
        {
            get { return (string.IsNullOrEmpty(_ExternalUserId)) ? string.Empty : _ExternalUserId.Trim(); }

            set { _ExternalUserId = value; }
        }
        [JsonIgnore]

        public Config.ExternalProvider ExternalProvider { get; set; }
        public string ProfilePhoto
        {
            get
            {
                string url = Config.AnonymousPictureOnlinePath;
                switch (ExternalProvider)
                {
                    case Config.ExternalProvider.Facebook:
                        url = string.Format("https://graph.facebook.com/{0}/picture?type=square", ExternalUserId);
                        break;
                    case Config.ExternalProvider.Google:
                        break;
                    case Config.ExternalProvider.Twitter:
                        break;
                }
                return url;
            }
        }

    }


}