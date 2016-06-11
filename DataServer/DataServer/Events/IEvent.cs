using System;

namespace DataServer.Events
{
    public interface IEvent
    {
        string Id { get; }
        string EventTypeName { get; }
        Type EventType { get; }
        string EventId { get; }
    }
}
