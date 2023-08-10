using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web.Models.Social
{
    public class FacebookDataModel
    {
        public Post MyPost { get; set; }
        public Comment MyComment { get; set; }
        public class Post
        {
            public int PostId { get; set; }
            public string PostDescription { get; set; }
            
        }

        public class Comment
        {
            public int CommentId { get; set; }
            public string CommentText { get; set; }
        }
    }
}