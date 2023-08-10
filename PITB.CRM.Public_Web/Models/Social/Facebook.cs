using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web.Models.Social
{
    public class GraphApi
    {


        public static string Profile = "/me?fields=id,first_name,last_name,email";
        public static string Permission = "/me/permissions";
        
    }
}