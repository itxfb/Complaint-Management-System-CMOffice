using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.FixitNew.CFNP
{
    public class RequestCNFPComplaintsModel
    {
        public string to { get; set; }

        public string from { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime FromDate { get; set; }
    }

    public class ResponseCNFPComplaintsModel
    {
        public List<ComplaintCNFPDbModel> ListResult { get; set; }

    }

    public class ComplaintCNFPModel
    {
        public DateTime complaint_resolve_date { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string complaint_number { get; set; }


        public string phone_num { get; set; }
        public string district { get; set; }
        public string tehsil { get; set; }
        public string uc_no { get; set; }
        public string service { get; set; }
        public string sub_service { get; set; }

    }

    public class ComplaintCNFPDbModel
    {
        public DateTime? complaint_resolve_date { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int complaint_number { get; set; }


        public string phone_num { get; set; }

        public int? district_Id { get; set; }
        public string district { get; set; }

        public int? tehsil_Id { get; set; }
        public string tehsil { get; set; }

        public int? uc_Id { get; set; }
        public string uc_no { get; set; }

        public int? service_Id { get; set; }
        public string service { get; set; }

        public int? sub_Service_Id { get; set; }
        public string sub_service { get; set; }

    }
}