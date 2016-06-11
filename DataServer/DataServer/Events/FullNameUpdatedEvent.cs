using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Utils;

namespace DataServer.Events
{
    public class FullNameUpdatedEvent : IEvent
    {
        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime CreatedUtc { get; private set; }
        public string EventTypeName { get; private set; }
        public Type EventType { get; private set; }
        public string EventId { get; private set; }

        public FullNameUpdatedEvent(string id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            CreatedUtc = DateTime.UtcNow;
            EventTypeName = GetType().Name;
            EventType = GetType();
            EventId = Guid.NewGuid().ToString();
        }
    }
}
