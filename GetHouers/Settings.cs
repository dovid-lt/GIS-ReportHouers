using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetHouers
{
    internal class Settings
    {
        public Settings()
        {
            
        }

        const string FILE_SETT = "settings.json";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FieldNameType { get; set; }
        public string FileName { get; set; }
        public DateTime TakeAfter { get; set; }
        
        public void Save()
        {
            var str = System.Text.Json.JsonSerializer.Serialize(this);
            File.WriteAllText(FILE_SETT, str);
        }
        public static Settings Load()
        {
            if(File.Exists(FILE_SETT))
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText(FILE_SETT)) ?? new Settings();
                }
                catch (Exception)
                {
                }

            return new Settings();
        }
    }
}
