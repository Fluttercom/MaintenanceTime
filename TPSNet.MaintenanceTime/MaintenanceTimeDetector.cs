using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceTime
{
    public class MaintenanceTimeDetector
    {
        private MaintenanceTimeConfig _settings;

        public DateTime NextMaintenance => _settings.GetNearestMaintTime(DateTime.Now);
        public TimeSpan TimeToWork => _settings.GetNearestWorkingTime(DateTime.Now) - DateTime.Now;
        public TimeSpan TimeToMaintenance => _settings.GetNearestMaintTime(DateTime.Now) - DateTime.Now;

        /// <summary>
        /// Call this constructor to load settings from config section.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sectionName"></param>
        public MaintenanceTimeDetector(IConfiguration config, string sectionName)
        {
            LoadSettings(config, sectionName);
        }

        public MaintenanceTimeDetector(MaintenanceTimeConfig config)
        {
            _settings = config;
        }

        public bool IsWorkingTime(DateTime? currentTime = null)
        {
            if (!currentTime.HasValue)
                currentTime = DateTime.Now;
            return !_settings.Any(s => !s.Disabled && s.IsInside(currentTime.Value));
        }

        public bool IsMaintenanceTime(DateTime? currentTime = null)
        {
            return !IsWorkingTime(currentTime);
        }

        public async Task DelayToWorkingTime(CancellationToken stoppingToken)
        {
            var currentTime = DateTime.Now;
            if (IsWorkingTime(currentTime))
            {
                return;
            }
            DateTime nearest = _settings.GetNearestWorkingTime(currentTime);
            if (nearest == DateTime.MinValue)
                return;
            var delay = nearest.Subtract(currentTime);
            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
        }

        public async Task DelayToMaintenanceTime(CancellationToken stoppingToken)
        {
            var currentTime = DateTime.Now;
            if (!IsWorkingTime(currentTime))
            {
                return;
            }
            DateTime nearest = _settings.GetNearestMaintTime(currentTime);
            if (nearest == DateTime.MinValue)
                return;
            var delay = nearest.Subtract(currentTime);
            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
        }


        private void LoadSettings(IConfiguration config, string sectionName)
        {
            _settings = new();
            config.GetSection(sectionName).Bind(_settings);
        }        
    }
}
