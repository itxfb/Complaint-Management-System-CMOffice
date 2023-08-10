using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_WindowService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            InitializeLicense();
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            //culture.DateTimeFormat.ShortDatePattern = "MM-dd-yyyy";
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            culture.DateTimeFormat.LongTimePattern = "";
            //Thread.CurrentThread.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            #if DEBUG
                Service1 mySevice = new Service1();
                mySevice.OnDebug();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            #else

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new Service1() 
                };
                ServiceBase.Run(ServicesToRun);
            #endif
        }

        public static void InitializeLicense()
        {
            string licenseName = "7;300-ZEESHAN";//... PRO license name
            string licenseKey = "580760998D23E693E1A2967076DEB86C";//... PRO license key


            if (Z.BulkOperations.LicenseManager.ValidateLicense())
            {
                //Z.BulkOperations

            }
            else
            {
                Z.BulkOperations.LicenseManager.AddLicense(licenseName, licenseKey);
              
            }
        }
    }
}
