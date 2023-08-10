using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using PITB.CMS_Common.Helper.License;

namespace PITB.CMS_Handlers.License
{
    public class LicenseHandler
    {
        public static void InitializeLicense(Config.LicenseType licenseType)
        {
            if (licenseType == Config.LicenseType.ZBulkOperation)
            {
                //ZLicenseGenrator zLicenseGenrator = new ZLicenseGenrator();
                //string[] licenseName = ZLicenseGenrator.GenerateZzzLicense(".NET Bulk Operations - SQL Server", "Zeeshan");
                
                InitializeLicense(".NET Bulk Operations - SQL Server");
            }
        }

        private static void InitializeZBulkOperation()
        {
                        
        }

        public static bool InitializeLicense(string productName /*string[] licenseKeys*/)
        {
            bool hasLicenseValidated = false;

            string randomString = null;
            if (Z.BulkOperations.LicenseManager.ValidateLicense())
            {
                hasLicenseValidated = true;
            }
            else
            {
                for (int i = 0; i < Config.LicenceRetryCount && !hasLicenseValidated; i++)
                {
                    randomString = Utility.GetRandomKey(Config.LicenseStringLength);
                    string[]strArr = ZLicenseGenrator.GenerateZzzLicense(productName, randomString);
                    Z.BulkOperations.LicenseManager.AddLicense(strArr[0], strArr[1]);

                    if (Z.BulkOperations.LicenseManager.ValidateLicense())
                    {
                        hasLicenseValidated = true;
                    }
                       
                }

            }

            return hasLicenseValidated;
        }
    }
}