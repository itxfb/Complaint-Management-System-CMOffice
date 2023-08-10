using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.ApiModels.API
{
    
    // Orignal Model
    public class SubmitComplaintModel
    {
        public string imei_number { get; set; }
        public int campaignID { get; set; }

        public int departmentId { get; set; }
        public int provinceID { get; set; }

        public int districtID { get; set; }

        public int tehsilID { get; set; }

        public int ucID { get; set; }

        public int wardID { get; set; }

        public int categoryID { get; set; }

        public int subCategoryID { get; set; }

        public string comment { get; set; }

        public DateTime date { get; set; }

        public TimeSpan time { get; set; }

        public string lattitude { get; set; }

        public string longitude { get; set; }
        public string remakrs { get; set; }

        // Person Information

        public string cnic { get; set; }

        public string personName { get; set; }

        public string personContactNumber { get; set; }

        public List<Picture> PicturesList { get; set; }

        public string video { get; set; }

        public string videoFileExtension { get; set; }

    }


    public class SubmitSEComplaintModel
    {
        //public string imei_number { get; set; }
        //public int campaignID { get; set; }

        public int schoolId { get; set; }

        public int departmentId { get; set; }

        public int categoryID { get; set; }

        public int subCategoryID { get; set; }

        public string schoolEmisCode { get; set; }

        //public int provinceID { get; set; }

        //public int districtID { get; set; }

        //public int tehsilID { get; set; }

        //public int ucID { get; set; }

        //public int wardID { get; set; }




        public string comment { get; set; }

        //  public DateTime date { get; set; }

        //  public TimeSpan time { get; set; }

        //  public string lattitude { get; set; }

        //  public string longitude { get; set; }
        public string remarks { get; set; }

        // Person Information

        public string cnic { get; set; }

        public int complaintDistrictId { get; set; }

        public int complaintTehsilId { get; set; }

        public int personDistrictId { get; set; }

        public string personName { get; set; }

        public string personContactNumber { get; set; }

        public string personFatherName { get; set; }

        public string personAddress { get; set; }

        public string personEmailAddress { get; set; }

        public int complaintSrc { get; set; }

        public string tagId { get; set; }

        public List<string> listAttachmentUrl { get; set; }

       // public List<Picture> PicturesList { get; set; }

        //  public string video { get; set; }

        // public string videoFileExtension { get; set; }

    }
    

    public class SubmitLWMCComplaintModel
    {
        public string userId { get; set; }
        public string userProvider { get; set; }
        public string imei_number { get; set; }
        public int campaignID { get; set; }

        public int departmentId { get; set; }
        public int provinceID { get; set; }

        public int districtID { get; set; }

        public int tehsilID { get; set; }

        public int ucID { get; set; }

        public int wardID { get; set; }

        public int categoryID { get; set; }

        public int subCategoryID { get; set; }

        public string comment { get; set; }

        public string date { get; set; }

        public DateTime complaintDate
        {
            get
            {
                return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public TimeSpan time { get; set; }

        public string lattitude { get; set; }

        public string longitude { get; set; }
        public string remakrs { get; set; }

        // Person Information

        public string cnic { get; set; }

        public string personName { get; set; }

        public string personContactNumber { get; set; }

        public List<Picture> PicturesList { get; set; }

        public string video { get; set; }

        public string videoFileExtension { get; set; }
        public string locationArea { get; set; }

        public bool ModelIsValid()
        {
            bool isValid = true;
            if (//string.IsNullOrEmpty(imei_number) ||
                 campaignID <= 0 || categoryID <= 0 || subCategoryID <= 0
                || string.IsNullOrEmpty(longitude) ||
                    string.IsNullOrEmpty(lattitude) || PicturesList == null)
                return false;

            //If all fields are empty then not valid
            if (string.IsNullOrEmpty(userId) &&
                string.IsNullOrEmpty(userProvider) &&
                string.IsNullOrEmpty(personContactNumber) &&
                string.IsNullOrEmpty(personContactNumber))
            {
                isValid = false;
            }




            if (!string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userProvider))
            {
                isValid = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(cnic) && string.IsNullOrEmpty(personContactNumber))
                    isValid = false;
            }


            return isValid;
        }
    }
}