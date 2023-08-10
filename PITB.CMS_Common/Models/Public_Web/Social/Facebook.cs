using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.Public_Web.Social
{
    public class GraphApi
    {


        public static string Profile = "/me?fields=id,first_name,last_name,email";
        public static string Permission = "/me/permissions";
        
    }
}