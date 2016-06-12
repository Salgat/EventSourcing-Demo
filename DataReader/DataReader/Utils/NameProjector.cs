using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataReader.Events;
using DataReader.Models;

namespace DataReader.Utils
{
    public class NameProjector : INameProjector
    {
        private readonly Dictionary<string, Action<IEvent, ReferenceHolder<NameDTO>>> _eventHandlers;

        public NameProjector()
        {
            _eventHandlers = new Dictionary<string, Action<IEvent, ReferenceHolder<NameDTO>>>
            {
                {typeof(FullNameCreatedEvent).Name, HandleFullNameCreatedEvent},
                {typeof(FullNameUpdatedEvent).Name, HandleFullNameUpdatedEvent}
            };
        }

        /// <summary>
        /// Projects the event onto the FullName Model provided.
        /// </summary>
        /// <typeparam name="T">Type of the Event</typeparam>
        /// <param name="eventData"></param>
        /// <param name="fullNameModel"></param>
        public void HandleEvent<T>(T eventData, ref NameDTO fullNameModel) where T : IEvent
        {
            var referenceHolder = new ReferenceHolder<NameDTO>(fullNameModel);
            _eventHandlers[eventData.GetType().Name](eventData, referenceHolder);
            fullNameModel = referenceHolder.Value;
        }

        private void HandleFullNameCreatedEvent(IEvent eventData, ReferenceHolder<NameDTO> fullNameModel)
        {
            var createdEvent = eventData as FullNameCreatedEvent;
            fullNameModel.Value = new NameDTO(createdEvent.Id, $"{createdEvent.FirstName} {createdEvent.LastName}");
        }

        private void HandleFullNameUpdatedEvent(IEvent eventData, ReferenceHolder<NameDTO> fullNameModel)
        {
            var updatedEvent = eventData as FullNameUpdatedEvent;
            fullNameModel.Value = new NameDTO(updatedEvent.Id, $"{updatedEvent.FirstName} {updatedEvent.LastName}");
        }
        
        /// <summary>
        /// This bypasses a restriction in Actions since Actions do not allow using "ref" parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ReferenceHolder<T>
        {
            public T Value { get; set; }

            public ReferenceHolder(T value)
            {
                Value = value;
            }
        }
    }
}