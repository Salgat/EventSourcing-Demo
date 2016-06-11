using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DataServer.Events;
using DataServer.Models;
using DataServer.Utils;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace DataServer.Repository
{
    public class FullNameRepository : IFullNameRepository
    {
        private IEventStoreConnection _connection;
        private IFullNameProjector _modelProjector;
        private readonly IDictionary<string, Type> _eventTypeResolver;

        public FullNameRepository(IFullNameProjector modelProjector)
        {
            _connection = EventStoreFactory.GetConnection();
            _modelProjector = modelProjector;
            _eventTypeResolver = new Dictionary<string, Type>()
            {
                { typeof(FullNameCreatedEvent).Name, typeof(FullNameCreatedEvent) },
                { typeof(FullNameUpdatedEvent).Name, typeof(FullNameUpdatedEvent) }
            };
        }

        /// <summary>
        /// Returns a FullName model reconstructed from the event stream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FullName> GetById(string id)
        {
            // First read all events for this Id
            var eventStreamName = $"FullName-{id}";
            var fullNameEvents = await _connection.ReadStreamEventsForwardAsync(eventStreamName, StreamPosition.Start, 4096, false);

            var events = new List<IEvent>();
            foreach (var resolvedEvent in fullNameEvents.Events)
            {
                var eventMetaObjectRaw = resolvedEvent.Event.Metadata;
                var eventMetaObjectAsJson = Encoding.UTF8.GetString(eventMetaObjectRaw);
                var eventMetaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventMetaObjectAsJson);
                var eventName = eventMetaDictionary["EventName"].ToString();
                var eventType = _eventTypeResolver[eventName];

                var eventObjectRaw = resolvedEvent.Event.Data;
                var eventObjectAsJson = Encoding.UTF8.GetString(eventObjectRaw);
                var eventObject = (IEvent)JsonConvert.DeserializeObject(eventObjectAsJson, eventType);
                events.Add(eventObject);
            }

            var dataModel = new FullName();
            foreach (var eventData in events)
            {
                _modelProjector.HandleEvent(eventData, ref dataModel);
            }

            return dataModel;
        }

        /// <summary>
        /// Writes an event to the event stream out of order.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public async Task<bool> SaveEvent(IEvent eventData)
        {
            var eventDataAsJson = JsonConvert.SerializeObject(eventData);
            var eventDataAsBytes = Encoding.UTF8.GetBytes(eventDataAsJson);

            var metaData = new Dictionary<string, object>()
            {
                {"EventType", eventData.EventType},
                {"EventName", eventData.EventTypeName},
                {"EventId", eventData.EventId}
            };
            var metaDataAsJson = JsonConvert.SerializeObject(metaData);
            var metaDataAsBytes = Encoding.UTF8.GetBytes(metaDataAsJson);

            var eventModel = new EventData(Guid.Parse(eventData.EventId), eventData.EventTypeName, true, eventDataAsBytes, metaDataAsBytes);

            var eventStreamName = $"FullName-{eventData.Id}";
            var writeResult = await _connection.AppendToStreamAsync(eventStreamName, ExpectedVersion.Any, eventModel);
            return true;
        }
    }
}