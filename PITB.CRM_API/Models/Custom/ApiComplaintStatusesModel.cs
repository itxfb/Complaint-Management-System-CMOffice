using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.Custom
{
    public class ApiComplaintStatusesPostedModel
    {
        public List<int> ListComplaintId { get; set; }
    }

    public class ApiComplaintStatusResponseModel : ApiStatus
    {
        public List<ApiComplaintStatusModel> ListApiStatusModel { get; set; }
    }

    public class ApiComplaintStatusModel
    {
        public int ComplaintId { get; set; }

        public int StatusId { get; set; }

        public string StatusMessage { get; set; }

        public List<Picture> ListPicturesUrl { get; set; }
    }
}