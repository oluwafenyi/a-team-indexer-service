using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using FakeItEasy;
using Hangfire;
using Hangfire.MemoryStorage;
using IndexerService.Controllers;
using IndexerService.Models;
using NUnit.Framework;
using SearchifyEngine.Database;
using SearchifyEngine.Store;

namespace IndexerService.Test
{
    public class IndexControllerTests
    {
        private IndexController SetupController()
        {
            JobStorage.Current = new MemoryStorage();
            var client = new BackgroundJobClient();
            var controller = new IndexController(client);
            return controller;
        }
        
        [Test]
        public void TestPing()
        {
            var controller = SetupController();

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:5000", ""), new HttpResponse(TextWriter.Null));
            var response = controller.Ping();

            var result = response as OkNegotiatedContentResult<string>;
            Assert.AreEqual("ok!", result.Content);
        }

        [Test]
        public async Task TestAcceptedIndexTask()
        {
            var controller = SetupController();
            
            Document document = new Document();
            document.Id = 1;
            document.Url = "http://www.africau.edu/images/default/sample.pdf";

            var store = A.Fake<IStore>();
            A.CallTo(() => store.GetLastId()).Returns<uint>(0);
            DbClient.Store = store;

            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/index");
            controller.Request.SetConfiguration(new HttpConfiguration());
            
            var response = await controller.Index(new []{document});
            var result = response as ResponseMessageResult;
            
            Assert.AreEqual(HttpStatusCode.Accepted,result.Response.StatusCode);
        }
        
        [Test]
        public async Task TestRejectedIndexTaskInvalidId()
        {
            var controller = SetupController();
            
            Document document = new Document();
            document.Id = 1;
            document.Url = "http://www.africau.edu/images/default/sample.pdf";

            var store = A.Fake<IStore>();
            A.CallTo(() => store.GetLastId()).Returns<uint>(2);
            DbClient.Store = store;

            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/index");
            controller.Request.SetConfiguration(new HttpConfiguration());
            
            var response = await controller.Index(new []{document});
            
            Assert.IsInstanceOf<InvalidModelStateResult>(response);
        }
    }
}