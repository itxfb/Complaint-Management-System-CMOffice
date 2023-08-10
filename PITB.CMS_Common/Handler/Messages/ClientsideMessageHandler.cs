using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Enums;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models.View.ClientMesages;

namespace PITB.CMS_Common.Handler.Messages
{
    public class ClientsideMessageHandler
    {
        public static ClientMessage GetUnauthorisedAccessMessage()
        {
            return new ClientMessage("Unauthorise Access", "You are not authorised for this operation", MessageCatalog.DialogType.Warning);
        }
        //public static ClientMessage GetMessageModel(OperationResult result)
        //{
        //    if (result.IsSuccess)
        //    {
        //        return new ClientMessage(
        //                    result.TaskType.GetDisplayName(),
        //                    result.TaskType.GetDisplayName() + " completed successfully",
        //                    MessageCatalog.DialogType.Success, false);

        //    }
        //    return new ClientMessage("ERROR", GetErrorMessageOfEnum(result.TaskType, result.IsSuccess), MessageCatalog.DialogType.Error, false);
        //}
        public static ClientMessage GetMessage(MessageCatalog.Task task, Config.CommandStatus status)
        {
            string title = string.Empty;
            bool isError = false;
            switch (status)
            {
                case Config.CommandStatus.Success:
                    title = "Success";

                    break;
                case Config.CommandStatus.Failure:
                case Config.CommandStatus.Exception:
                    title = "Error";
                    isError = true;
                    break;
            }
            MessageCatalog.DialogType dialog = (isError)
                ? MessageCatalog.DialogType.Error
                : MessageCatalog.DialogType.Success;
            return new ClientMessage(title, GetMessageOfTask(task, isError), dialog);
        }

        private static string GetMessageOfTask(MessageCatalog.Task task, bool isError)
        {
          
            string message = string.Empty;
           // var taskEnum = (Enum)Enum.Parse(typeof(MessageCatalog.Task), task.GetDisplayName());
            message = string.Format(isError ? "Error occured during <b>{0}</b>" : "Task <b>{0}</b> completed successfully", task.GetDisplayName());
            return message;
            //string message;
            //MessageCatalog.EnumErrorMessages.TryGetValue(task, out message);
            //return message;
        }
    }
}