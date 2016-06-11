using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataServer.Events;
using DataServer.Models;
using DataServer.Repository;

namespace DataServer.BusinessLogic
{
    public class CommandFullName : ICommandFullName
    {
        private readonly IFullNameRepository _dataRepository;

        public CommandFullName(IFullNameRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// Creates a new FullName model and returns the Id of the new model.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public async Task<string> CreateData(string data1, string data2)
        {
            var newId = Guid.NewGuid().ToString();
            var createdEvent = new FullNameCreatedEvent(newId, data1, data2);
            var result = await _dataRepository.SaveEvent(createdEvent);
            return result ? newId : null;
        }

        public async Task<bool> UpdateData(string id, string data1, string data2)
        {
            var updatedEvent = new FullNameUpdatedEvent(id, data1, data2);
            var result = await _dataRepository.SaveEvent(updatedEvent);
            return result;
        }

        public async Task<FullName> GetById(string id)
        {
            return await _dataRepository.GetById(id);
        }
    }
}