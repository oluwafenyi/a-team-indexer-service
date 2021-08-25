using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using System.Web.Http.ValueProviders;
using Hangfire;
using IndexerService.Core;
using IndexerService.Models;
using SearchifyEngine.Database;

namespace IndexerService.Controllers
{
    public class IndexController : ApiController
    {
        private readonly IBackgroundJobClient _client;
        
        public IndexController(IBackgroundJobClient client)
        {
            _client = client;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Root()
        {
            string url = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return Redirect(url + "/swagger/ui/index");
        }

        // GET: api/
        [Route("api/")]
        [HttpGet]
        public IHttpActionResult Ping()
        {
            if (HttpContext.Current.Request.QueryString["marco"] != null)
            {
                return Ok("polo!");
            }
            if (HttpContext.Current.Request.QueryString["ping"] != null)
            {
                return Ok("pong!");
            }
            return Ok("ok!");
        }
        
        // POST: api/index/
        [Route("api/index/")]
        [HttpPost]
        public async Task<IHttpActionResult> Index(Document[] data)
        {
            if (ModelState.IsValid)
            {
                ModelStateDictionary modelStateDictionary = new ModelStateDictionary();
                uint lastId = await DbClient.Store.GetLastId();
                
                foreach (var document in data)
                {
                    if (document.Id <= lastId)
                    {
                        string idStr = document.Id.ToString();
                        modelStateDictionary[idStr] = new ModelState
                        {
                            Value = new ValueProviderResult("error: this id has already been indexed",
                                "error: this id has already been indexed", CultureInfo.CurrentCulture)
                        };
                    }
                }

                if (modelStateDictionary.Keys.Count > 0)
                {
                    return BadRequest(modelStateDictionary);
                }
                
                foreach (var document in data.OrderBy(doc => doc.Id))
                {
                    IndexingServiceQueue.Enqueue(document);
                }
                return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.Accepted, "document queued for indexing"));
            }

            return BadRequest();
        }
    }
}