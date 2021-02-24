﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aggregator_server.Exceptions;
using aggregator_server.Models;
using log4net;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(m_repository.GetConfiguration(id));
            }
            catch (RepositoryItemNotFoundException rinfe)
            {
                log.Warn($"Get request for invalid PollConfiguration ID {id}");
                return NotFound(new { ErrorMessage = rinfe.Message });
            }
            catch (RepositoryException re)
            {
                log.Error($"Unexpected RepositoryException getting PollConfiguration ID {id}, message: {re.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = re.Message });
            }
        }

        // POST api/<PollConfigurationController>
        [HttpPost]
        public IActionResult Post([FromBody] PollConfigurationTransferObject newConfiguration)
        {
            // TODO: all validation inside of repository?
            if (newConfiguration.URL == null || newConfiguration.PollIntervalMinutes == null || newConfiguration.PollIntervalMinutes <= 0)
            {
                return BadRequest(new { ErrorMessage = "Poll configuration must specify a URL, and a poll interval greater than 0." });
            }

            try
            {
                bool active = newConfiguration.Active ?? true;

                var addedConfiguration = m_repository.AddConfiguration(newConfiguration.URL, newConfiguration.PollIntervalMinutes.Value, active);
                configLog.Info($"Added poll configuration: ID = {addedConfiguration.ID}, Interval = {addedConfiguration.PollIntervalMinutes}, URL = {addedConfiguration.URL}");

                return Ok(addedConfiguration);
            }
            catch (RepositoryConflictException rce)
            {
                log.Warn($"Add of poll configuration {newConfiguration}, failed with message {rce.Message}");
                return Conflict(new { ErrorMessage = rce.Message }); 
            }
            catch (RepositoryException re)
            {
                log.Error($"Unexpected RepositoryException adding poll configuration {newConfiguration}, message: {re.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = re.Message });
            }
        }

        // PUT api/<PollConfigurationController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //DELETE api/<PollConfigurationController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                m_repository.DeleteConfiguration(id);

                return Ok();
            }
            catch (RepositoryItemNotFoundException rinfe)
            {
                log.Warn($"Delete request for invalid PollConfiguration ID {id}");
                return NotFound(new { ErrorMessage = rinfe.Message });
            }
            catch (RepositoryException re)
            {
                log.Error($"Unexpected RepositoryException deleting PollConfiguration ID {id}, message: {re.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = re.Message });
            }
        }
    }
}
