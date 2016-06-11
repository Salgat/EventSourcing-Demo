using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataServer.Events;
using DataServer.Models;

namespace DataServer.Utils
{
    public class FullNameProjector : IFullNameProjector
    {
        private readonly Dictionary<Type, Action<IEvent, FullName>> _eventHandlers;

        public FullNameProjector()
        {
            _eventHandlers = new Dictionary<Type, Action<IEvent, FullName>>
            {
                {typeof(FullNameCreatedEvent), (eventData, data) => HandleFullNameCreatedEvent(eventData, ref data)},
                {typeof(FullNameUpdatedEvent), (eventData, data) => HandleFullNameUpdatedEvent(eventData, ref data)}
            };
        }

        /// <summary>
        /// Projects the event onto the FullName Model provided.
        /// </summary>
        /// <typeparam name="T">Type of the Event</typeparam>
        /// <param name="eventData"></param>
        /// <param name="fullNameModel"></param>
        public void HandleEvent<T>(T eventData, ref FullName fullNameModel) where T : IEvent
        {
            _eventHandlers[eventData.GetType()](eventData, fullNameModel);
        }

        private void HandleFullNameCreatedEvent(IEvent eventData, ref FullName fullNameModel)
        {
            var createdEvent = eventData as FullNameCreatedEvent;
            fullNameModel.Id = createdEvent.Id;
            fullNameModel.FirstName = createdEvent.FirstName;
            fullNameModel.LastName = createdEvent.LastName;
        }

        private void HandleFullNameUpdatedEvent(IEvent eventData, ref FullName fullNameModel)
        {
            var updatedEvent = eventData as FullNameUpdatedEvent;
            fullNameModel.FirstName = updatedEvent.FirstName;
            fullNameModel.LastName = updatedEvent.LastName;
        }
    }
}