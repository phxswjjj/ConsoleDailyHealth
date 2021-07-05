using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace ConsoleDailyHealth
{
    class Program
    {
        static AppConfig _Settings;

        static void Main(string[] args)
        {
            LoadSettings();

            var delaySec = new Random().Next(_Settings.DailyHealth.RandomDelaySecond);
            Console.WriteLine($"Wait {delaySec} sec..");
            Thread.Sleep(delaySec * 1000);

            var token = GetToken();
            Console.WriteLine($"token: {token}");

            Console.WriteLine();
            SaveDailyHealth(token);

            Console.WriteLine();
            Console.WriteLine("press [ENTER] to continue..");
            //Console.ReadLine();
        }

        static void LoadSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var settings = config.Get<AppConfig>();
            _Settings = settings;
        }

        static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 3);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json, text/plain, */*");

            client.DefaultRequestHeaders.AcceptEncoding.Clear();
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");

            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7,zh-CN;q=0.6");

            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");

            client.BaseAddress = new Uri(_Settings.DailyHealth.BaseUrl);
            return client;
        }

        static string GetToken()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _Settings.DailyHealth.AuthBasicBearer);

            var formData = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("username", _Settings.User.Account),
                new KeyValuePair<string, string>("password", _Settings.User.Password),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("scope", "health"),
            });

            var resp = client.PostAsync("oauth/token", formData).GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
                throw new Exception("GetToken Fail");
            var r = resp.Content.ReadAsAsync<TokenResult>().GetAwaiter().GetResult();
            return r.Access_Token;
        }

        static void SaveDailyHealth(string token)
        {
            string filepath = _Settings.DailyHealth.TemplatePath;
            var data = DailyHealth.LoadTemplate(filepath);
            data.date = DateTime.Now.Date.ToUniversalTime();

            var wt = GetCurrentWork();
            data.surveyItemValueList.Find(s => s.itemId == 502).selected = false;
            data.surveyItemValueList.Find(s => s.itemId == 503).selected = false;
            data.surveyItemValueList.Find(s => s.itemId == 504).selected = false;
            switch (wt)
            {
                case WorkType.Office:
                    data.surveyItemValueList.Find(s => s.itemId == 502).selected = true;
                    break;
                case WorkType.WorkFromHome:
                    data.surveyItemValueList.Find(s => s.itemId == 503).selected = true;
                    break;
                case WorkType.Rest:
                    data.surveyItemValueList.Find(s => s.itemId == 504).selected = true;
                    break;
            }

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var resp = client.PostAsJsonAsync<DailyHealth>("tempMonitor/saveDailySurvey", data).GetAwaiter().GetResult();
            var result = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //var result = JsonConvert.SerializeObject(data);
            Console.WriteLine(result);
        }

        static WorkType GetCurrentWork()
        {
            var day = DateTime.Now.DayOfWeek;
            switch (day)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return WorkType.Rest;
                default:
                    return WorkType.Office;
            }
        }
    }

    enum WorkType
    {
        Office,
        WorkFromHome,
        Rest,
    }
}
