using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AzDOAddIn
{
    internal static class PatHelper
    {
        private static string GetKey()
        {
            string mName = Environment.MachineName;
            string key = "";

            for (int i = 0; i < 32; i++)
            {
                double code = 0;

                if (i < mName.Length) code = char.ConvertToUtf32(mName, i);
                else code = i;

                key += code.ToString();
            }

            return key.Substring(0, 32);
        }

        internal static string GetPat(string url)
        {
            if (Properties.Settings.Default.PATManager == "") return "";

            Pats pats = DecryptPats();

            foreach (var pat in pats.Dictionary) if (pat.Key == url) return pat.Value;

            return "";
        }

        private static Pats DecryptPats()
        {
            string key = GetKey();

            XmlSerializer serializer = new XmlSerializer(typeof(Pats));

            string decstr = DecryptString(key, Properties.Settings.Default.PATManager);

            StringReader reader = new StringReader(decstr);

            Pats pats = (Pats)serializer.Deserialize(reader);
            return pats;
        }

        internal static List<string> GetStoredUrls()
        {
            List<string> listUrls = new List<string>();

            if (Properties.Settings.Default.PATManager != "")
                        {
                Pats pats = DecryptPats();

                if (pats.Dictionary.Count > 0)
                    foreach (var dict in pats.Dictionary) listUrls.Add(dict.Key);
            }

            return listUrls;
        }

        internal static string SetPat(string url, string pat)
        {
            string key = GetKey();

            Pats pats = null;

            if (Properties.Settings.Default.PATManager != "")
            {
                pats = DecryptPats();

                for (int i = 0; i < pats.Dictionary.Count; i++)
                {
                    if (pats.Dictionary[i].Key == url && pats.Dictionary[i].Value != pat)
                    {
                        pats.Dictionary.RemoveAt(i);

                        pats.Dictionary = new List<KeyValuePair<string, string>>();
                        pats.Dictionary.Add(new KeyValuePair<string, string>() { Key = url, Value = pat });

                        break;
                    }
                }
            }
            else
            {
                pats = new Pats();

                pats.Dictionary = new List<KeyValuePair<string, string>>();
                pats.Dictionary.Add(new KeyValuePair<string, string>() { Key = url, Value = pat });                
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Pats));

            string PatsStr = "";

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, pats);
                PatsStr = writer.ToString();
            }

            Properties.Settings.Default.PATManager = EncryptString(key, PatsStr);            

            Properties.Settings.Default.Save();

            return "";
        }

        private static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class Pats
    {
        [XmlElement("StringDictionary")]
        public List<KeyValuePair<string, string>> Dictionary;
    }

    [Serializable]
    [XmlType(TypeName = "StringDictionary")]
    public struct KeyValuePair<K, V>
    {
        public K Key
        { get; set; }

        public V Value
        { get; set; }
    }
}
