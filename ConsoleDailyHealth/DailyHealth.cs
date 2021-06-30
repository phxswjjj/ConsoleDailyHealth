using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleDailyHealth
{
    public class DailyHealth
    {
        public string currentAddress { get; set; }
        public string mobileTel { get; set; }
        public bool agreeTheForm { get; set; }
        public DateTime date { get; set; }
        public List<DailyHealthItem> surveyItemValueList { get; set; }
        public bool isNormal { get; set; }

        public static DailyHealth LoadTemplate(string filepath)
        {
            using (var reader = new StreamReader(filepath))
            {
                var s = reader.ReadToEnd();
                return ParseJson(s);
            }
        }
        public static DailyHealth ParseJson(string json)
        {
            var data = JsonConvert.DeserializeObject<DailyHealth>(json);
            return data;
        }
    }
}
