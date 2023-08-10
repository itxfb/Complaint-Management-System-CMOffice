using System;
using System.Collections.Generic;

namespace PITB.CMS_WindowService
{
    public class Config
    {
        /*
        public enum ServiceType
        {
            SendMsgToDCO = 1,
            MarkComplaintToOriginUserIfPresent = 2,
            SendPendingOverDueSMS = 3,
            SycUserWiseComplaints = 4,
            MainServiceError = 5,
            SyncSchoolDataMain = 6
        }*/
        public const int FunctionRetryCount = 4;

        public class  ExcutionModel
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
        public enum MsgSrcType
        {
            Web = 1,
            Mobile = 2,
            WindowService = 3
        }

        public enum MsgType
        {
            ToStakeholder = 1,
            ToComplainant = 2
        }

        public enum MsgTags
        {
            SchoolEducationStakeholder = 1,
            SchoolEducationStakeholderSupervisor = 2,
            SchoolEducationSecretary = 3,
            SchoolEducationStakeholderUsernameChangeReminder = 4
        }

        public enum PermissionsType
        {
            Campaign = 1,
            User = 2
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
            {8,"Resolved"}
        };

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

        public enum CampaignPermissions
        {
            DontSendMessages = 1,
            ShowWards = 2,
            ShowStakeholderMessagesView = 3,
            HideProfileInfoFromStakeholderDetail = 4,
            
        }
        
    }
}
