using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class GetStakeholderStatuses
    {
        public List<User> UsersList { get; set; }
        public class User
        {
            public string Username { get; set; }
            public List<DbStatus> ListStatuses { get; set; }
        }
    }

    public class GetStakeholderStatusesResponse
    {
        //public TYPE Type { get; set; }
    }
}