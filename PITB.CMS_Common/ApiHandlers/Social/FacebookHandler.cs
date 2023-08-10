using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;

namespace PITB.CMS_Common.ApiHandlers.Social
{
    public class FacebookHandler
    {

        public class FBProfile
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Permissions Permissions { get; set; }
        }

        public class Permissions
        {
            public Datum[] Data { get; set; }
        }

        public class Datum
        {
            public string Permission { get; set; }
            public string Status { get; set; }
        }

        public static bool IsTokenValid(string accessToken)
        {
            var fbClient = new FacebookClient(accessToken);
            try
            {
                var data = fbClient.Get("/me?id,name");
                if (data != null) return true;
                return false;
            }
            catch (Exception)
            {

                return true;
            }
        }
        public static bool IsTokenValid(FacebookClient client)
        {
            try
            {
                var data = client.Get("/me?id,name");
                if (data != null) return true;
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static string CreatePost()
        {
            return null;
        }
    }
}