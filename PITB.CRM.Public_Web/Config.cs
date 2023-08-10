using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web
{
    public class Config
    {
        public enum ExternalProvider : byte
        {
            None = 0,
            Facebook,
            Google,
            Twitter
        }
        public enum AttachmentReferenceType
        {
            Add = 1,
            ChangeStatus = 2
        }

        public enum ComplaintStatus
        {
            PendingFresh = 1,
            ResolvedUnverified = 2,
            ResolvedVerified = 3,
            Notapplicable = 4,
            Fake = 5,
            UnsatisfactoryClosed = 6,
            PendingReopened = 7,
            Inapplicable = 8,
            ResolvedUnverifiedEscalateable = 12,
            InApplicableEscalateable=13
        }

        public enum PublicComplaintStatus : byte
        {
            Pending = 1,
            Resolved = 2,
            UnsatisfactoryClosed = 3,
            Inapplicable = 4

        }
        public enum Campaign : byte
        {
            FixIt = 49
        }

        public static string AuthenticationType = "ExternalCookie";
        public static string ExternalCookieName = "ExternalSocial";
        public static string FacebookAccessTokenKeyName = "FacebookAccessToken";
        public static string FacebookFirstName = "urn:facebook:first_name";
        public static string FacebookLastName = "urn:facebook:last_name";
        public static string FacebookUserId = "urn:facebook:id";

        public static byte PageDraw = 10;

        public enum UserVote : byte
        {
            NoVote = 0,
            UpVote = 1,
            DownVote = 2
        }



    }
}