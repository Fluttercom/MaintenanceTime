using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceTime.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void WeeklyMaintenance()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            MaintenanceTimeDetector detector = new MaintenanceTimeDetector(config, "MaintenanceTime");
            DateTime working = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(2);
            Assert.IsTrue(detector.IsWorkingTime(working));
            DateTime maint = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(6);
            Assert.IsFalse(detector.IsWorkingTime(maint));
            DateTime maint2 = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(1).AddHours(18);
            Assert.IsFalse(detector.IsWorkingTime(maint2));
        }

        [TestMethod]
        public void DailyMaintenance()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            MaintenanceTimeDetector detector = new MaintenanceTimeDetector(config, "DailyMaintenance");
            DateTime maint = DateTime.Today.AddHours(18);
            Assert.IsTrue(detector.IsMaintenanceTime(maint));
        }

        [TestMethod]
        public void DailyOverlapMaintenance()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            MaintenanceTimeDetector detector = new MaintenanceTimeDetector(config, "DailyOverlap");
            DateTime maint = DateTime.Today.AddHours(20);
            Assert.IsTrue(detector.IsMaintenanceTime(maint));
        }

        [TestMethod]
        public void ManualCreation()
        {
            MaintenanceTimeConfig config = new MaintenanceTimeConfig
            {
                new MaintenanceTimeConfigEntry{From = new WeekdayTimeEntry("17:00", "Friday"), To = new WeekdayTimeEntry("06:00", "Monday")},
                new MaintenanceTimeConfigEntry("Tuesday, 6:00", "Wednesday,6:00")
            };
            MaintenanceTimeDetector detector = new MaintenanceTimeDetector(config);
            var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            Assert.IsTrue(detector.IsMaintenanceTime(weekStart.AddDays(6)));
            Assert.IsTrue(detector.IsMaintenanceTime(weekStart.AddDays(3)));
        }

        [TestMethod]
        public void ManualDailyCreation()
        {
            MaintenanceTimeConfig config = new MaintenanceTimeConfig
            {
                new MaintenanceTimeConfigEntry{From = new WeekdayTimeEntry("16:00"), To = new WeekdayTimeEntry("18:00")}
            };
            MaintenanceTimeDetector detector = new MaintenanceTimeDetector(config);
            var now = DateTime.Now.Date + TimeSpan.FromHours(17);
            Assert.IsTrue(detector.IsMaintenanceTime(now));
            //CancellationTokenSource cts = new();
            //Task.Run(() => detector.DelayToWorkingTime(cts.Token));
            //cts.Cancel();
        }

        [TestMethod]
        public void DisabledEntry()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            MaintenanceTimeDetector detector = new MaintenanceTimeDetector(config, "DisabledMaintenance");
            Assert.IsTrue(detector.IsWorkingTime());
            var ts = new CancellationTokenSource(1000);
            detector.DelayToMaintenanceTime(ts.Token).Wait();
            Assert.AreEqual(DateTime.MinValue, detector.NextMaintenance);
        }
    }
}
