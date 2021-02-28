using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aggregator_server.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aggregator_server.Controllers
{
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(DocumentController)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentController));

        private IDocumentRepository m_repository;

        public DocumentController(IDocumentRepository repository)
        {
            m_repository = repository;
        }

        // GET: api/<DocumentController>
        [HttpGet]
        public IEnumerable<Document> Get()
        {
            return m_repository.GetDocuments();
        }

        //// GET api/<DocumentController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<DocumentController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<DocumentController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<DocumentController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
