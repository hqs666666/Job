using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Quartz.Spi;

namespace Quartz.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                       .ConfigureHostConfiguration(configHost =>
                       {
                           //配置根目录
                           configHost.SetBasePath(Directory.GetCurrentDirectory());
                           //读取host的配置json
                           //configHost.AddJsonFile("hostsettings.json", true, true);
                           //读取环境变量，Asp.Net core默认的环境变量是以ASPNETCORE_作为前缀的，这里也采用此前缀以保持一致
                           configHost.AddEnvironmentVariables("ASPNETCORE_");
                           //可以在启动host的时候之前可传入参数
                           //configHost.AddCommandLine(args);
                       })
                       .ConfigureAppConfiguration((hostContext, configApp) =>
                       {
                           configApp.AddJsonFile("appsettings.json", true);
                           configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                           configApp.AddEnvironmentVariables();
                           //configApp.AddCommandLine(args);
                       })
                       .ConfigureServices((hostContext, services) =>
                       {
                           //配置服务及依赖注入注册，注：没有Middleware的配置了。
                           services.AddLogging();
                           //services.AddHostedService<TimedHostedService>();
                           services.AddSingleton<IJobFactory, JobFactory>();
                           services.AddSingleton(provider =>
                           {
                               var option = new QuartzOption(hostContext.Configuration);
                               var sf = new StdSchedulerFactory(option.ToProperties());
                               var scheduler = sf.GetScheduler().Result;
                               scheduler.JobFactory = provider.GetService<IJobFactory>();
                               return scheduler;
                           });
                           services.AddHostedService<QuartzService>();

                           services.AddSingleton<TestJob, TestJob>();
                           services.AddSingleton<TestJob1, TestJob1>();
                       })
                       .ConfigureLogging((hostContext, configLogging) =>
                       {
                           // 日志配置
                           configLogging.AddConsole();
                           if (hostContext.HostingEnvironment.EnvironmentName == EnvironmentName.Development)
                           {
                               configLogging.AddDebug();
                           }
                       })
                       .UseConsoleLifetime()    //使用控制台生命周期,使用Ctrl + C退出
                       .Build();

            host.Run();
        }
    }
}
