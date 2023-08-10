//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Web;
//using System.Web.Helpers;
//using Microsoft.SqlServer.Server;
//using PITB.CRM_API.Handlers.Authentication;
//using PITB.CRM_API.Models.API.Authentication;
//using PITB.CRM_API.Models.Authentication;
//using System.IO;

//namespace PITB.CRM_API.Handlers.Business
//{
//    public class BlToken
//    {
//        public static ApiToken.Response GetToken(string jsonStr, string headers)
//        {
//            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonStr);
//            ApiToken.Response apiTokenResponse = new ApiToken.Response();

//            string strToEncript = jsonStr + headers;

//            if (authModel.IsAuthenticated)
//            {
//                string keyForEncr = GetASPNET20machinekey();
//                string encrValue = Encrypt(strToEncript, keyForEncr);
//                //apiTokenResponse.TokenSecret = 
//            }
//            else
//            {
//                apiTokenResponse.TokenSecret = null;
//                apiTokenResponse.TokenValue = null;
//                apiTokenResponse.Status = Config.ResponseMessages[Config.ResponseType.AuthenticationFailed];
//                apiTokenResponse.Message = Config.ResponseMessages[Config.ResponseType.AuthenticationFailed];
//            }
//            return null;
//        }

//        public static string TestValues()
//        {
//            for (int i = 0; i < 10; i++)
//            {
//                var keyConfig = GetASPNET20machinekey();
//                Console.WriteLine(keyConfig);
//            }
//            return null;
//        }


//        public static string GetRandomKey(int bytelength)
//        {
//            byte[] buff = new byte[bytelength];
//            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
//            rng.GetBytes(buff);
//            StringBuilder sb = new StringBuilder(bytelength * 2);
//            for (int i = 0; i < buff.Length; i++)
//                sb.Append(string.Format("{0:X2}", buff[i]));
//            return sb.ToString();
//        }

//        public static string GetASPNET20machinekey()
//        {
//            StringBuilder aspnet20machinekey = new StringBuilder();
//            string key64byte = GetRandomKey(64);
//            string key32byte = GetRandomKey(32);
//            aspnet20machinekey.Append("<machineKey \n");
//            aspnet20machinekey.Append("validationKey=\"" + key64byte + "\"\n");
//            aspnet20machinekey.Append("decryptionKey=\"" + key32byte + "\"\n");
//            aspnet20machinekey.Append("validation=\"SHA1\" decryption=\"AES\"\n");
//            aspnet20machinekey.Append("/>\n");
//            return aspnet20machinekey.ToString();
//        }

//        private const string initVector = "tu89geji340t89u2";

//        private const int keysize = 512;

//        public static string Encrypt(string Text, string Key)
//        {
//            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
//            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Text);
//            PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null);
//            byte[] keyBytes = password.GetBytes(keysize / 8);
//            RijndaelManaged symmetricKey = new RijndaelManaged();
//            symmetricKey.Mode = CipherMode.CBC;
//            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
//            MemoryStream memoryStream = new MemoryStream();
//            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
//            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
//            cryptoStream.FlushFinalBlock();
//            byte[] Encrypted = memoryStream.ToArray();
//            memoryStream.Close();
//            cryptoStream.Close();
//            return Convert.ToBase64String(Encrypted);
//        }

//        public static string Decrypt(string EncryptedText, string Key)
//        {
//            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
//            byte[] DeEncryptedText = Convert.FromBase64String(EncryptedText);
//            PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null);
//            byte[] keyBytes = password.GetBytes(keysize / 8);
//            RijndaelManaged symmetricKey = new RijndaelManaged();
//            symmetricKey.Mode = CipherMode.CBC;
//            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
//            MemoryStream memoryStream = new MemoryStream(DeEncryptedText);
//            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
//            byte[] plainTextBytes = new byte[DeEncryptedText.Length];
//            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
//            memoryStream.Close();
//            cryptoStream.Close();
//            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
//        }
//    }
//}