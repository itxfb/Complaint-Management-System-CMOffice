using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PITB.CMS_Common
{
    public class ApiStatusMessages
    {
        public static string NoVote = "Your have submitted no vote.";
        public static string VoteSubmitted = "Your vote has been submitted successfully.";
        public static string AlreadyVoted = "You have already submitted your vote for this complaint.";
        public static string PostShared = "Your post successfully submitted";
        public static string PostAlreadyShared = "You have already shared this post";


    }

    public static class StringExtensions
    {
        public static string TrimEndLine(this string s)
        {
            return Regex.Replace(s, @"\t|\r|\n", "");
        }
        public static string TrimTab(this string s)
        {
            return Regex.Replace(s, @"\t", "");
        }
    }
}