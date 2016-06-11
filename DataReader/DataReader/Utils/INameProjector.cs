using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataReader.Events;
using DataReader.Models;

namespace DataReader.Utils
{
    public interface INameProjector
    {
        void HandleEvent<T>(T eventData, ref NameDTO fullNameModel) where T : IEvent;
    }
}