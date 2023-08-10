using System.Linq.Expressions;

namespace PITB.CRM_API.Models.DB
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

        public byte? Gender { get; set; }

        [StringLength(50)]
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

        [StringLength(50)]
        public string Ward_Id { get; set; }

        public DateTime? Created_Date { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Created_By { get; set; }

        public int? Updated_By { get; set; }

        [StringLength(50)]
        public string Imei_No { get; set; }

        public int? RegisterType { get; set; }

        public bool? IsEditable { get; set; }

        [StringLength(100)]
        public string Category_of_Seats { get; set; }

        [StringLength(50)]
        public string Party_Affiliation { get; set; }
        public string External_Provider { get; set; }
        public string External_User_ID { get; set; }

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
        public static DbPersonInformation GetByCnic(string cnic, string userId, string userProvider, DBContextHelperLinq db = null)
        {
            try
            {
                Expression<Func<DbPersonInformation, bool>> wherePredicate;

                if (!string.IsNullOrEmpty(cnic) && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userProvider))
                {
                    //When user logged in with CNIC only
                    wherePredicate = m => m.Cnic_No == cnic;
                }

                else if (!string.IsNullOrEmpty(userProvider) && !string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(cnic))
                {
                    //WHen user logged in with Social Only
                    wherePredicate =
                        m =>
                            m.External_User_ID == userId &&
                            m.External_Provider == userProvider;
                }
                else
                {
                    //When user logged in with both
                    wherePredicate =
                        m =>
                            (m.Cnic_No == cnic &&
                            (m.External_Provider == userProvider &&
                             m.External_User_ID == userId));
                }

                if (db == null)
                {
                    db = new DBContextHelperLinq();
                }
                return
                    db.DbPersonalInfo
                        .FirstOrDefault(wherePredicate);

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
                DBContextHelperLinq db = new DBContextHelperLinq();
                return
                    db.DbPersonalInfo.AsNoTracking().FirstOrDefault(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase) && m.Mobile_No == mobileNo);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbPersonInformation GetByExternalUserId(string userId, string userProvider, DBContextHelperLinq db = null)
        {
            try
            {
                if (db == null)
                {
                    db = new DBContextHelperLinq();
                }
                return
                    db.DbPersonalInfo.AsNoTracking().FirstOrDefault(m => m.External_User_ID == userId && m.External_Provider == userProvider);


            }
            catch (Exception ex)
            {
                throw;
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


        public static List<DbPersonInformation> GetListByMobileNo(string mobileNo)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbPersonalInfo
                    .AsNoTracking()
                    .Where(m => m.Mobile_No.Equals(mobileNo, StringComparison.OrdinalIgnoreCase) ||
                        m.Secondary_Mobile_No.Equals(mobileNo, StringComparison.OrdinalIgnoreCase))
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
                KeyValuePair<int, int> campaignAndComplaintIdKeyValuePair = UtilityExtensions.GetCampaignAndComplaintId(complaintId);
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbPersonalInfo
                        join c in db.DbComplaints
                        on p.Person_id equals c.Person_Id
                        where c.Compaign_Id == campaignAndComplaintIdKeyValuePair.Key && c.Id == campaignAndComplaintIdKeyValuePair.Value
                        select p).ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void InsertDbPersonInformation(DbPersonInformation personInformation)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                db.DbPersonalInfo.Add(personInformation);
                db.SaveChanges();
            }

        }

        public static int GetPersonIdByUserId(string userId, string externalProvider)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                DbPersonInformation info =
                    db.DbPersonalInfo.AsNoTracking()
                        .FirstOrDefault(m => m.External_Provider == externalProvider && m.External_User_ID == userId);
                return (info == null) ? -1 : info.Person_id;

            }
        }

        public static int GetPersonIdByUserId(string userId, string externalProvider, string cnic)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                DbPersonInformation info =
                    db.DbPersonalInfo.AsNoTracking()
                        .FirstOrDefault(m => m.External_Provider == externalProvider && m.External_User_ID == userId && m.Cnic_No==cnic);
                return (info == null) ? -1 : info.Person_id;

            }
        }

        #endregion
    }
}
