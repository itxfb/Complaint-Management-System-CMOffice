using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models.View;

namespace PITB.CMS_Common.Models.Custom.Police
{
    public class PoliceModel
    {
        public class AddAction
        {
            public CustomForm.CustomForm.Post PostedData { get; set; }

            public Config.SourceType SourceType { get; set; }

            public Config.OtherSystemId OtherSystemId { get; set; }

            public CMSCookie Cookie { get; set; }

            public DateTime CreatedDateTime { get; set; }


            public string ComplaintId { get; set; }

            public AddAction()
            {
                
            }

            public AddAction(CMSCookie cmsCookie, CustomForm.CustomForm.Post postedData, DateTime createdDateTime, Config.SourceType sourceType, string complaintId)
            {
                this.PostedData = postedData;
                this.Cookie = cmsCookie;
                this.SourceType = sourceType;
                //this.OtherSystemId = otherSystemType;
                this.ComplaintId = complaintId;
                this.CreatedDateTime = createdDateTime;
            }
        }

        public class AddFeedback
        {
            public CustomForm.CustomForm.Post PostedData { get; set; }

            public Config.SourceType SourceType { get; set; }

            public Config.OtherSystemId OtherSystemId { get; set; }

            public CMSCookie Cookie { get; set; }

            public DateTime CreatedDateTime { get; set; }

            //public PostModel.File ListPostedFiles { get; set; }

            public string ComplaintId { get; set; }

            public AddFeedback()
            {

            }

            public AddFeedback(CMSCookie cmsCookie, CustomForm.CustomForm.Post postedData, DateTime createdDateTime, Config.SourceType sourceType, string complaintId)
            {
                this.PostedData = postedData;
                //this.PostedData.postedFiles = new PostModel.File(files);
                this.Cookie = cmsCookie;
                this.SourceType = sourceType;
                //this.OtherSystemId = otherSystemType;
                this.ComplaintId = complaintId;
                this.CreatedDateTime = createdDateTime;
            }
        }


        public class AddComplaint
        {
            public VmAddComplaint VmAddComplaint { get; set; }
            //public HttpFileCollectionBase Files { get; set; }

            //public List<HttpPostedFileBase> ListPostedFile { get; set; }
            //public NameValueCollection FormCollection { get; set; }

            public CMSCookie Cookie { get; set; }

            public DateTime CreatedDateTime { get; set; }

            public PostModel.File ListPostedFiles { get; set; }

            public Dictionary<string,string> FormCollectionDict { get; set; }
            public bool IsProfileEditing { get; set; }
            public bool IsComplaintEditing { get; set; }

            public Config.SourceType SourceType { get; set; }

            public Config.OtherSystemId OtherSystemId { get; set; }

            public string ComplaintId { get; set; }

            public AddComplaint()
            {
                
            }

            public AddComplaint(CMSCookie cmsCookie, DateTime createdDateTime, Config.SourceType sourceType, VmAddComplaint vmAddComplaint, HttpFileCollection files, NameValueCollection formCollection,
                bool isProfileEditing, bool isComplaintEditing)
            {


                //List<HttpPostedFileBase> listPostedFile = new List<HttpPostedFileBase>();
                //foreach (string fileName in files)
                //{
                //    HttpPostedFileBase file = files[fileName];
                //    listPostedFile.Add(files[fileName]);
                //}
                //this.ListPostedFile = Utility.GetListHttpPostedFileBase(files);//listPostedFile;
                this.Cookie = cmsCookie;
                this.CreatedDateTime = createdDateTime;
                this.FormCollectionDict = Utility.GetDictionary(formCollection);
                this.VmAddComplaint = vmAddComplaint;
                this.ListPostedFiles = new PostModel.File(files);
                this.SourceType = sourceType;
                //this.Files = files;
                //this.FormCollection = formCollection;
                this.IsProfileEditing = IsProfileEditing;
                this.IsComplaintEditing = IsComplaintEditing;
            }
        }
    }
}