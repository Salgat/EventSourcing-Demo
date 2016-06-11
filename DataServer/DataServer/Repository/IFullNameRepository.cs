using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Events;
using DataServer.Models;

namespace DataServer.Repository
{
    public interface IFullNameRepository
    {
        Task<FullName> GetById(string id);
        Task<bool> SaveEvent(IEvent eventData);
    }
}
