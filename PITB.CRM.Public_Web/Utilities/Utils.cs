using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web.Utilities
{
    public class Utils
    {
       
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string Encrypt(Int64 int64Integer)
        {
            Hashids hashids = new Hashids("IKTZ");
            return hashids.EncodeLong(int64Integer);
        }

        public static Int64 Decrypt(string encryptedString)
        {
            var hashids = new Hashids("IKTZ");
            return hashids.DecodeLong(encryptedString)[0];
        }

        public static string CalculateTurnRoundTime(TimeSpan span)
        {
            string responseTime = string.Empty;

            if (span.Days > 0)
            {
                if (span.Hours > 0)
                    responseTime = span.Days + " " + "days" + " " + span.Hours + " " + "hour(s)" + "";
                else
                    responseTime = span.Days + " " + "days" + " " + span.Minutes + " " + "minute(s)" + "";

            }
            else if (span.Days <= 0)
            {
                if (span.Hours < 24 && span.Hours > 0)
                {
                    if (span.Minutes >= 0)
                        responseTime = span.Hours + " " + "hour(s)" + " " + span.Minutes + " " + "minute(s)" + "";
                    else
                        responseTime = span.Hours + " " + "hour(s)" + " " + span.Seconds + " " + "second(s)" + "";
                }
                else if (span.Hours <= 0 && span.Minutes >= 0)
                {
                    if (span.Seconds > 0 && span.Minutes < 0)
                        responseTime = span.Seconds + " " + "second(s)" + "";
                    else
                        responseTime = span.Minutes + " " + "minute(s)" + "";
                }
                else if (span.Minutes <= 0 && span.Seconds >= 0)
                {
                    responseTime = span.Seconds + " " + "second(s)" + "";
                }
            }
            return responseTime;
        }
    }
}