using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using Amazon.Glacier;

namespace PITB.CMS.Handler.Complaint
{
    public class ExtensionsHandler
    {
        private static List<string> ListExcelExt = new List<string>() 
        { 
            "application/vnd.ms-excel", 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

        private static List<string> ListImageExt = new List<string>()
        {
            //"image"
            "image/g3fax",
            "image/gif",
            "image/ief",
            "image/jpeg",
            "image/tiff",
            "image/png" 
        };


        private static List<string> ListPdfExt = new List<string>() 
        { 
            "application/pdf"
        };

        private static List<string> ListWordExt = new List<string>() 
        { 
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
            "application/vnd.ms-word.document.macroEnabled.12",
            "text/plain"
        };

        private static List<string> ListVideoExt = new List<string>()
        {
            "application/octet-stream"
        };

        public static bool IsContentTypeValid(string contentType, Config.AttachmentExtensionType attachmentExtension)
        {
            string contentTypeLowerCase = contentType.ToLower();
            switch (attachmentExtension)
            {
                case Config.AttachmentExtensionType.Excel:
                    return ListExcelExt.Contains(contentTypeLowerCase);
                break;

                case Config.AttachmentExtensionType.Image:
                    return ListImageExt.Contains(contentTypeLowerCase);
                break;

                case Config.AttachmentExtensionType.Pdf:
                    return ListPdfExt.Contains(contentTypeLowerCase);
                break;

                case Config.AttachmentExtensionType.Word:
                    return ListWordExt.Contains(contentTypeLowerCase);
                break;

                case Config.AttachmentExtensionType.Video:
                    return ListVideoExt.Contains(contentTypeLowerCase);
                break;

                default:
                    break;
            }
          
            return false;
        }

        public static bool IsContentTypeValid(string contentType)
        {
            string contentTypeLowerCase = contentType.ToLower();

            if (ListExcelExt.Contains(contentTypeLowerCase))
            {
                return true;
            }
            else if (ListImageExt.Contains(contentTypeLowerCase))
            {
                return true;
            }
            else if (ListPdfExt.Contains(contentTypeLowerCase))
            {
                return true;
            }
            else if (ListWordExt.Contains(contentTypeLowerCase))
            {
                return true;
            }
            else if (ListVideoExt.Contains(contentTypeLowerCase))
            {
                return true;
            }
            return false;
        }
    }
}