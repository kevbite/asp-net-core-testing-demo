using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Demo1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventsDataStore _store;

        public EventsController(EventsDataStore store) => _store = store;

        [HttpGet] // /events
        public IEnumerable<Event> Get()
        {
            return _store.GetAll();
        }
        
        [HttpPost] // events
        public IActionResult Post(Event @event, [FromServices] IConfiguration configuration)
        {
            if (_store.Get(@event.Id) is not null)
            {
                return Conflict();
            }

            if (configuration.GetValue<bool?>("ValidateNameFeature") is true)
            {
                if (!@event.Name.StartsWith("Example"))
                {
                    return BadRequest();
                }
            }
            
            _store.Add(@event);
            return Ok();
        }

        [HttpGet("/{id}")] // /events/{id}
        public IActionResult Get(Guid id)
        {
            return _store.Get(id) switch
            {
                { } @event => Ok(@event),
                _ => NotFound()
            };
        }
        
        [HttpDelete("/{id}")] // /events/{id}
        [Authorize(Roles = "Administrators")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            if (_store.Get(@id) is not null)
            {
                return NotFound();
            }
            
            _store.Delete(@id);
            return Ok();
        }
    }
}
