using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventStore.ClientAPI;

namespace DataServer.Utils
{
    public static class EventStoreFactory
    {
        private static IEventStoreConnection _connection = null;

        public static void Init(string connectionString)
        {
            if (_connection != null) return;
            var connection = EventStoreConnection.Create(connectionString);
            connection.ConnectAsync().Wait();
            _connection = connection;
        }

        public static IEventStoreConnection GetConnection()
        {
            return _connection;
        }
    }
}