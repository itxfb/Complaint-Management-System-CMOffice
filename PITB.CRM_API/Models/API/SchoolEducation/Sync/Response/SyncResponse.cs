using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API.SchoolEducation.Sync.Response
{
    public class SyncResponse
    {
        public class UserDataResponse : ApiStatus
        {
            public List<UserData> ListUserData { get; set; }
        }
        public class UserData
        {
            public int UserId { get; set; }
            public string UserName { get; set; }

            public string Password { get; set; }

            public int ProvinceId { get; set; }

            public int DivisionId { get; set; }

            public int DistrictId { get; set; }

            public int TehsilId { get; set; }

            public int UcId { get; set; }

            public bool IsActive { get; set; }
        }
    }
}