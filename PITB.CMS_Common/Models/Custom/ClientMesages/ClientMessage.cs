using PITB.CMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.ClientMesages
{
    public class ClientMessage
    {
        public string Title { get;  set; }
        public string Message { get; set; }
        public MessageCatalog.DialogType DialogType { get; set; }
        public bool NeedConfirmation { get; set; }
        public bool IsSuccess { get; set; }

        public bool CanShow { get; set; }


        public ClientMessage()
        {
            Title = "Success";
            Message = "Success";
            CanShow = true;
        }

        public ClientMessage(string message, bool isSuccess)
        {
            Title = (isSuccess) ? "Success" : "Error";
            Message = message;
            IsSuccess = isSuccess;
            CanShow = true;
        }
       
        public ClientMessage(string title, string message, MessageCatalog.DialogType dialogType = MessageCatalog.DialogType.Info)
        {
            Title = title;
            Message = message;
            DialogType = dialogType;
            CanShow = true;
        }
        public ClientMessage(string title, string message, MessageCatalog.DialogType dialogType, bool needConfirmation)
        {
            Title = title;
            Message = message;
            DialogType = dialogType;
            NeedConfirmation = needConfirmation;
            CanShow = true;
        }
    }
}