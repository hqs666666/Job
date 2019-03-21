using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace HangFire.Demo
{
    public class Startup
    {
        public static ConnectionMultiplexer Redis;

        public Startup()
        {
            Redis = ConnectionMultiplexer.Connect("localhost");
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHangfire(configuration =>
            //{
            //    configuration.UseRedisStorage(Redis);
            //});
            services.AddScoped<ITimerService, TimerService>();
            services.AddScoped<ICheckService, CheckService>();
            services.AddHangfire(r => r.UseSqlServerStorage("Server=.;Database=myDataBase;Trusted_Connection=True;MultipleActiveResultSets=true;"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            GlobalConfiguration.Configuration.UseActivator<MyActivator>(new MyActivator(serviceProvider));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.Run(async (context) =>
            {
                BackgroundJob.Enqueue<ICheckService>(c => c.Check());
                context.Response.Redirect("/hangfire");
                await Task.CompletedTask;
            });
        }
    }
}
