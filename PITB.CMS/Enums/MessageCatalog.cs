using PITB.CMS_Common.Helper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PITB.CMS.Enums
{
    public class MessageCatalog
    {
        public enum DialogType : byte
        {
            Error = 1,
            Success,
            Warning,
            Info,
           // Confirmation
        }
        public enum Task
        {
            [Display(Name = "Adding complaint")]
            ComplaintAdd = 1,
            [Display(Name = "Editing complaint")]
            ComplaintEdit,
            [Display(Name = "Resolving complaint")]
            ComplaintResolve,
            [Display(Name = "Feedback submission")]
            FeedbackAdd,
            [Display(Name = "Reviewing feedback submission")]
            FeedbackEdit,
            [Display(Name = "Change password")]
            ChangePassword,
            [Display(Name = "Update User Profile")]
            UserSettings,
            [Display(Name = "Update User Verification code")]
            UserVerificationCode
        }
      
        public static Dictionary<Enum, string> EnumErrorMessages = new Dictionary<Enum, string>
        {
            {Task.ComplaintAdd, string.Format("Error occured during {0}", Task.ComplaintAdd.GetDisplayName())},
            {Task.ComplaintEdit, string.Format("Error occured during {0}", Task.ComplaintEdit.GetDisplayName())},
            {Task.ComplaintResolve, string.Format("Error occured during {0}", Task.ComplaintResolve.GetDisplayName())},
            {Task.FeedbackAdd, string.Format("Error occured during {0}", Task.FeedbackAdd.GetDisplayName())},
            {Task.FeedbackEdit, string.Format("Error occured during {0}", Task.FeedbackEdit.GetDisplayName())},
        };
        public static string GetEnumMessage(Task enumeration,bool isError)
        {
            string message = string.Empty;
            var taskEnum=(Enum) Enum.Parse(typeof (Task), enumeration.GetDisplayName());
            message = string.Format(isError ? "Error occured during {0}" : "Task of {0} completed successfully", taskEnum.GetDisplayName());
            return message;
        }
       
    }
    public enum CallStatuses
    {
        [Display(Name = "Complete")]
        Complete = 1,
        [Display(Name = "User Busy")]
        Userbusy = 2,
        [Display(Name = "Wrong Number/Person")]
        Wrongnumber = 3,
        [Display(Name = "No response")]
        Noresponse = 4,
    }
}