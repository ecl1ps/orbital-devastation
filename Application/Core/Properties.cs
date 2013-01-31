using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Globalization;

namespace Orbit.Core
{
    public class Properties
    {
        private Dictionary<PropertyKey, String> list = new Dictionary<PropertyKey, String>();
        private String filename;

        public Properties(String file)
        {
            Reload(file);
        }

        public Properties(String file, Dictionary<PropertyKey, String> defaultValues)
        {
            Reload(file, defaultValues);
        }

        public String Get(PropertyKey field, String defValue)
        {
            return (Get(field) == null) ? (defValue) : (Get(field));
        }

        public String Get(PropertyKey field)
        {
            return (list.ContainsKey(field)) ? (list[field]) : (null);
        }

        public void Set(PropertyKey field, Object value)
        {
            if (!list.ContainsKey(field))
                list.Add(field, value is string ? value as string : value.ToString());
            else
                list[field] = value is string ? value as string : value.ToString();
        }

        public void SetAndSave(PropertyKey field, Object value)
        {
            Set(field, value);
            Save();
        }

        public void Save()
        {
            Save(filename);
        }

        public void Save(String filename)
        {
            this.filename = filename;

            Set(PropertyKey.PROPERTY_CHECK, ComputeCRC());

            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (PropertyKey prop in list.Keys.ToArray())
                    if (!String.IsNullOrWhiteSpace(list[prop]))
                        file.WriteLine(prop + "=" + list[prop].ToString(CultureInfo.InvariantCulture));
            }
        }

        private string ComputeCRC()
        {
            string hashString = Get(PropertyKey.PLAYER_NAME) + Get(PropertyKey.PLAYER_HASH_ID) + Get(PropertyKey.PLAYER_HIGHSCORE_QUICK_GAME) +
                  Get(PropertyKey.PLAYER_HIGHSCORE_SOLO1) + Get(PropertyKey.PLAYER_HIGHSCORE_SOLO2) + Get(PropertyKey.PLAYER_HIGHSCORE_SOLO3) +
                  Get(PropertyKey.PLAYER_HIGHSCORE_SOLO4) + Get(PropertyKey.PLAYER_HIGHSCORE_SOLO5) +
                  Get(PropertyKey.AVAILABLE_COLORS) + Get(PropertyKey.CHOSEN_COLOR) + SharedDef.SALT;

            SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(hashString));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        public void Reload()
        {
            Reload(filename);
        }

        public void Reload(String filename, Dictionary<PropertyKey, String> defaultValues = null)
        {
            this.filename = filename;
            if (defaultValues != null)
                list = defaultValues;

            if (File.Exists(filename))
                LoadFromFile(filename);

            if (!CheckCRC())
            {
                list = GameProperties.GetDefaultConfigValues();
                Save();
            }
        }

        private bool CheckCRC()
        {
            return ComputeCRC().Equals(Get(PropertyKey.PROPERTY_CHECK));
        }

        private void LoadFromFile(String file)
        {
            foreach (String line in File.ReadAllLines(file))
            {
                if ((!String.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (line.Contains('=')))
                {
                    int index = line.IndexOf('=');
                    String key = line.Substring(0, index).Trim();
                    String value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
                        PropertyKey k = (PropertyKey)Enum.Parse(typeof(PropertyKey), key);
                        if (list.ContainsKey(k))
                            list[k] = value;
                        else
                            list.Add(k, value);

                        if (k == PropertyKey.PLAYER_NAME && value.Length > 10)
                            Reload("", GameProperties.GetDefaultConfigValues());
                    }
                    catch { }
                }
            }
        }
    }
}
