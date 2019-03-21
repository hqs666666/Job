using System;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace HangFire.Demo
{
    public interface ICheckService
    {
        void Check();
    }

    public class CheckService : ICheckService
    {
        private readonly ILogger<CheckService> _logger;
        private ITimerService _timeservice;
        public CheckService(ILoggerFactory loggerFactory,
            ITimerService timerService)
        {
            _logger = loggerFactory.CreateLogger<CheckService>();
            _timeservice = timerService;
        }

        public void Check()
        {
            _logger.LogInformation($"check service start checking, now is {DateTime.Now}");
            //延迟执行后台脚本，仅执行一次
            BackgroundJob.Schedule(() => _timeservice.Timer(), TimeSpan.FromMilliseconds(30));
            _logger.LogInformation($"check is end, now is {DateTime.Now}");
        }

        public void Test()
        {
            ////执行后台脚本，仅执行一次
            //BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));

            ////延迟执行后台脚本呢，仅执行一次
            //BackgroundJob.Schedule(
            //    () => Console.WriteLine("Delayed!"),
            //    TimeSpan.FromDays(7));

            ////周期性任务
            //RecurringJob.AddOrUpdate(
            //    () => Console.WriteLine("Recurring!"),
            //    Cron.Daily);

            ////等上一任务完成后执行
            //BackgroundJob.ContinueWith(
            //    jobId,  //上一个任务的jobid
            //    () => Console.WriteLine("Continuation!"));
        }
    }

}
