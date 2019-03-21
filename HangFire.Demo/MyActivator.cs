using System;
using Hangfire;

namespace HangFire.Demo
{
    /// <summary>
    /// 重写ActivateJob方法，使其返回的类型从我们的IServiceProvider中获取
    /// </summary>
    public class MyActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;
        public MyActivator(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public override object ActivateJob(Type jobType)
        {
            return _serviceProvider.GetService(jobType);
        }
    }
}
