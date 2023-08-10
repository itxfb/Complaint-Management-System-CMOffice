using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PITB.CRM_API
{
    public class Config
    {

        public static List<string> ListHeadersForTokenGeneration = new List<string>
        {
            
        };

        public enum UserType : byte
        {
            Complainant = 1,
            Resolver = 2
        }

        public enum VersionType : byte
        {
            Db = 1,
            MobileApp = 2
        }

        public enum MsgSrcType : byte
        {
            Web = 1,
            Mobile = 2
        }

        public enum SchoolType
        {
            Secondary = 2,
            Elementary = 1
        }

        public enum MsgType : byte
        {
            ToStakeholder = 1,
            ToComplainant = 2
        }


        public enum CampaignType
        {
            SchoolEducation = 1
        }

        public enum Categories
        {
            First = 1,
            Second = 2,
            Third = 3
        }

        public enum SchoolEducationAssignedTo
        {
            MEA = 1,
            SDU = 2
        }

        public enum Campaign
        {
            Wasa = 35,
            Lwmc = 36,
            Cdgl = 39,
            FixItLwmc = 49,
            SchoolEducationEnhanced = 47,


        }

        public enum Language : byte
        {
            Default = 0,
            English = 1,
            Urdu = 2
        }

        public enum District
        {
            Lahore = 1,
            Bahawalpur = 18
        }

        public enum AppID : byte
        {
            Awazekhalq = 0,
            SmartLahore = 1,
            FixitLwmc = 2
        }

        public enum PlatformID : byte
        {
            None = 0,
            Android = 1,
            IOS = 2
        }
        public enum UserVote : byte
        {
            NoVote = 0,
            UpVote = 1,
            DownVote = 2
        }

        public enum PermissionsType
        {
            Campaign = 1,
            User = 2
        }
        public enum Permissions
        {
            TransferComplaint = 1,
            //ChangeStatusInComplaintsAll = 2,
            ChangeCategoryInStakeholderDetail = 2,
            ShowStatusChangeInComplaintsAllStakeholder = 3,
            ShowStakeholderEscalationInDetail = 4,
            ShowOnlyComplaintsAllInResolver = 5,
            StakeholderLandOnDashboard = 6,
            StakeholderStatusesOnStatusChangeView = 7,
            ShowProfileInfoInStakeholderDetail = 8,
            StatusesForComplaintListing = 9,
            StatusesForComplaintListingAll = 10,
            HideProfileInfoInStakeholderDetail = 11
        }

        public enum CampaignPermissions
        {
            DontSendMessages = 1,
            ShowWards = 2,
            ShowStakeholderMessagesView = 3,
            HideProfileInfoFromStakeholderDetail = 4,
            CanResetEscalation = 5

        }

        public const int PageSizeMobileLWMC = 20;
        public enum Roles : byte
        {
            [Display(Name = "Call Centre Agent")]
            Agent = 1,
            [Display(Name = "Call Centre Supervisor")]
            AgentSuperVisor = 2,
            [Display(Name = "Stakeholder")]
            Stakeholder = 3,

            [Display(Name = "AdminStakeholder")]
            AdminStakeholder = 4,

            [Display(Name = "AdminCampaign")]
            AdminCampaign = 5
        }

        public enum SubRoles : byte
        {
            [Display(Name = "SDU")]
            SDU = 1
        }

        public const int UserMaxHierarchy = 1000;

        public const string SchoolEducationUnsatisfactoryStatus = "Pending (Overdue)";
        public const string UnsatisfactoryClosedStatus = "Unsatisfactory Closed";
        public enum StakeholderComplaintListingType
        {
            None = 0,
            AssignedToMe = 1,
            UptilMyHierarchy = 2,
            Inquiry = 3,
            Suggestion = 4
        }

        public enum CnicOrSocialPresent : byte
        {
            None,
            OnlyCnic,
            OnlySocial,
            CnicAndSocial
        }
        public enum Hierarchy : int
        {
            [Display(Name = "Province Level")]
            Province = 1,
            [Display(Name = "Division Level")]
            Division = 2,
            [Display(Name = "District Level")]
            District = 3,
            [Display(Name = "Tehsil Level")]
            Tehsil = 4,
            [Display(Name = "Union council Level")]
            UnionCouncil = 5,
            [Display(Name = "Ward Level")]
            Ward = 6
        }

        public static List<Config.ComplaintStatus> ListMeaStatuses = new List<Config.ComplaintStatus>() { Config.ComplaintStatus.ResolvedVerified, Config.ComplaintStatus.PendingReopened };
        public enum TableRef
        {
            SchoolEducation = 1
        }

        public enum CategoryType : byte
        {
            Main = 1,
            Sub = 2
        }

        public enum ComplainantRemarkType : byte
        {
            None = 0,
            Satisfied = 1,
            Unsataisfied = 2
        }

        public enum Province : byte
        {
            Punjab = 1
        }


        public enum IncrementalType : byte
        {
            GetReplyMsg = 1
        }

        public enum ResponseType : byte
        {
            Success = 1,
            Failure = 2,
            AuthenticationSuccess = 3,
            AuthenticationFailed = 4,
            ParameterError = 5,

        }

        public static Dictionary<ResponseType,string> ResponseMessages = new Dictionary<ResponseType, string>
        {
            {ResponseType.Success, "Success"},
            {ResponseType.Failure, "Faiure"},

            {ResponseType.AuthenticationSuccess, "Authentication Success"},
            {ResponseType.AuthenticationFailed, "Authentication Failed"},
        };  

        public enum ComplaintSource : byte
        {
            Agent = 1,
            Mobile = 2,
            Public = 3,
            OtherSystem = 4
        }

        public enum RequestType : byte
        {
            Web = 1,
            Mobile = 2
        }

        public enum MessageStatus : byte
        {
            New = 1,
            Sending = 2,
            Sent = 3
        }

        public enum MobileUserRequestType : byte
        {
            AddComplaint = 1,
            ChangeStatus = 2,
            MessageSubmit = 3
        }

        public enum AttachmentSaveType : byte
        {
            WebServer = 1,
            LocalFileSystem = 2
        }

        public enum FileType : byte
        {
            File = 1,
            Video = 2
        }

        public enum AttachmentReferenceType : byte
        {
            Add = 1,
            ChangeStatus = 2
        }

        public enum ComplaintType : byte
        {
            Complaint = 1,
            Suggestion = 2,
            Inquiry = 3
        }


        public const int MEALoginId = 1454;
        public enum SchoolEducationUserSubRoles
        {
            MEA = 1,
            SDU = 2
        }

        public static List<int> ListNewFixitCampaings = new List<int>{54,55,56,57,58,59,60};
        public const int NewFixitResolvedStatusId = 8;

        public enum ComplaintStatus : byte
        {
            [Display(Name = "Pending (Fresh)")]
            PendingFresh = 1,
            [Display(Name = "Resolved (Unverified)")]

            ResolvedUnverified = 2,
            [Display(Name = "Resolved (Verified)")]

            ResolvedVerified = 3,
            [Display(Name = "Not Applicable")]

            Notapplicable = 4,
            [Display(Name = "Fake")]

            Fake = 5,
            [Display(Name = "Unsatisfactory Closed")]
            UnsatisfactoryClosed = 6,

            [Display(Name = "Pending (Reopened)")]

            PendingReopened = 7,
            [Display(Name = "Resolved")]

            Resolved = 8,
            [Display(Name = "Not Applicable (Verified)")]

            NotApplicableVerified = 9,
            [Display(Name = "Fake (Verified)")]

            FakeVerified = 10,
            [Display(Name = "Closed (Verified) ")]

            ClosedVerified = 11,
            [Display(Name = "Resolved (Unverified)")]

            ResolvedUnverifiedEscalatable = 12,

            [Display(Name = "Inapplicable")]

            Inapplicable=13
        }
        public enum ExternalProvider : byte
        {
            None,
            Facebook,
            Google,
            Twitter
        }

        public enum UserHirerchyLwmc : int
        {
            None = 0,
            Zoner = 1,
            AM = 2,
            HigherManagement = 3
        }
        public enum FilterTypeApi : byte
        {
            None = 0,
            MostRecent = 1,
            Popular = 2,
            Nearest = 3
        }

        public static double RadiusRangeInKiloMeters = 10;
        public static Dictionary<int, int> CampDistDictionary = new Dictionary<int, int>() { { 18, 8 }, { 35, 1 }, { 36, 1 }, { 39, 1 } };

        public static List<AppConfig> LwmcAppConfigurations
        {
            get
            {
                List<AppConfig> listAppCampaignConfig = new List<AppConfig>();


                // App configuration for SmartLahore
                AppConfig appCampaignConfiguration = new AppConfig
                {
                    AppId = AppID.FixitLwmc,
                    ListCampaigns = new List<CampaignConfig>
                    {
                        new CampaignConfig() {CampaignId = Campaign.FixItLwmc, DistrictId = District.Lahore}
                    }
                };

                listAppCampaignConfig.Add(appCampaignConfiguration);
                return listAppCampaignConfig;
            }
        }

        public static List<AppConfig> ListAppCampaignConfigurations
        {
            get
            {
                List<AppConfig> listAppCampaignConfig = new List<AppConfig>();

                //List<CampaignConfig> listCampaigns = new List<CampaignConfig>();

                // App configuration for SmartLahore
                AppConfig appCampaignConfiguration = new AppConfig
                {
                    AppId = AppID.SmartLahore,
                    ListCampaigns =
                        new List<CampaignConfig>
                        {
                            new CampaignConfig()
                            {
                                CampaignId = Campaign.Cdgl,
                                DistrictId = District.Lahore
                            },
                            new CampaignConfig()
                            {
                                CampaignId = Campaign.Lwmc,
                                DistrictId = District.Lahore
                            },
                            new CampaignConfig()
                            {
                                CampaignId = Campaign.Wasa,
                                DistrictId = District.Lahore
                            }
                        }
                };


                AppConfig fixItLwmc = new AppConfig
                {
                    AppId = AppID.FixitLwmc,
                    ListCampaigns = new List<CampaignConfig>
                    {
                        new CampaignConfig() {CampaignId = Campaign.FixItLwmc, DistrictId = District.Lahore}
                    }
                };
                listAppCampaignConfig.Add(appCampaignConfiguration);
                listAppCampaignConfig.Add(fixItLwmc);

                return listAppCampaignConfig;
            }
        }



        public class AppConfig
        {
            public AppID AppId { get; set; }
            public List<CampaignConfig> ListCampaigns { get; set; }

        }

        public class CampaignConfig
        {
            public Campaign CampaignId { get; set; }

            public District DistrictId { get; set; }

        }

        public class PWSConfig
        {
            public string AccessKeyId { get; set; }
            public string AccessKeySecret { get; set; }
            public string Bucket { get; set; }
            public string UrlPrefix { get; set; }
            public PWSConfig()
            {
            }

            public PWSConfig(string accessKeyId, string accessKeySecret, string bucket, string urlPrefix)
            {
                this.AccessKeyId = accessKeyId;
                this.AccessKeySecret = accessKeySecret;
                this.Bucket = bucket;
                this.UrlPrefix = urlPrefix;
            }
        }
        public class AmazonConfig
        {
            public string AmazonKeyId { get; set; }

            public string AmazonSecretKey { get; set; }

            public string AmazonBucket { get; set; }

            public string AmazonUrlPrefix { get; set; }

            public AmazonConfig()
            {
            }

            public AmazonConfig(string amazonKeyId, string amazonSecretKey, string amazonBucket, string amazonUrlPrefix)
            {
                AmazonKeyId = amazonKeyId;
                AmazonSecretKey = amazonSecretKey;
                AmazonBucket = amazonBucket;
                AmazonUrlPrefix = amazonUrlPrefix;
            }
        }

        public const string FacebookPrefixCnic = "FFFFF";
        public const string AnonymousPictureOnlinePath = "https://static.xx.fbcdn.net/rsrc.php/v3/yo/r/UlIqmHJn-SK.gif";

        public const string AmazonConfigDevKeyId = "AKIAIILWDOUZQR3K5ESA";
        public const string AmazonConfigDevSecretKey = "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";
        public const string AmazonDevBucket = "crm-cms-dev";
        public const string AmazonDevUrlPrefix = "https://crm-cms-dev.s3.amazonaws.com/";

        public const string AmazonConfigProdKeyId = "AKIAIZNFP6Z6UXMXRY7Q";
        public const string AmazonConfigProdSecretKey = "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";
        public const string AmazonProdBucket = "crm-cms";

        public const string AmazonProdUrlPrefix = "https://crm-cms.s3.amazonaws.com/";





        public const string PWSConfigDevKeyId = "D602F89B1A3A4142A898DE5AE6F240FD";
        public const string PWSConfigDevSecretKey = "358C897C0A144A3CAAF9EC61CB856B24";
        public const string PWSDevBucket = "crm-cms-dev";
        public const string PWSDevUrlPrefix = "https://storage.punjab.gov.pk/crm-cms-dev/";

        public const string PWSConfigProdKeyId = "C4A6466EEF8D49809DB55882CE6A23ED";
        public const string PWSConfigProdSecretKey = "A23F41B1527241A8AC0A2C052A022B43";
        public const string PWSProdBucket = "crm-cms";
        public const string PWSProdUrlPrefix = "https://storage.punjab.gov.pk/crm-cms/";



        public const string FileName = "Mobile_App_Upload";
        public const string FileExtension = ".jpg";
        public const string FileContentType = "image/jpeg";

        

        public enum LwmcResolverHirarchyId
        {
            Zoner = 10,
            Am = 20,
            HigherLevelInitial = 30,
            HigherLevelFinal = 50
        }

        


    }
}