using PITB.CRM_API.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class GetSchoolEducationComplaintModel : ApiStatus
    {
        public int TotalComplaints { get; set; }
        public List<SchoolModelWrapper> ListComplaint { get; set; }
    }

    public class SchoolModelWrapper
    {
        public string emisCode { get; set; }

        public int totalComplaints { get; set; }

        public List<SchoolComplaintModel> listSchoolComplaintModel { get; set; }

        public SchoolModelWrapper()
        {
            this.emisCode = null;
            this.totalComplaints = 0;
            this.listSchoolComplaintModel = new List<SchoolComplaintModel>();
        }


        //public int complaintID { get; set; }
        //public int campaignID { get; set; }

        //public string campaignName { get; set; }

        //public int departmentId { get; set; }
        //public int provinceID { get; set; }

        //public string provinceName { get; set; }

        //public int districtID { get; set; }

        //public string districtName { get; set; }

        //public int tehsilID { get; set; }

        //public string tehsilName { get; set; }

        //public int ucID { get; set; }

        //public string ucName { get; set; }

        //public int wardID { get; set; }

        //public int categoryID { get; set; }

        //public string categoryName { get; set; }

        //public int subCategoryID { get; set; }

        //public string subCategoryName { get; set; }

        //public string comment { get; set; }

        //public string date { get; set; }

        ////public TimeSpan time { get; set; }

        ////public string lattitude { get; set; }

        ////public string longitude { get; set; }


        //// Person Information

        //public string cnic { get; set; }

        //public string personName { get; set; }

        //public string personContactNumber { get; set; }

        //public int statusId { get; set; }

        //public string statusStr { get; set; }

        //public string statusChangeRemarks { get; set; }

        //public List<Picture> ListPicturesComplaintsUrl { get; set; }

        //public List<Video> ListVideoComplaintsUrl { get; set; }

        ////public string video { get; set; }

        ////public string videoFileExtension { get; set; }

        //public List<Picture> ListPicturesStatusUrl { get; set; }

        //public double? Latitude { get; set; }
        //public double? Longitude { get; set; }

        ////public int UpVotes { get; set; }
        ////public int DownVotes { get; set; }
        //// public Config.UserVote UserVote { get; set; }
        //public string ComplainantRemarks { get; set; }
        //public int ComplainantSatisfactionStatus { get; set; }
        ////public string StakeHolderRemarks { get; set; }
        ////public string StakeHolderRemarksDate { get; set; }

    }

    public class SchoolComplaintModel
    {
        //public string imei_number { get; set; }

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

        public int subCategoryID { get; set; }

        public string subCategoryName { get; set; }

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

        public string statusChangeRemarks { get; set; }

        public string statusChangeDateTime { get; set; }

        public List<Picture> ListPicturesComplaintsUrl { get; set; }

        public List<Video> ListVideoComplaintsUrl { get; set; }

        //public string video { get; set; }

        //public string videoFileExtension { get; set; }

        public List<Picture> ListPicturesStatusUrl { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        //public int UpVotes { get; set; }
        //public int DownVotes { get; set; }
        // public Config.UserVote UserVote { get; set; }
        public string ComplainantRemarks { get; set; }
        public int ComplainantSatisfactionStatus { get; set; }
        //public string StakeHolderRemarks { get; set; }
        //public string StakeHolderRemarksDate { get; set; }
    }
}