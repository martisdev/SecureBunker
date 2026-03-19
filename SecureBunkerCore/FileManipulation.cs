using NetCoreFileAccess.Criptography;
using SecureBunkerCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SecureBunkerCore
{
    public static class FileManipulation
    {
        
        #region FIELDS
        
        public static List<DataItems> ListItems = new List<DataItems>();

        #endregion

        #region PRIVATE METHODS

        private static byte[] SerializeDataFile()
        {
            MemoryStream memSteam = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(memSteam))
            {
                XmlSerializer x = new XmlSerializer(ListItems.GetType());
                x.Serialize(writer, ListItems);
                writer.Flush();
                memSteam.Position = 0;
                return memSteam.ToArray();
            }
        }

        private static Boolean DeserializeDataFile(byte[] XmlData)
        {
            try
            {                
                XmlSerializer serializer = new XmlSerializer(ListItems.GetType());
                using (Stream stream = new MemoryStream(XmlData))
                    ListItems = (List<DataItems>)serializer.Deserialize(stream);
                
                return true;
            }
            catch { }
            return false;
        }

        #endregion

        #region PUBLIC METHODS

        public static MemoryStream EncryptDocument(string UserLogin, string PassWordLogin)
        {
            Cryptography.GenerateIV();

            byte[] StrRamdom = Cryptography.GererateRamdomKEY(Cryptography.RAMDOM_LENGTH);

            byte[] UserPlain = Encoding.UTF8.GetBytes(UserLogin.PadRight(Cryptography.USER_LENGTH));
            byte[] UserEncryp = Cryptography.AESEncrypt(UserPlain,
                                Encoding.UTF8.GetBytes(PassWordLogin.PadRight(Cryptography.USER_LENGTH)));

            byte[] XmlData = SerializeDataFile();
            byte[] EncryptData = Cryptography.AESEncrypt(XmlData,
                                Encoding.UTF8.GetBytes(PassWordLogin.PadRight(Cryptography.USER_LENGTH)));

            MemoryStream stream = new MemoryStream();
            stream.Write(StrRamdom, 0, StrRamdom.Length);
            stream.Write(Cryptography.KEY_IV, 0, Cryptography.KEY_IV.Length);

            // write user encrypted data with length prefix
            var userEncryp = UserEncryp;
            var lenBytes = BitConverter.GetBytes(userEncryp.Length);
            stream.Write(lenBytes, 0, lenBytes.Length);
            stream.Write(userEncryp, 0, userEncryp.Length);

            stream.Write(EncryptData, 0, EncryptData.Length);
            stream.Position = 0;
            return stream;
        }

        public static Boolean LoadDocument(MemoryStream stream, string mUser, string mPassword)
        {
            byte[] XmlByte = null;


            stream.Position = Cryptography.RAMDOM_LENGTH;
            byte[] keyIV = new byte[Cryptography.IV_LENGTH];

            int bytesRead = stream.Read(keyIV, 0, keyIV.Length);
            Cryptography.KEY_IV = keyIV;

            byte[] lenBytes = new byte[4];
            stream.Read(lenBytes, 0, 4);
            int userCipherLen = BitConverter.ToInt32(lenBytes, 0);
            byte[] userCipher = new byte[userCipherLen];
            stream.Read(userCipher, 0, userCipherLen);
            byte[] outBytes = Cryptography.AESDecrypt(userCipher,
                Encoding.UTF8.GetBytes(mPassword.PadRight(Cryptography.USER_LENGTH)));

            string UserName = Encoding.UTF8.GetString(outBytes).Trim();

            if (UserName != mUser)
                return false;

            byte[] DataByte = new byte[stream.Length - stream.Position];
            bytesRead = stream.Read(DataByte, 0, DataByte.Length);

            XmlByte = Cryptography.AESDecrypt(DataByte, Encoding.UTF8.GetBytes(mPassword.PadRight(Cryptography.USER_LENGTH)));

            if (XmlByte == null)
                return false;

            DeserializeDataFile(XmlByte);
            return true;
        }

        public static void CreateEmptyDocument(string User, string Password)
        {
            FileManipulation.ListItems.Clear();
            MemoryStream data = FileManipulation.EncryptDocument(User, Password);
            Manager.SaveFile(data);
        }

        #endregion
    }
}
