using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Threading.Tasks;
using MonolithicNetCore.Service;
using MonolithicNetCore.Service.GoogleService;

namespace MonolithicNetCore.Web.ScheduleJob.Job
{

    public class BackUpDatabaseJob : IJob
    {
        private readonly IServiceProvider _provider;
        public BackUpDatabaseJob(IServiceProvider provider)
        {
            this._provider = provider;
        }
        public Task Execute(IJobExecutionContext context)
        {
            //using (var scope = _provider.CreateScope())
            //{
            //    // Resolve the Scoped service
            //    var _googleDriverService = scope.ServiceProvider.GetService<IGoogleDriverService>();
            //    _googleDriverService.BackUpDatabase();
            //}
            Console.WriteLine("Backup database done");
            return Task.CompletedTask;
        }
    }
}
