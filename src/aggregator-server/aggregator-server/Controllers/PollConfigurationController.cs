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
    [Route("api/configuration/poll")]
    [ApiController]
    public class PollConfigurationController : ControllerBase
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(PollConfigurationController)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(PollConfigurationController));

        private PollConfigurationRepository m_repository;

        public PollConfigurationController(PollConfigurationRepository repository)
        {
            m_repository = repository;
        }

        // GET: api/<PollConfigurationController>
        [HttpGet]
        public IEnumerable<PollConfiguration> Get()
        {
            return m_repository.Configurations;
        }

        // GET api/<PollConfigurationController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<PollConfigurationController>
        [HttpPost]
        public void Post([FromBody] PollConfiguration value)
        {
            configLog.Info($"Adding poll configuration: ID = {value.ID}, Interval = {value.PollIntervalMinutes}, URL = {value.URL}");
            m_repository.Configurations.Add(value);
        }

        // PUT api/<PollConfigurationController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<PollConfigurationController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
            
        //}
    }
}
