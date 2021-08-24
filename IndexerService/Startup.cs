using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using Owin;
using SearchEngine.Database;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

namespace IndexerService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            Task.Run(async () =>
            {
                DbClient.CreateClient(true);
                await DbClient.CreateTables();
            }).GetAwaiter().GetResult();
            Console.WriteLine("db initialized");

            GlobalConfiguration.Configuration.UseMemoryStorage();

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}