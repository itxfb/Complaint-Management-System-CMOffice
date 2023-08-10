using System.IO;
using System.Linq;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Complaints_SubType")]
    public class DbComplaintSubType
    {
        [Key]
        [Column("Id")]
        public int Complaint_SubCategory { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? Complaint_Type_Id { get; set; }

        public double? Retaining_Hours { get; set; }

        public string Url { get; set; }

        public string Color_Code { get; set; }
        public string Url_Urdu { get; set; }


        [NotMapped]
        public string Picture { get; set; }

        [NotMapped]
        public string SubCategory_UrduName { get; set; }

        public bool Is_Active { get; set; }

        public string TagId { get; set; }

        #region Helpers

        public static DbComplaintSubType GetById(int id)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.Where(n => n.Complaint_SubCategory == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintSubType> All()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().OrderBy(m=>m.Name).ToList();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public static List<DbComplaintSubType> GetByComplaintType(int complaintType)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().Where(m => m.Complaint_Type_Id == complaintType && m.Is_Active).OrderBy(m=>m.Name).ToList();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public static List<DbComplaintSubType> GetByComplaintTypes(List<int> listComplaintTypes)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().Where(m => listComplaintTypes.Contains((int)m.Complaint_Type_Id) && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static float? GetRetainingByComplaintSubTypeId(int complaintSubType)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    double? retainingHours = db.DbComplaints_SubType.AsNoTracking().Where(m => m.Complaint_SubCategory == complaintSubType && m.Is_Active).FirstOrDefault().Retaining_Hours;
                    if (retainingHours != null)
                    {
                        return (float?)retainingHours;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbComplaintSubType> PopulateImageInBase64FromComplaintsSubtype(List<DbComplaintSubType> listComplaintSubtype)
        {
            try
            {
                foreach (DbComplaintSubType dbComplaintSubType in listComplaintSubtype)
                {
                    if (!string.IsNullOrEmpty(dbComplaintSubType.Url))
                    {
                        dbComplaintSubType.Picture = Utility.GetBase64FromUrl(dbComplaintSubType.Url);
                       // byte[] bytes = Convert.FromBase64String(dbComplaintSubType.Picture);

                        //System.Drawing.Image image;
                        //using (MemoryStream ms = new MemoryStream(bytes))
                        //{
                        //    image = System.Drawing.Image.FromStream(ms);
                        //    //image.Save(string.Format("E:\\Categories\\{0}.jpg",dbComplaintSubType.Name));
                        //}
                    }
                    else
                    {
                        dbComplaintSubType.Picture =  ""; 
                    }
                }
                return listComplaintSubtype;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
