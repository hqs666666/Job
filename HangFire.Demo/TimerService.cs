using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HangFire.Demo
{
    public interface ITimerService
    {
        void Timer();
    }

    public class TimerService : ITimerService
    {
        private readonly ILogger<TimerService> _logger;
        public TimerService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerService>();
        }
        public void Timer()
        {
            _logger.LogInformation($"timer service is starting, now is {DateTime.Now}");
            _logger.LogWarning("timering");
            _logger.LogInformation($"timer is end, now is {DateTime.Now}");
        }
    }
}
