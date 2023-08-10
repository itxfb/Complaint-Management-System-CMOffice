using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
//using PITB.CMS_Common.Helper;
using System;
using System.Web.Routing;
using System.Configuration;
//using PITB.CMS_Common.Models.Custom;
using System.ComponentModel;
using PITB.CMS_Common.Helper.Extensions;

namespace PITB.CMS_Common
{
    public partial class Config
    {
        public class Web
        {

        }
        public class Api
        {

        }
        public class WindowService
        {

        }
        public const string TAG_COMPLAINT_ADD = "ComplaintAdd";
        public const string TAG_COMPLAINT_STATUS_CHANGE = "ComplaintStatusChange";

        // Police System Tags
        public const string TAG_COMPLAINT_ACTION_INTERIM = "ComplaintActionInterim";
        public const string TAG_COMPLAINT_ACTION_FINAL = "ComplaintActionFinal";

        public const string FILTER_NAME_POLICE_ACTION = "TablePoliceActionRowId";
        public const string FILTER_NAME_POLICE_ACTION_CURRENT_STEP = "TablePoliceActionCurrentStep";

        public const string ApiUrlSchoolComplaintSystem = "http://complaints.schools.punjab.gov.pk";
        public const string ApiKeySchoolComplaintSystem = "197353s00sdf38df0s94422l3l93-32394223hfowue232akkooe";

        public const string MasterPageAgentPolice = "~/Views/Shared/_AgentPoliceLayout.cshtml";
        public const string MasterPageAgentSpecialEducation = "~/Views/Shared/_AgentSpecialEducationLayout.cshtml";
        public const string DefaultMasterPageAgent = "~/Views/Shared/_MainLayout.cshtml";

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
            AdminCampaign = 5,
            [Display(Name = "PriviledgedUser")]
            PriviledgedUser = 6,
            [Display(Name = "Executive")]
            Executive = 7,

            [Display(Name = "ExecutiveCampaigns")]
            ExecutiveCampaigns = 8,

            [Display(Name = "PublicUser")]
            PublicUser = 9
        }

        public const int FunctionRetryCount = 4;

        public enum SubRoles : byte
        {
            [Display(Name = "SDU")]
            SDU = 1
        }

        public const string ContentTypePdf = "application/pdf";

        public const string Separator = "___";

        public const string SqlDateFormat = "yyyy-MM-dd";

        public const string EncryptionKey1 = "abcdefghijklmnopqrstuvwxyz-SIOEWOIJWEUQIIPEPPEUHELiEIu=232399";
        public const string EncryptionKey2 = "ereuwegkdeoeiuwiwqwueuryws3FSOUEWPSDKAPEIUEIOWSMSMSQY94203842";
        public const string EncryptionKey3 = "uewirywytewieiquweieorpwwqbEUIDJDQPDIUCN<V<ZXZAPu9420384234-_";

        public const string SaltKey = "S@LT&KEY";
        public const string VIKey = "@1B2c3D4e5F6g7H8";

        public const int ServerSideDropDownListSize = 10;

        public const int DataPoolMaxLength = 5000;

        public const string SchoolEducationUnsatisfactoryStatus = "Pending (Overdue)";
        public const string WasaPendingFreshStatus = "Un-Resolved";
        public const string UnsatisfactoryClosedStatus = "Overdue";
        public const string PendingFreshStatus = "Pending (Fresh)";
        public const string ClosedVerifiedStatus = "Closed (Verified)";
        public const string SchoolUcLvl = "Markaz";

        public static List<int> ListSchoolEducationStatuses = new List<int> { 6, 7, 1, 2, 3, 11 };

        public static List<int> ListSchoolEducationSecretaryDistricts = new List<int> { 8, 1, 34, 35 };

        public const string SchoolEducationSecretaryPhoneNo = "03334233733";

        public const int MEALoginId = 1454;

        public const int DashBoardSpecific1LoginId = 55880;

        public const int FollowupId = -1;

        public const string TransferStr = "1,0";

        public const string None = "None";

        public List<QuarterDistribution> ListQuarterDistribution = new List<QuarterDistribution>
        {
            new QuarterDistribution{QuarterId = 1, FromMonth = 1, ToMonth = 3}
        };



        public class QuarterDistribution
        {
            public int QuarterId { get; set; }
            public int FromMonth { get; set; }

            public int ToMonth { get; set; }
        }

        public enum UniqueIncrementorTag
        {
            Cnic = 1
        }

        public enum FilterTypeApi : byte
        {
            None = 0,
            MostRecent = 1,
            Popular = 2,
            Nearest = 3
        }

        public const int CnicLength = 16;

        public enum Campaign
        {
            ZimmedarShehri = 1,
            SchoolEducationEnhanced = 47,
            FixItLwmc = 49,
            WasaNew = 50,
            Hospital = 69,
            DcoOffice = 70,
            Police = 78,
            [Description("Awaz-e-Khalq")]
            AwazeKhalq = 18,
            [Description("Punjab Land Revenue Authority")]
            PLRA = 80,
            SpecialEducation = 83,
            CombinedCampaign = 87,
            LGCD = 88,

            Wasa = 35, // from API
            Lwmc = 36, // from API
            Cdgl = 39, // from API
            LocalGovNew = 95,
            DcChiniot = 99,
            CMCC = 104,
            Health = 105,
            LGCDCMCC = 106
        }

        public const string CampaignSchoolEducation = "School Education Enhanced";

        public enum UserWiseGraphType
        {
            MyOwn = 1,
            CumulationOfLower = 2,
            LowerIndividual = 3,
            GeneralCampaign = 4
        }
        public enum AgingReportType
        {
            Monthly = 1,
            Quarterly = 2,
            Yearly = 3
        }

        public enum TableRef
        {
            SchoolEducation = 1
        }

        public enum SchoolType
        {
            Secondary = 2,
            Primary = 1,
            Elementary = 3
        }

        public static List<int> DashboardStatusOrderList = new List<int> { 6, 7, 1, 2, 3, 11 };


        public static List<int> ListSchoolUsersToDiscartDashboard = new List<int> { 1200, 1201, 1202, 1203 };

        public static List<string> ListAgingReportXDistribution = new List<string> { ">75% (Early)", "51-75% (Early)", "26-50% (Early)", "10-25% (Early)", "In Time", "0-25% (Late)", "26-50% (Late)", "51-75% (Late)", "75%< (Late)" };

        public static List<float> ListPercentageDistribution = new List<float> { 100000, 75, 50, 25, 10, 0, -25, -50, -75, -100000 };

        public static Dictionary<string, string> DictTranslationMap = new Dictionary<string, string>()
        {
            {"School Education Hotline (042-111-11-2020)","School Education"}
        };
        public static Dictionary<string, string> CampaignIdTranslation = new Dictionary<string, string>(){
            {"Health","68,69,72,73,74"},
            {"DC-Office","70"},
            {"Police","71"},
            {"Education","47"}
        };

        public enum BinaryStatus
        {
            Yes = 1,
            No = 2
        }
        public enum SchoolEducationUserSubRoles
        {
            MEA = 1,
            SDU = 2
        }

        public enum MsgSrcType
        {
            Web = 1,
            Mobile = 2,
            WindowService = 3
        }

        public enum SourceType
        {
            Web = 1,
            Service = 2,
            OtherSystemApi = 3,
        }

        public enum DeployedOnServer
        {
            Local = 1,
            Crm = 2,
            Police = 3
        }

        public enum ServerType
        {
            Local = 1,
            Production = 2
        }

        public enum HelplineSrc
        {
            None = 0,
            DefaultHelpline = 1,
            ChiefMinisterHelpline = 2,
            SpecialBranch = 3

        }

        public enum Events
        {
            ComplaintAssignToOrigin = 1
        }

        public enum MsgType
        {
            ToStakeholder = 1,
            ToComplainant = 2
        }

        public enum CategoryType
        {
            Main = 1,
            Sub = 2,
            Tertiary = 3
        }

        public enum ComplaintSource
        {
            Agent = 1,
            Mobile = 2,
            Public = 3,
            //Whatsapp = 4
            OtherSystem = 4
        }

        public enum PermissionsType
        {
            Campaign = 1,
            User = 2,
            WindowsService = 3,
            General = 4,
            //CampaignMapToOtherSystem = 4
        }



        public enum CampaignPermissions
        {
            DontSendMessages = 1,
            ShowWards = 2,
            ShowStakeholderMessagesView = 3,
            HideProfileInfoFromStakeholderDetail = 4,
            CanResetEscalation = 5,
            CampaignAssignmentMatrix = 6,
            ExecutiveCampaignStatusReMap = 7,
            CategoriesForFeedback = 8,
            ResolverUserPermission = 9,
            AllowedCtrlActionList = 10,
            PaintDashboardLabels = 11
            //CampaignMapIds = 8,
            //CampaignMapStatusIds = 9
        }
        public enum FeedbackStatuses
        {
            Satisfied = 1,
            Dissatisfied = 2,
            NoAnswer = 3,
            Busy = 4,
            Cancel = 5,
            Congestion = 6,
            NotComplete = 7,
            NotApplicable = 0,
            Pending = -1
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
            HideProfileInfoInStakeholderDetail = 11,
            HideStatusChangeInComplaintsAssignedToMeStakeholder = 12,
            ShowCallBackStatusSubmitInViewOfAssignedToMe = 13,
            ShowCallBackStatusSubmitInViewOfSubordinates = 14,
            ViewComplaintTypeWiseData = 15,
            LoginNavigationForCampaignUser = 16,
            ExecutiveMasterpageNavigation = 17,
            MultipleCampaignsAssignment = 18,
            MultipleCampaignsMerger = 19,
            ExecutiveCampaignWiseNavigation = 20,
            CombineMultipleStatusCountInDashboard = 21,
            ShowSpecificListingPageToUser = 22,
            ViewableSchoolsToUser = 23,
            AllowedCtrlActionList = 24,
            QaedUserShowExtraColumnInComplaintExport = 25,
            AssignmentMatrixTagOnStatusChange = 26,
            ChangeTimeForComplaint = 27,
            UpdateComplaintDetail = 28,
            CMCCExtraColumnInComplaintExport= 29
            //ExecutiveNavigationToUser = 21
        }
        public enum WindowsServicePermissions
        {
            SendSmsToExecutivesCampaigns = 1,
            SendSmsToExecutivesCumulativeCampaigns = 2,
            SendHierarchyMessages = 3,
            GeneralExecutiveSmsPhoneNos = 4,
            CampaignSpecificExecutiveSmsPhoneNos = 5
        }
        public enum GeneralPermissions
        {
            CumulativeCampaignName = 1,
        }
        public enum SelectionType
        {
            All,
            Specific
        }

        public enum MsgTags
        {
            SchoolEducationStakeholder = 1,
            SchoolEducationStakeholderSupervisor = 2,
            SchoolEducationSecretary = 3,
            SchoolEducationStakeholderUsernameChangeReminder = 4,
            SchoolEducationStakeholderUsernameAndPasswordNotification = 5
        }

        public enum Language : byte
        {
            Default = 0,
            English = 1,
            Urdu = 2
        }

        public enum ResponseType : byte
        {
            Success = 1,
            Failure = 2,
            AuthenticationSuccess = 3,
            AuthenticationFailed = 4,
            ParameterError = 5,

        }

        public enum ResponseCodesMobile
        {
            Success = 200,
            ServerError = 500,
            NoDataFound = 404,
            AuthenticationError = 401,
            UpdateAppUrl = 402,
            Unauthorized = 520,
        }

        public enum NotificationType
        {
            Complaint = 1
        }

        public enum NotificationSubType
        {
            Launch = 1
        }

        public enum NotificationStatus
        {
            DontSend = 0,
            Send = 1
        }

        public enum DynamicCategoryTypeId
        {
            SchoolEducationTeachingQuality = 8
        }

        public enum DynamicFieldControlId
        {
            SchoolEducationTeachingQuality = 19
        }

        public enum PieGraphTypes
        {
            None,
            SchoolEducationStatuses,
            SchoolEducationTeachingQuality,
            SchoolEducationCallVolume,
            SchoolEducationCompliantsByCategory,
            SchoolEducationPhcipDashboard
            //    ,
            //SchoolEducationPhcipComplaintStatus,
            //SchoolEducationPhcipComplaintSource,
            //SchoolEducationPhcipComplaintByStakeholder
        }


        public enum DynamicControlType
        {
            [Display(Name = "dropdown")]
            DropDownList = 1,
            [Display(Name = "edittext")]
            TextBox = 2,
            [Display(Name = "serversidesearchableDropdown")]
            DropDownListServerSideSearchable = 3,
            [Display(Name = "label")]
            Label = 4,
            [Display(Name = "attachment")]
            FileAttachment = 5,
            [Display(Name = "date")]
            Date = 6,
            [Display(Name = "multiselectdropdown")]
            MultiSelectDropdown = 7,

            [Display(Name = "hidden")]
            Hidden = 8,
            [Display(Name = "edittext")]
            Password = 9,
            [Display(Name = "url")]
            URL = 10,

            [Display(Name = "CheckBox")]
            CheckBox= 11

            
        }


        public enum PublicUserComplaintListingType
        {
            AddedByMe = 1
        }

        public enum AgentComplaintListingType
        {
            AddedByMe = 1,
            All = 2
        }

        public enum StakeholderComplaintListingType
        {
            None = 0,
            AssignedToMe = 1,
            UptilMyHierarchy = 2,
            Inquiry = 3,
            Suggestion = 4
        }

        public enum DynamicFieldType
        {
            SingleText = 1,
            MultiSelection = 2,
        }

        public enum CategoryHierarchyType
        {
            None = -1,
            MainCategory = 1,
            SubCategory = 2,
        }

        public enum Hierarchy : int
        {
            [Display(Name = "N/A")]
            None = 0,
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

        public const int UserMaxHierarchy = 1000;

        public enum UserHierarchy : int
        {
            [Display(Name = "N/A")]
            None = 0,
            [Display(Name = "Province Level")]
            First = 1,
            [Display(Name = "Division Level")]
            Second = 2,
            [Display(Name = "District Level")]
            Third = 3,
            [Display(Name = "Tehsil Level")]
            Fourth = 4,
            [Display(Name = "Union council Level")]
            Fifth = 5
        }

        public static readonly List<Config.Hierarchy> ListHierarchy = new List<Hierarchy>() { Hierarchy.Province, Hierarchy.Division, Hierarchy.District, Hierarchy.Tehsil, Hierarchy.UnionCouncil };

        public enum MessageType
        {
            success,
            error,
            info,
            warning
        }
        public enum CommandStatus : byte
        {
            Failure = 0,
            Success = 1,
            Exception = 2,
        }

        public enum Gender : byte
        {
            Female = 0,
            Male = 1
        }

        public enum ComplaintType
        {
            [PluralName("None")]
            None = 0,
            [PluralName("Complaints")]
            Complaint = 1,
            [PluralName("Suggestions")]
            Suggestion = 2,
            [PluralName("Inquires")]
            Inquiry = 3
        }

        public enum AttachmentType : byte
        {
            File = 1,
            Video = 2
        }

        public enum ComplainantRemarkType : byte
        {
            None = 0,
            Satisfied = 1,
            Unsataisfied = 2
        }

        public enum FileType
        {
            Excel, PDF
        }

        public enum ComplaintStatus
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

            [Display(Name = "Closed (Verified)")]
            ClosedVerified = 11,

            [Display(Name = "Inapplicable (Verfied)")]
            InapplicableVerfied = 14,

            [Display(Name = "InApplicable Escalateable")]
            InApplicableEscalateable = 13,

            [Display(Name = "In Proceeding")]
            InProceeding = 23,

            [Display(Name = "Irrelevant")]
            Irrelevant = 21,

            [Display(Name = "In Progress")]
            InProgress = 22,

            [Display(Name = "Resolved (Unverified)")]
            ResolvedUnverifiedEscalatable = 12,

            [Display(Name = "Relief granted")]
            ReliefGranted = 25,

            //[Display(Name = "Pending (Fresh)")]
            //PendingFresh = 1,
            //[Display(Name = "Resolved (Unverified)")]

            //ResolvedUnverified = 2,
            //[Display(Name = "Resolved (Verified)")]

            //ResolvedVerified = 3,
            //[Display(Name = "Not Applicable")]

            //Notapplicable = 4,
            //[Display(Name = "Fake")]

            //Fake = 5,
            //[Display(Name = "Unsatisfactory Closed")]
            //UnsatisfactoryClosed = 6,

            //[Display(Name = "Pending (Reopened)")]

            //PendingReopened = 7,
            //[Display(Name = "Resolved")]

            //Resolved = 8,
            //[Display(Name = "Not Applicable (Verified)")]

            //NotApplicableVerified = 9,
            //[Display(Name = "Fake (Verified)")]

            //FakeVerified = 10,
            //[Display(Name = "Closed (Verified) ")]

            //ClosedVerified = 11,
            //[Display(Name = "Resolved (Unverified)")]

            //ResolvedUnverifiedEscalatable = 12,

            //[Display(Name = "Inapplicable")]

            //Inapplicable = 13
        }

        public enum StatusEscalationAction
        {
            ResetEscalationFromFirstUser = 1,
            ResetEscalationFromUserWhoChangedStatus = 2,
            //ResetEscalationFromUserWhoChangedStatusAndGiveExtraTime = 3,
            ImmediatelyMoveComplaintToNextUserAfterStatusChange = 3,
            DontChangeDatesAtAll = 4,
        }


        public static Dictionary<int, string> StatusDict = new Dictionary<int, string>()
        {
            {1,"Pending (Fresh)"},
            {2,"Resolved (Unverified)"},
            {3,"Resolved (Verified)"},
            {4,"Not Applicable"},
            {5,"Fake"},
            {6,"Unsatisfactory Closed"},
            {7,"Pending (Reopened)"},
            {8,"Resolved"},
            {9,"Not Applicable (Verified)"},
            {10,"Fake (Verified)"},
            {11,"Closed (Verified)"},
            {12,"Resolved (Unverified)"},
            {13,"Inapplicable"},
            {14,"Inapplicable (Verfied)"},
            {15,"Not Related/Sub-Division Mismatch"},
            {16,"Pending Due to Funds"},
            {17,"Untraceable"},
            {18,"Unresolved"},
            {19,"Pending"},
            {20,"Closed (Chronic)"}
        };

        public static Dictionary<int, string> HiertarchyTableConfigDict = new Dictionary<int, string>()
        {
            {1,"Pitb.Provinces,Id,Province_Name"},
            {2,"Pitb.Divisions,Id,Division_Name"},
            {3,"Pitb.Districts,Id,District_Name"},
            {4,"Pitb.Tehsil,Id,Tehsil_Name"},
            {5,"Pitb.Union_Councils,Id,Councils_Name"},
            {6,"Pitb.Wards,Id,Wards_Name"}
        };

        public static Dictionary<int, string> RegionDict = new Dictionary<int, string>()
        {
            {1,"Province"},
            {2,"Division"},
            {3,"District"},
            {4,"Tehsil"},
            {5,"Union Council"},
            {6,"Ward"}
        };

        public static Dictionary<int, string> ScoolEducationRegionDict = new Dictionary<int, string>()
        {
            {1,"Province"},
            {2,"Division"},
            {3,"District"},
            {4,"Tehsil"},
            {5,"Markaz"},
            {6,"Ward"}
        };

        public static Dictionary<string, string> DictFormPostTag = new Dictionary<string, string>()
        {
            {"StatusChange", "ClassName::PITB.CMS_Common.Handler.Complaint.StatusHandler__MethodName::ChangeStatus__MethodType::Static"}
        };

        public static Dictionary<ConfigType, string> DictConfigType = new Dictionary<ConfigType, string>()
        {
            {ConfigType.Config, "Config"},
            {ConfigType.Query, "Query"}
        };

        public static Dictionary<Config.Campaign, List<int>> CampaignWiseStatusDict = new Dictionary<Config.Campaign, List<int>>()
        {
            {Config.Campaign.SchoolEducationEnhanced,new List<int>{1,2,3,6,7,11}},
        };

        public static List<int> ListWasaStatusSorted = new List<int> { 1, 8, 15, 16, 17, 7, 6 };

        public enum ConfigType
        {
            Config = 1,
            Query = 2
        }

        public const string CONFIG_TAG_ALLOWED_CTRL_ACTION_LIST = "Config::AllowedCtrlActionList";
        public const string CONFIG_TAG_STATUS_MERGE_LIST_PENDING = "Config::PendingStatusMergeList";
        public const string CONFIG_TAG_STATUS_MERGE_LIST_RESOLVED = "Config::ResolvedStatusMergeList";
        public enum MessageStatus
        {
            New = 1,
            Sending = 2,
            Sent = 3
        }

        public const int MaxFileSize = 5000000;

        public enum AttachmentSaveType
        {
            WebServer = 1,
            LocalFileSystem = 2
        }

        public enum AttachmentReferenceType
        {
            Add = 1,
            ChangeStatus = 2,
            InterimReport = 3,
            FinalReport = 4
        }


        public enum AttachmentExtensionType
        {
            Image = 1,
            Excel = 2,
            Pdf = 4,
            Word = 5,
            Video = 6

        }

        public enum AttachmentErrorType
        {
            NoError = 1,
            InvalidExtension = 2,
            FileSizeExceeded = 3,
            NoFileAttached = 4
        }

        public class Url
        {
            public string Action { get; set; }
            public string Controller { get; set; }

            public RouteValueDictionary ParamsDict { get; set; }

        }
        /*public static Url GetUrl(byte roleId)
        {
            
            Url url;
            switch (roleId)
            {
                case 0://When an error occurs
                    url = new Url { Action = "Logoff", Controller = "account" };
                    break;
                case (byte) Roles.Agent: //Agent
                    url = new Url { Action = "Search", Controller = "Complaint" }; //Operational
                    break;
                case (byte)Roles.AgentSuperVisor: //Agent Supervisor
                    url = new Url { Action = "Search", Controller = "Complaint" }; //Operational
                    break;
                case (byte)Roles.Stakeholder: //Stakeholder
                    url = new Url { Action = "StakeholderComplaintsListingServerSide", Controller = "Complaint" }; //Operational
                    break;
                case (byte)Roles.AdminStakeholder: //Admin Stakeholder
                    url = new Url { Action = "AddEditUser", Controller = "AdminStakeholder" }; //Operational
                    break;

                case (byte)Roles.AdminCampaign: //Admin Stakeholder
                    url = new Url { Action = "ViewCampaign", Controller = "AdminCampaign" }; //Operational
                    break;

                default:
                    url = new Url { Action = "Logoff", Controller = "account" };
                    break;

            }
            return url;
        }*/

        public class Message
        {
            public const string UpdateSuccess = "Record updated successfully";
            public const string Error = "An error occured while processing your request";
            public const string ComplainError = "Complaint already exist for this user against department";

            public const string Failure = Error;
        }
        public class CommandMessage
        {
            public CommandMessage() { }

            public CommandMessage(CommandStatus status, string message)
            {
                Status = status;
                Value = message;
            }
            public CommandStatus Status { get; set; }
            public string Value { get; set; }

            public static CommandMessage GetFailureMessage()
            {
                return new Config.CommandMessage() { Status = Config.CommandStatus.Failure, Value = Config.Message.Failure };
            }
        }
        public enum District
        {
            Lahore = 1,
            Bahawalpur = 18
        }
        public class CampaignConfig
        {
            public Campaign CampaignId { get; set; }

            public District DistrictId { get; set; }

        }
        public enum AppID : byte
        {
            Awazekhalq = 0,
            SmartLahore = 1,
            FixitLwmc = 2
        }
        public class AppConfig
        {
            public AppID AppId { get; set; }
            public List<CampaignConfig> ListCampaigns { get; set; }

        }
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

        public static List<AppConfig> CombinedCampaignConfigurations
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
                        new CampaignConfig() {CampaignId = Campaign.CombinedCampaign, DistrictId = District.Lahore}
                    }
                };

                listAppCampaignConfig.Add(appCampaignConfiguration);
                return listAppCampaignConfig;
            }
        }

        public enum ServiceType
        {

            SendMsgToDCO = 1,
            MarkComplaintToOriginUserIfPresent = 2,
            SendPendingOverDueSMS = 3,
            SycUserWiseComplaints = 4,
            MainServiceError = 5,
            SyncSchoolDataMain = 6,
            SendSmsToExecutives = 7,
            ImageTransfer = 8
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
                this.AmazonKeyId = amazonKeyId;
                this.AmazonSecretKey = amazonSecretKey;
                this.AmazonBucket = amazonBucket;
                this.AmazonUrlPrefix = amazonUrlPrefix;
            }
        }

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




        public enum SyncApiRequiredData
        {
            districts = 0,
            tehsils = 1,
            markazes = 2,
            school = 3
        }

        public enum CrmModule
        {
            Hierarchy = 1,
            SchoolMapping = 2,
            Users = 3,
            CampaignIds = 4,
            StatusIds = 5
        }


        public enum OtherSystemId
        {
            SchoolEducation = 1,
            Health = 2,
            Police = 3,
            Crm = 4,
            PrivateSchoolEducation = 5, // from PITB CMS
            eTransferSIS = 6,
            AeoApp = 7
        }


        public enum CreatedBy
        {
            Pmiu = -2
        }

        public enum SummaryReportType
        {
            General = 1,
            Specific = 2
        }

        public enum SyncOperations
        {
            NoChange = 0,
            Add = 1,
            Update = 2,
            Delete = 3
        }

        public enum PoliceActionReportType
        {
            Interim = 1,
            Final = 2
        }


        public static Dictionary<string, int> SchoolTypeMapDict = new Dictionary<string, int>
        {
            {"sMosque",1},
            {"Middle",3},
            {"H_Sec",2},
            {"H.Sec.",2},
            {"Primary",1},
            {"PRIMARY",1},
            {"High",2}
        };

        public static Dictionary<string, int> SchoolTypeDict = new Dictionary<string, int>
        {
            {"Secondary", 2},
            {"Primary", 1},
            {"Elementary", 3}
        };

        public static Dictionary<string, int> SchoolGenderDict = new Dictionary<string, int>
        {
            {"Female", 0},
            {"Male", 1},
            {"MALE", 1},
            {"FEMALE", 0}
        };


        public class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success
            {
                get;
                set;
            }
            [JsonProperty("error-codes")]
            public List<string> ErrorMessage
            {
                get;
                set;
            }
        }

        public enum PasswordProperty
        {
            AlphabetsLowerCase = 1,
            AlphabetsUpperCase = 2,
            Numbers = 3,
            Characters = 4,
            All = 5
        }

        public enum LicenseType
        {
            ZBulkOperation = 1
        }

        public const int LicenceRetryCount = 10;

        public const int LicenseStringLength = 10;


        public static List<string> LicensePossibleNamesZBulk = new List<string> { "sdfg", "sfgw", "dsf23", "234ffg", "233fg", "ssdafg34", "None" };

        public const string PMIU_API_DATA = "http://pmiu.pitb.gov.pk/sr_integration_api/district_tehsil_markaz_school_info/";
        public const string PMIU_API_USERS_DATA = "http://pmiu.pitb.gov.pk/sr_integration_api/users_info/";

        public static Dictionary<string, bool> dictEncStatus = new Dictionary<string, bool>()
        {
            {"connectionStr", true} // true on local and false for live
        };
        private static string _connStr = null;
        public static string ConnectionString
        {
            get
            {
                if (_connStr == null)
                {
                    //if (dictEncStatus["connectionStr"])
                    //{
                    //    _connStr = Utility.GetDecryptedString(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
                    //    _connStr = _connStr.Replace("10.50.28.50", "10.50.124.114");
                    //}
                    //else
                    {
                        _connStr = ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString;
                    }
                }
                return _connStr;
            }
            set
            {
                _connStr = value;
            }
        }


        public const string CNPF_ACCESS_TOKEN = "2b4ee0d61871f70fba559a78fa5d7c9f";

        //public const Config.DeployedOnServer CURRENT_DEPLOYMENT_SERVER = Config.DeployedOnServer.Crm;
        public const Config.DeployedOnServer CURRENT_DEPLOYMENT_SERVER = Config.DeployedOnServer.Crm;

        public const Config.ServerType CURRENT_SERVER_TYPE = Config.ServerType.Local;
        public static Dictionary<string, string> DictUrl = new Dictionary<string, string>()
        {
            {"Police-AddComplaint", Utility.GetUrl("api/Police/AddComplaint") },
            {"Police-PostAction", Utility.GetUrl("api/Police/PostAction")},
            {"Police-PostFeedback", Utility.GetUrl("api/Police/PostFeedback")}
        };
    }


    #region API Project Configs
    public partial class Config
    {
        public static List<string> ListHeadersForTokenGeneration = new List<string>
        {

        };

        //public enum FileType : byte
        //{
        //    File = 1,
        //    Video = 2
        //}

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

        public const int PageSizeMobileLWMC = 20;

        public enum CnicOrSocialPresent : byte
        {
            None,
            OnlyCnic,
            OnlySocial,
            CnicAndSocial
        }

        public static List<Config.ComplaintStatus> ListMeaStatuses = new List<Config.ComplaintStatus>() { Config.ComplaintStatus.ResolvedVerified, Config.ComplaintStatus.PendingReopened };

        public enum Province : byte
        {
            Punjab = 1
        }


        public enum IncrementalType : byte
        {
            GetReplyMsg = 1
        }

        public static Dictionary<ResponseType, string> ResponseMessages = new Dictionary<ResponseType, string>
        {
            {ResponseType.Success, "Success"},
            {ResponseType.Failure, "Faiure"},

            {ResponseType.AuthenticationSuccess, "Authentication Success"},
            {ResponseType.AuthenticationFailed, "Authentication Failed"},
        };

        public enum RequestType : byte
        {
            Web = 1,
            Mobile = 2
        }

        public enum MobileUserRequestType : byte
        {
            AddComplaint = 1,
            ChangeStatus = 2,
            MessageSubmit = 3
        }



        public static List<int> ListNewFixitCampaings = new List<int> { 54, 55, 56, 57, 58, 59, 60 };
        public const int NewFixitResolvedStatusId = 8;

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

        public static double RadiusRangeInKiloMeters = 10;
        public static Dictionary<int, int> CampDistDictionary = new Dictionary<int, int>() { { 18, 8 }, { 35, 1 }, { 36, 1 }, { 39, 1 } };


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

        public const string FacebookPrefixCnic = "FFFFF";
        public const string AnonymousPictureOnlinePath = "https://static.xx.fbcdn.net/rsrc.php/v3/yo/r/UlIqmHJn-SK.gif";


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
    #endregion

    #region from window service
    public partial class Config
    {
        public class ExcutionModel
        {
            public ExcutionSpan ExcutionSpan { get; set; }

            public System.DayOfWeek ExcutionDay { get; set; }

            public TimeSpan ExecutionTime { get; set; }

            public ExcutionModel(ExcutionSpan executionSpan, System.DayOfWeek excutionDay)
            {
                this.ExcutionSpan = executionSpan;
                this.ExcutionDay = excutionDay;
            }
            public ExcutionModel(ExcutionSpan executionSpan)
            {
                this.ExcutionSpan = executionSpan;
            }
        }

        public enum ExcutionSpan
        {
            Daily = 1,
            Weekly = 2,
        }

        public enum UserPermissions
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
            SendMessageStatsOfSpecificCampaign = 10,
            SendMessageStatsOfAllCumulativeCampaign = 11
        }
    }
    #endregion

    #region from public web
    public partial class Config
    {
        public enum PublicComplaintStatus : byte
        {
            Pending = 1,
            Resolved = 2,
            UnsatisfactoryClosed = 3,
            Inapplicable = 4

        }


        public static string AuthenticationType = "ExternalCookie";
        public static string ExternalCookieName = "ExternalSocial";
        public static string FacebookAccessTokenKeyName = "FacebookAccessToken";
        public static string FacebookFirstName = "urn:facebook:first_name";
        public static string FacebookLastName = "urn:facebook:last_name";
        public static string FacebookUserId = "urn:facebook:id";

        public static byte PageDraw = 10;

    }
    #endregion
}
