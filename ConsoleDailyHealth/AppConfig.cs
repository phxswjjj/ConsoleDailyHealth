using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDailyHealth
{
    public class AppConfig
    {
        public DailyHealthApi DailyHealth { get; set; }
        public UserInfo User { get; set; }

    }
    public class DailyHealthApi
    {
        public string BaseUrl { get; set; }
        public string TemplatePath { get; set; }
        public string AuthBasicBearer { get; set; }
        public int RandomDelaySecond { get; set; }
    }
    public class UserInfo
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
