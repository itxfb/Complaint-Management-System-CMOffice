using PITB.CMS_Common.ApiModels.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.Health
{
    public class HealthRequestModel
    {
        public class SubmitComplaint
        {
            public string imei_number { get; set; }
            public int campaignID { get; set; }

            public int districtID { get; set; }

            public int tehsilID { get; set; }

            public int? departmentId { get; set; }

            public int categoryID { get; set; }

            public int subCategoryID { get; set; }

            // new fields
            public Config.ComplaintType complaintType { get; set; }

            public Config.HelplineSrc helplineSrc { get; set; }

            // end new fields

            public int statusId { get; set; }

            public string comment { get; set; }

            public Config.ComplaintSource complaintSrcId { get; set; }

            public float lattitude { get; set; }

            public float longitude { get; set; }
            public string remakrs { get; set; }

            // Person Information

            public string cnic { get; set; }

            public string personName { get; set; }

            public string personContactNumber { get; set; }

            public DateTime createdDateTime { get; set; }

            public List<Picture> PicturesList { get; set; }

            public int refComplaintId { get; set; }

        }

        public class SubmitStatus
        {
            //public string UserName { get; set; }
            public string complaintId { get; set; }

            public DateTime createdDateTime { get; set; }
            public int statusId { get; set; }
            public string statusComments { get; set; }

            public List<Picture> picturesList { get; set; }
        }
    }
}