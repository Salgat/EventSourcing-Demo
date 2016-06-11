using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.Events
{
    public interface IEvent
    {
        string Id { get; }
        string EventTypeName { get; }
        string EventId { get; }
    }
}
