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

        private IPollConfigurationRepository m_repository;

        public PollConfigurationController(IPollConfigurationRepository repository)
        {
            m_repository = repository;
        }

        // GET: api/<PollConfigurationController>
        [HttpGet]
        public IEnumerable<PollConfiguration> Get()
        {
            return m_repository.GetConfigurations();
        }

        // GET api/<PollConfigurationController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<PollConfigurationController>
        [HttpPost]
        public IActionResult Post([FromBody] PollConfiguration newConfiguration)
        {
            if (newConfiguration.URL == null || newConfiguration.PollIntervalMinutes == null || newConfiguration.PollIntervalMinutes <= 0)
            {
                return BadRequest(new { ErrorMessage = "Poll configuration must specify a URL, and a poll interval greater than 0." });
            }

            var addedConfiguration = m_repository.AddConfiguration(newConfiguration.URL, newConfiguration.PollIntervalMinutes.Value);
            configLog.Info($"Added poll configuration: ID = {addedConfiguration.ID}, Interval = {addedConfiguration.PollIntervalMinutes}, URL = {addedConfiguration.URL}");

            return Ok(addedConfiguration);
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
