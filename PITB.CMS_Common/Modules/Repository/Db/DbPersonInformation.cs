using PITB.CMS_Common.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace PITB.CMS_Common.Models
{

    public partial class DbPersonInformation
    {
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
                        .Where(m => m.Person_id == id)
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
                    .Where(m => (!string.IsNullOrEmpty(m.Mobile_No) && m.Mobile_No.Equals(mobileNo, StringComparison.OrdinalIgnoreCase)) ||
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
                string[] strArr = complaintId.Split('-');
                //KeyValuePair<int, int> campaignAndComplaintIdKeyValuePair = UtilityExtensions.GetCampaignAndComplaintId(complaintId);
                DBContextHelperLinq db = new DBContextHelperLinq();
                //int[] intArr = Array.ConvertAll(strArr, s => int.Parse(s));
                int campaignId = int.Parse(strArr[0]);
                int cId = strArr.Length==2 ?  int.Parse(strArr[1]) : int.Parse(strArr[0]);

                if (strArr.Length==2)
                {
                    return (from p in db.DbPersonalInfo
                            join c in db.DbComplaints
                            on p.Person_id equals c.Person_Id
                            where c.Compaign_Id == campaignId && c.Id == cId
                            select p).ToList();

                }
                else if (strArr.Length == 1)
                {
                    return (from p in db.DbPersonalInfo
                            join c in db.DbComplaints
                            on p.Person_id equals c.Person_Id
                            where  c.Id == cId
                            select p).ToList();

                }
                return new List<DbPersonInformation>();
            }
            catch (Exception ex)
            {
                return new List<DbPersonInformation>();
                //throw;
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

    #region From API
    public partial class DbPersonInformation
    {
        //public static DbPersonInformation GetPersonInformationByPersonId(int personId)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();


        //        //var ss = (from p in db.DbPersonalInfo.AsNoTracking()
        //        //    join d in db.DbDistricts.AsNoTracking()
        //        //        on p.District_Id equals d.District_Id
        //        //    where p.Person_id == personId
        //        //    select p).FirstOrDefault();
        //        return db.DbPersonalInfo.FirstOrDefault(m => m.Person_id == personId);



        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        //public static DbPersonInformation GetByCnic(string cnic)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();
        //        return
        //            db.DbPersonalInfo.AsNoTracking()
        //                .Where(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase))
        //                .FirstOrDefault();


        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
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

        //public static DbPersonInformation GetByCnicAndMobileNo(string cnic, string mobileNo)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();
        //        return
        //            db.DbPersonalInfo.AsNoTracking().FirstOrDefault(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase) && m.Mobile_No == mobileNo);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

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
        //public static DbPersonInformation GetByCnicAndMobileNo(string cnic, string mobileNo, DBContextHelperLinq db)
        //{
        //    try
        //    {
        //        return
        //            db.DbPersonalInfo.AsNoTracking().FirstOrDefault(m => m.Cnic_No.Equals(cnic, StringComparison.OrdinalIgnoreCase) && m.Mobile_No == mobileNo);


        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        //public static List<DbPersonInformation> GetListByMobileNo(string mobileNo)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();
        //        return db.DbPersonalInfo
        //            .AsNoTracking()
        //            .Where(m => m.Mobile_No.Equals(mobileNo, StringComparison.OrdinalIgnoreCase) ||
        //                m.Secondary_Mobile_No.Equals(mobileNo, StringComparison.OrdinalIgnoreCase))
        //            .ToList();


        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        //public static List<DbPersonInformation> GetListBySecondaryMobileNo(string secondaryMobileNo)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();
        //        return (from p in db.DbPersonalInfo
        //                where p.Secondary_Mobile_No == secondaryMobileNo
        //                select p).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static List<DbPersonInformation> GetListByComplaintId(string complaintId)
        //{
        //    try
        //    {
        //        KeyValuePair<int, int> campaignAndComplaintIdKeyValuePair = UtilityExtensions.GetCampaignAndComplaintId(complaintId);
        //        DBContextHelperLinq db = new DBContextHelperLinq();
        //        return (from p in db.DbPersonalInfo
        //                join c in db.DbComplaints
        //                on p.Person_id equals c.Person_Id
        //                where c.Compaign_Id == campaignAndComplaintIdKeyValuePair.Key && c.Id == campaignAndComplaintIdKeyValuePair.Value
        //                select p).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

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
                        .FirstOrDefault(m => m.External_Provider == externalProvider && m.External_User_ID == userId && m.Cnic_No == cnic);
                return (info == null) ? -1 : info.Person_id;

            }
        }
    }
    #endregion
}
