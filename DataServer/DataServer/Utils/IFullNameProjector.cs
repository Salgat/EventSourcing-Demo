using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Events;
using DataServer.Models;

namespace DataServer.Utils
{
    public interface IFullNameProjector
    {
        void HandleEvent<T>(T eventData, ref FullName fullNameModel) where T : IEvent;
    }
}
