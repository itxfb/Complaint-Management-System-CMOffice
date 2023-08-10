using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PITB.CMS.Helper.License
{
    public class ZLicenseGenrator
    {
        private class licenseClass
        {
            public DateTime dateTime_0;
            public int int_0;
            public int int_1;
            public string string_0;
            public string string_1;
        }

        private static string GetThingsToHash(licenseClass class18_0)
        {
            string format = "{0}{1};{2}{3}{4};{5};{6}";

            return string.Format(format, new object[]
            {
                class18_0.int_1,
                class18_0.string_1,
                class18_0.dateTime_0.Year,
                class18_0.dateTime_0.Month.ToString("00"),
                class18_0.dateTime_0.Day.ToString("00"),
                class18_0.int_0,
                class18_0.string_0
            });
        }
        private static string Amalgamate(licenseClass class18_0)
        {
            return string.Format("{0};{1};{2};{3}", new object[]
            {
                class18_0.int_1,
                class18_0.string_1,
                class18_0.int_0,
                class18_0.string_0
            });
        }

        public static string[] GenerateZzzLicense(string product, string username)
        {
            string[] retS = new string[2];
            //First string is licenseName, second one is LicenseCode

            licenseClass licenseClass = new licenseClass();
            Random random = new Random();
            int month = random.Next(1, 12);
            int day = random.Next(1, 12);
            string text = "40" + month.ToString("00") + day.ToString("00");
            licenseClass.int_0 = random.Next(100, 200);
            licenseClass.string_0 = username.ToUpper();

            licenseClass.int_1 = int.Parse(ZzzProducts.FirstOrDefault(x => x.Value == product).Key);

            licenseClass.dateTime_0 = new DateTime(2040, month, day);

            licenseClass.string_1 = product;

            string str = licenseClass.int_0.ToString();
            str += ";";
            str += licenseClass.int_1.ToString();
            string string_ = licenseClass.string_0;
            string text2 = str + "-" + string_;
            //string thingsToHash = this.GetThingsToHash(licenseClass);
            string thingsToHash = GetThingsToHash(licenseClass);
            string text3 = MakeMD5Hash(thingsToHash).Substring(0, 26);
            retS[0] = text2;
            string text4 = Amalgamate(licenseClass);
            int num = 0;
            char[] array = text4.ToCharArray();
            char[] array2 = array;
            checked
            {
                for (int i = 0; i < array2.Length; i++)
                {
                    char char_ = array2[i];
                    num += GetIntOfChar(char_);
                }
                string text5 = (num * 1981 % 1000000).ToString("000000");
                for (short num2 = 5; num2 != -1; num2 -= 1)
                {
                    int startIndex = Convert.ToInt32(text5[(int)num2].ToString());
                    text3 = text3.Insert(startIndex, System.Convert.ToString(text[(int)num2]));
                }

                retS[1] = text3;
            }

            return retS;
        }

        private static int GetIntOfChar(char char_0)
        {
            return (int)char_0;
        }

        private static string MakeMD5Hash(string input)
        {
            MD5 mD = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] array = mD.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            int arg_29_0 = 0;
            checked
            {
                int num = array.Length - 1;
                for (int i = arg_29_0; i <= num; i++)
                {
                    stringBuilder.Append(array[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }

        private static readonly Dictionary<string, string> ZzzProducts = new Dictionary<string, string>
        {
            {
                "100",
                ".NET EF Extensions - Bundle"
            },
            {
                "101",
                ".NET EF Extensions - SQL Server"
            },
            {
                "102",
                ".NET EF Extensions - SQL Compact"
            },
            {
                "103",
                ".NET EF Extensions - MySQL"
            },
            {
                "104",
                ".NET EF Extensions - SQLite"
            },
            {
                "105",
                ".NET EF Extensions - PostgreSQL"
            },
            {
                "106",
                ".NET EF Extensions - Oracle"
            },
            {
                "107",
                ".NET EF Extensions - Firebird"
            },
            {
                "300",
                ".NET Bulk Operations - Bundle"
            },
            {
                "301",
                ".NET Bulk Operations - SQL Server"
            },
            {
                "302",
                ".NET Bulk Operations - SQL Compact"
            },
            {
                "303",
                ".NET Bulk Operations - MySQL"
            },
            {
                "304",
                ".NET Bulk Operations - SQLite"
            },
            {
                "305",
                ".NET Bulk Operations - PostgreSQL"
            },
            {
                "306",
                ".NET Bulk Operations - Oracle"
            },
            {
                "307",
                ".NET Bulk Operations - Firebird"
            },
            {
                "700",
                "Dapper Plus - Bundle"
            },
            {
                "701",
                "Dapper Plus - SQL Server"
            },
            {
                "702",
                "Dapper Plus - SQL Compact"
            },
            {
                "703",
                "Dapper Plus - MySQL"
            },
            {
                "704",
                "Dapper Plus - SQLite"
            },
            {
                "705",
                "Dapper Plus - PostgreSQL"
            },
            {
                "706",
                "Dapper Plus - Oracle"
            },
            {
                "707",
                "Dapper Plus - Firebird"
            }
        };
    }
}