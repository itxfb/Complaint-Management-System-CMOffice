namespace PITB.CMS.Models.DB
{
    using Helper;
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Person_Information")]
    public class DbPersonInformation
    {
        [Key]
        public int Person_id { get; set; }

        [StringLength(100)]
        public string Person_Name { get; set; }

        [StringLength(100)]
        public string Person_Father_Name { get; set; }

        [StringLength(16)]
        public string Cnic_No { get; set; }

        [StringLength(20)]
        public string Account_No { get; set; }

        public Config.Gender? Gender { get; set; }

        [StringLength(15)]
        public string Mobile_No { get; set; }

        [StringLength(15)]
        public string Secondary_Mobile_No { get; set; }

        [StringLength(10)]
        public string LandLine_No { get; set; }

        [StringLength(250)]
        public string Person_Address { get; set; }

        [StringLength(30)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Nearest_Place { get; set; }

        public int? Province_Id { get; set; }

        public int? Division_Id { get; set; }

        public int? District_Id { get; set; }

        public int? Tehsil_Id { get; set; }

        public int? Town_Id { get; set; }

        public int? Uc_Id { get; set; }

        public bool? Is_Cnic_Present { get; set; }

        public virtual DbDistrict District { get; set; }
        public virtual DbProvince Province{ get; set; }

        public bool? Is_Active { get; set; }

        #region HelperMethods

        public static DbPersonInformation GetPersonInformationByPersonId(int personId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
               

                    //var ss = (from p in db.DbPersonalInfo.AsNoTracking()
                    //    join d in db.DbDistricts.AsNoTracking()
                    //        on p.District_Id equals d.District_Id
                    //    where p.Person_id == personId
                    //    select p).FirstOrDefault();
                    return db.DbPersonalInfo.FirstOrDefault(m => m.Person_id == personId);
                
             

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbPersonInformation GetById(int id, DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                return
                    db.DbPersonalInfo.AsNoTracking()
                        .Where(m => m.Person_id==id)
                        .FirstOrDefault();


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbPersonInformation GetByCnic(string cnic)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return
                    db.DbPersonalInfo.AsNoTracking()
                        .Where(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();


            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPersonInformation> GetListByCnic(string cnic)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                    return
                        db.DbPersonalInfo.AsNoTracking()
                            .Where(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPersonInformation> GetListByMobileNo(string mobileNo)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbPersonalInfo
                        .AsNoTracking()
                        .Where(m => (!string.IsNullOrEmpty(m.Mobile_No) && m.Mobile_No.Equals(mobileNo,StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(m.Secondary_Mobile_No) && m.Secondary_Mobile_No.Equals(mobileNo, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPersonInformation> GetListBySecondaryMobileNo(string secondaryMobileNo)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbPersonalInfo
                        where p.Secondary_Mobile_No == secondaryMobileNo
                        select p).ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPersonInformation> GetListByComplaintId(string complaintId)
        {
            try
            {
                KeyValuePair<int,int> campaignAndComplaintIdKeyValuePair = UtilityExtensions.GetCampaignAndComplaintId(complaintId);
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbPersonalInfo
                        join c in db.DbComplaints
                        on p.Person_id equals c.Person_Id
                        where c.Compaign_Id == campaignAndComplaintIdKeyValuePair.Key && c.Id == campaignAndComplaintIdKeyValuePair.Value
                        select p).ToList();

            }
            catch (Exception ex)
            {
                throw ;
            }
        }

        public static DbPersonInformation GetByCnicAndMobileNo(string cnic, string mobileNo, DBContextHelperLinq db)
        {
            try
            {
                return
                    db.DbPersonalInfo.AsNoTracking().FirstOrDefault(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase) && m.Mobile_No == mobileNo);


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbPersonInformation GetByCnicAndMobileNo(string cnic, string mobileNo)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    return db.DbPersonalInfo.AsNoTracking()
                        .FirstOrDefault(
                            m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase) && m.Mobile_No == mobileNo);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
