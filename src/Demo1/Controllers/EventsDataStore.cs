using System;
using System.Collections.Generic;

namespace Demo1.Controllers
{
    public class EventsDataStore
    {
        private readonly Dictionary<Guid, Event> _events = new();
        public IEnumerable<Event> GetAll()
        {
            return _events.Values;
        }
        
        public Event? Get(Guid id)
        {
            return _events.TryGetValue(id, out var @event) switch
            {
                true => @event,
                false => null
            };
        }

        public void Add(Event @event)
        {
            _events.Add(@event.Id, @event);
        }

        public void Delete(Guid id)
        {
            _events.Remove(id);
        }
    }
}