using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Facebook;
using PITB.CRM.Public_Web.Contacts;
using PITB.CRM.Public_Web.Handler.Authentication;
using PITB.CRM.Public_Web.Models.Social;

namespace PITB.CRM.Public_Web.Handler
{
    public class FacebookHandler :ISocialAction<FacebookDataModel>
    {
        FacebookAuthHandler authHandler = new FacebookAuthHandler();

        public int PostComment(FacebookDataModel obj)
        {
            FacebookClient client = GetFacebookClient();
            //1755514181129312
            //string message = string.Format("{0}", obj.MyComment.CommentText);
            dynamic parameters = new ExpandoObject();
            parameters.message = "SAMI";
            string postId = "1492064529126_1755514181129312";
            client.Post("1492064529126_1755514181129312/comments", parameters);
            return 0;
        }

        public FacebookClient GetFacebookClient()
        {
            return new FacebookClient(authHandler.GetAccessToken());
        }
    }
}