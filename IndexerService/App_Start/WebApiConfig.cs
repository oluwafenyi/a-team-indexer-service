using Newtonsoft.Json.Serialization;

using System.Web.Http;
using System.Web.Http.Cors;
using Swagger.Net.Application;

namespace IndexerService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            config.EnableSwagger(c => c.SingleApiVersion("v1", "Searchify Indexing Service")).EnableSwaggerUi();

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            // Set JSON formatter as default one and remove XmlFormatter
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            jsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        }
    }
}