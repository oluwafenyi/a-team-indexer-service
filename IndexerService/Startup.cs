using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using IndexerService.Core;
using Owin;
using SearchifyEngine.Database;
using SearchifyEngine.Indexer;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

namespace IndexerService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            Task.Run(async () =>
            {
                string env = Environment.GetEnvironmentVariable("NET_ENV") ?? "Development";
                if (env  == "Development")
                {
                    DbClient.CreateClient(true);
                }
                else
                {
                    DbClient.CreateClient(false);
                }
                await DbClient.CreateTables();
            }).GetAwaiter().GetResult();

            Console.WriteLine("db initialized");

            GlobalConfiguration.Configuration.UseMemoryStorage();

            var options = new BackgroundJobServerOptions
            {
                WorkerCount = 1
            };
            app.UseHangfireDashboard();
            app.UseHangfireServer(options);

            string id = BackgroundJob.Enqueue(() => LongRunningTask());
            Environment.SetEnvironmentVariable("RUNNING_TASK_ID", id);
        }
        
        [DisableConcurrentExecution(60 * 10)]
        public static async Task LongRunningTask()
        {
            Indexer indexer = new Indexer(DbClient.Store);
            while(true)
            {
                try
                {
                    var doc = IndexingServiceQueue.Pop();
                    if (doc != null)
                    {
                        Console.WriteLine("indexing file id: {0}", doc.Id);
                        await indexer.Index(doc.Url, doc.Id);
                        Console.WriteLine("file {0} finished indexing", doc.Id);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                await Task.Delay(5 * 1000);
            }
        }
    }
}