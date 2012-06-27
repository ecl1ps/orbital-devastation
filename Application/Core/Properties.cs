using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (PropertyKey prop in list.Keys.ToArray())
                    if (!String.IsNullOrWhiteSpace(list[prop]))
                        file.WriteLine(prop + "=" + list[prop]);
            }
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
        }

        private void LoadFromFile(String file)
        {
            foreach (String line in File.ReadAllLines(file))
            {
                if ((!String.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
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
                    }
                    catch { }
                }
            }
        }
    }
}
