using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Models.API
{
    public class GetStakeholderStatuses
    {
        public List<User> UsersList { get; set; }
        public class User
        {
            public string Username { get; set; }
            public List<DbStatuses> ListStatuses { get; set; }
        }
    }

    public class GetStakeholderStatusesResponse
    {
        //public TYPE Type { get; set; }
    }
}