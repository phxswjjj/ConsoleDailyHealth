using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleDailyHealth;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsoleDailyHealth.Tests
{
    [TestClass()]
    public class DailyHealthTests
    {
        const string TemplatePath = @"..\..\..\..\assets\daily_health.json";
        [TestMethod()]
        public void ParseJsonTest()
        {
            var json = ReadFile(TemplatePath);
            var data = DailyHealth.ParseJson(json);
            Assert.IsNotNull(data);
            Assert.AreEqual("***", data.currentAddress);
            Assert.AreEqual("***", data.mobileTel);
            Assert.IsTrue(data.agreeTheForm);
            Assert.AreEqual(new DateTime(2021, 6, 27).ToUniversalTime(), data.date);
            Assert.IsTrue(data.surveyItemValueList.Find(s => s.itemId == 552).selected.Value);
            Assert.IsTrue(data.surveyItemValueList.Find(s => s.itemId == 553).isNeedTrack.Value);
            Assert.IsTrue(data.isNormal);
        }

        [TestMethod()]
        public void LoadTemplateTest()
        {
            var data = DailyHealth.LoadTemplate(TemplatePath);
            data.date = DateTime.Now.Date.ToUniversalTime();

            Assert.AreEqual(DateTime.Now.AddDays(-1).Day, data.date.Day);
            Assert.AreEqual(16, data.date.Hour);
        }

        private string ReadFile(string filepath)
        {
            using (var reader = new StreamReader(filepath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}