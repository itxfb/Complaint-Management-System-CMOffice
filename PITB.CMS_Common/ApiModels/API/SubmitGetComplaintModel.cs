using System;
using System.Diagnostics;

namespace PITB.CMS_Common.ApiModels.API
{
    public class SubmitGetComplaintModel
    {
        public SubmitGetComplaintModel()
        {
            Filter = Config.FilterTypeApi.MostRecent;
        }

        public string StartDate { get; set; }
        public string EndDate { get; set; }


        

        public string Cnic { get; set; }
        public string Cell { get; set; }
        public int AppId { get; set; }
        public int From { get; set; }
        public string UserProvider { get; set; }
        public string UserId { get; set; }
        public int To { get; set; }
        public Config.FilterTypeApi Filter { get; set; }
        public byte LanguageId { get; set; }
        public int CampaignId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int ComplaintStatus { get; set; }

        public Config.CnicOrSocialPresent CnicOrSocialPresent
        {
            get
            {
                Config.CnicOrSocialPresent cnicOrSocial = Config.CnicOrSocialPresent.None;

                //using (EventLog eventLog = new EventLog("Application"))
                //{
                //    eventLog.Source = "Application";
                //    eventLog.WriteEntry("Checking for both social and cnic", EventLogEntryType.Information, 101, 1);

                    if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(UserProvider) &&
                        !string.IsNullOrEmpty(Cnic))
                    {
                        cnicOrSocial = Config.CnicOrSocialPresent.CnicAndSocial;
                    }
                 //   eventLog.Source = "Application";
                 //   eventLog.WriteEntry("Checking  cnic", EventLogEntryType.Information, 101, 1);

                    if (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(UserProvider) &&
                        !string.IsNullOrEmpty(Cnic))
                    {
                        cnicOrSocial = Config.CnicOrSocialPresent.OnlyCnic;
                    }
                 //   eventLog.Source = "Application";
                  //  eventLog.WriteEntry("Checking for social ", EventLogEntryType.Information, 101, 1);
                    if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(UserProvider) &&
                        string.IsNullOrEmpty(Cnic))
                    {
                        cnicOrSocial = Config.CnicOrSocialPresent.OnlySocial;

                    }

                  //  eventLog.Source = "Application";
                  //  eventLog.WriteEntry("Final selection"+cnicOrSocial.ToString(), EventLogEntryType.Information, 101, 1);
               // }

                return cnicOrSocial;
            }
        }
        public bool IfCnicAndSocialDataIsPresent
        {
            get
            {
                if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(UserProvider) &&
                    !string.IsNullOrEmpty(Cnic))
                    return true;
                else
                    return false;
            }
        }

        public bool IfCnicPresentOnly { get; set; }

        public bool ModelIsValid()
        {
            bool isValid = true;


            if ((string.IsNullOrEmpty(Cnic) && string.IsNullOrEmpty(Cell)) && (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(UserProvider)))
            {

                if (
                    Latitude == 0 ||
                    Longitude == 0
                    )
                    isValid = false;
            }
            if (CampaignId <= 0 || AppId == 0)
                isValid = false;
            if (!string.IsNullOrEmpty(UserId))
                if (string.IsNullOrEmpty(UserProvider))
                    isValid = false;


            return isValid;
        }

    }
}