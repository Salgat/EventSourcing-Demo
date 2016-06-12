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
        private static readonly IDictionary<string, Type> EventTypeResolver;

        static NameRepository()
        {
            EventTypeResolver = new Dictionary<string, Type>()
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

            // Subscribe to the FullName category event stream for all events now and in the future
            var eventStreamName = "$ce-FullName";
            var catchupSettings = new CatchUpSubscriptionSettings(4096, 4096, true, true);
            _connection.SubscribeToStreamFrom(eventStreamName, null, catchupSettings, HandleEventFromSubscription, null, ReconnectToStreamSubscription);

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

        #region Process Events

        /// <summary>
        /// Processes a resolvedEvent and returns a deserialized IEvent.
        /// </summary>
        /// <param name="resolvedEvent"></param>
        /// <returns></returns>
        private static IEvent ReadEventData(ResolvedEvent resolvedEvent)
        {
            var eventMetaObjectRaw = resolvedEvent.Event.Metadata;
            var eventMetaObjectAsJson = Encoding.UTF8.GetString(eventMetaObjectRaw);
            var eventMetaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventMetaObjectAsJson);
            var eventName = eventMetaDictionary["EventName"].ToString();
            var eventType = EventTypeResolver[eventName];

            var eventObjectRaw = resolvedEvent.Event.Data;
            var eventObjectAsJson = Encoding.UTF8.GetString(eventObjectRaw);
            var eventData = (IEvent)JsonConvert.DeserializeObject(eventObjectAsJson, eventType);
            return eventData;
        }

        /// <summary>
        /// Projects an IEvent onto a model and updates the in-memory Names.
        /// </summary>
        /// <param name="eventData"></param>
        private static void HandleEventData(IEvent eventData)
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

        #endregion

        #region Event Subscription

        /// <summary>
        /// Callback used to handle events from the event stream subscription. Takes event and projects it onto a model, either
        /// adding or updating the current Name models in memory.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="resolvedEvent"></param>
        private static void HandleEventFromSubscription(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            var eventData = ReadEventData(resolvedEvent);
            HandleEventData(eventData);
        }

        /// <summary>
        /// Callback used to handle a dropped subscription to the Name category event stream.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="reason"></param>
        /// <param name="exception"></param>
        private static void ReconnectToStreamSubscription(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception exception)
        {
            throw new Exception($"Subscription to stream ended due to reason: {exception.Message}");
        }

        #endregion

    }
}