using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DataReader.Events;
using DataReader.Models;
using DataReader.Utils;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace DataReader.Repository
{
    public static class NameRepository
    {
        private static IEventStoreConnection _connection;
        private static INameProjector _modelProjector;
        private static ConcurrentDictionary<string, NameDTO> _names;
        private static ConcurrentBag<string> _nameIds;
        private static readonly IDictionary<string, Type> _eventTypeResolver;

        static NameRepository()
        {
            _eventTypeResolver = new Dictionary<string, Type>()
            {
                { typeof(FullNameCreatedEvent).Name, typeof(FullNameCreatedEvent) },
                { typeof(FullNameUpdatedEvent).Name, typeof(FullNameUpdatedEvent) }
            };
        }

        /// <summary>
        /// Initializes the in-memory names list with all names that exist in the "FullName" events category and
        /// sets up the service to subscribe to all future FullName events to update the names list.
        /// </summary>
        public static async Task<bool> Init(IEventStoreConnection eventStoreConnection, INameProjector nameProjector)
        {
            _connection = eventStoreConnection;
            _modelProjector = nameProjector;
            _names = new ConcurrentDictionary<string, NameDTO>();
            _nameIds = new ConcurrentBag<string>();

            // First read all events in the FullName category
            // TODO: Add paging to handle more than 4096 events
            var eventStreamName = "$ce-FullName";
            var fullNameEvents = await _connection.ReadStreamEventsForwardAsync(eventStreamName, StreamPosition.Start, 4096, true);

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
            
            // For each event, find or create a new model, and project the event onto the model
            foreach (var eventData in events)
            {
                var modelId = eventData.Id;
                NameDTO model;
                if (!_names.TryGetValue(modelId, out model))
                {
                    _nameIds.Add(modelId);
                    model = new NameDTO();
                }

                _modelProjector.HandleEvent(eventData, ref model);

                _names.AddOrUpdate(modelId, model, (key, value) => model);
            }

            // Finally, subscribe to the FullName category event stream for future updates.
            // TODO:

            return true;
        }

        public static NameDTO GetById(string id)
        {
            try
            {
                NameDTO name;
                _names.TryGetValue(id, out name);
                return name;
            }
            catch (ArgumentNullException)
            {
                throw new Exception($"Name does not exist with Id: {id}");
            }
        }

        /// <summary>
        /// Returns an array of Ids (unique) for all existing Names.
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllIds()
        {
            return _nameIds.ToArray();
        }
    }
}