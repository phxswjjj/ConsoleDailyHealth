using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDailyHealth
{
    public class AppConfig
    {
        public DailyHealthApi DailyHealth;
        public UserInfo User;

    }
    public class DailyHealthApi
    {
        public string BaseUrl;
        public string TemplatePath;
        public string AuthBasicBearer;
    }
    public class UserInfo
    {
        public string Account;
        public string Password;
    }
}
