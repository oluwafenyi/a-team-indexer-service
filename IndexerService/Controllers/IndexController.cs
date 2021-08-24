using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Hangfire;
using IndexerService.Core;
using IndexerService.Models;
using SearchEngine.Database.Models;

namespace IndexerService.Controllers
{
    public class IndexController : ApiController
    {
        private readonly IBackgroundJobClient _client;
        
        public IndexController(IBackgroundJobClient client)
        {
            _client = client;
        }

        // GET: api/
        [Route("api/")]
        [HttpGet]
        public IHttpActionResult Root()
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
        public async Task<IHttpActionResult> Index(Document data)
        {
            if (ModelState.IsValid)
            {
                uint lastId = await InvertedIndex.GetLastId();
                if (data.Id <= lastId)
                {
                    return BadRequest();
                }
                
                _client.Enqueue(() => IndexingService.Index(data));
                return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.Accepted, "document queued for indexing"));
            }

            return BadRequest();
        }
    }
}