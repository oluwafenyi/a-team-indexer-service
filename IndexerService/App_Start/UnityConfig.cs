using System.Web.Http;
using Hangfire;
using IndexerService.Controllers;
using Unity;
using Unity.WebApi;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace IndexerService
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            // var client = new BackgroundJobClient();

            container.RegisterSingleton<IBackgroundJobClient, BackgroundJobClient>();
            container.RegisterType<ApiController, IndexController>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}