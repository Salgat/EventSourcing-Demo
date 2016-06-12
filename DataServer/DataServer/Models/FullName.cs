using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataServer.Events;

namespace DataServer.Models
{
    public class FullName
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public FullName()
        {
        }

        public FullName(string id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public IList<IEvent> CreateFullName(string firstName, string lastName)
        {
            var events = new List<IEvent>();

            Id = Guid.NewGuid().ToString();
            FirstName = firstName;
            LastName = lastName;
            events.Add(new FullNameCreatedEvent(Id, firstName, lastName));

            return events;
        }

        public IList<IEvent> UpdateFullName(string firstName, string lastName)
        {
            var events = new List<IEvent>();

            FirstName = firstName;
            LastName = lastName;
            events.Add(new FullNameUpdatedEvent(Id, firstName, lastName));

            return events;
        }
    }
}