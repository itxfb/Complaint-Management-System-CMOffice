using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.Custom
{
    public class PostModel
    {
        public class File
        {
            public List<Single> ListFiles { get; set; }

            public File()
            {
                
            }

            public File(HttpFileCollection files)
            {
                //List<HttpPostedFileBase> listPostedFile = Utility.GetListHttpPostedFileBase(files);

                this.ListFiles = GetListOfFiles(files);
            }

           /* public File(List<HttpPostedFileBase> listFileBase)
            {
                this.ListFiles = GetListOfFiles(listFileBase);
            }*/

            public File(List<Single> listFile)
            {
                this.ListFiles = listFile;
            }

            public Single GetSingle(string name)
            {
                return this.ListFiles.Where(n => n.FormKey == name).FirstOrDefault();
            }

            private List<Single> GetListOfFiles(HttpFileCollection fileCollectionBase)
            {
                List<HttpPostedFile> listPostedFile = new List<HttpPostedFile>();
                int index = 0;
                ListFiles = new List<Single>();
                foreach (string formFileKey in fileCollectionBase)
                {
                    HttpPostedFile file = fileCollectionBase[index];
                    listPostedFile.Add(fileCollectionBase[index]);

                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtention = Path.GetExtension(file.FileName);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string contentType = file.ContentType;
                        Stream fileStream = file.InputStream;

                        int fileLength = file.ContentLength;
                        byte[] fileData = new byte[fileLength];
                        fileStream.Read(fileData, 0, fileLength);
                        ListFiles.Add(new Single(formFileKey, file.FileName, fileExtention, contentType, file.ContentLength, fileData, Config.AttachmentType.File));
                    }
                    index++;
                }
                return ListFiles;
            }


            public static List<Single> GetListFiles(List<string> listImageBase64Str, string fileName, string fileExtension, string fileContentType, Config.AttachmentType fileType)
            {
                List<Single> listSingle = new List<Single>();
                foreach (string imgStr in listImageBase64Str)
                {
                    Single singleFile = new Single(imgStr, fileName, fileExtension, fileContentType, fileType);
                    listSingle.Add(singleFile);
                }


                return listSingle;
            }

            //private List<Single> GetListOfFiles(List<HttpPostedFileBase> listFileBase)
            //{
            //    ListFiles = new List<Single>();
            //    foreach (HttpPostedFileBase file in listFileBase)
            //    {
            //        string fileName = Path.GetFileName(file.FileName);
            //        string fileExtention = Path.GetExtension(file.FileName);

            //        if (!string.IsNullOrEmpty(fileName))
            //        {
            //            string contentType = file.ContentType;
            //            Stream fileStream = file.InputStream;

            //            int fileLength = file.ContentLength;
            //            byte[] fileData = new byte[fileLength];
            //            fileStream.Read(fileData, 0, fileLength);
            //            ListFiles.Add(new Single(file.FileName, fileName, fileExtention, contentType, file.ContentLength, fileData));
            //        }
            //    }
            //    return ListFiles;
            //}
            public class Single
            {
                
                public string FormKey { get; set; }
                public string Name { get; set; }

                public string Extention { get; set; }

                public string ContentType { get; set; }

                public int ContentLength { get; set; }

                public Config.AttachmentType AttachmentType { get; set; }

                public byte[] FileBytes { get; set; }

                public Single(string formKey, string name, string extension, string contentType, int contentLength, byte[] fileBytes, Config.AttachmentType attachmentType)
                {
                    this.FormKey = formKey;
                    this.Name = name;
                    this.Extention = extension;
                    this.ContentType = contentType;
                    this.ContentLength = contentLength;
                    this.FileBytes = fileBytes;
                    this.AttachmentType = attachmentType;
                }

                public Single(string imageBase64Str, string fileName, string fileExtension, string fileContentType, Config.AttachmentType attachmentType)
                {
                    this.FileBytes = Convert.FromBase64String(imageBase64Str);
                    this.Name = fileName;
                    this.Extention = fileExtension;
                    this.ContentType = fileContentType;
                    this.AttachmentType = attachmentType;
                }

            }
        }
        
        
    }
}