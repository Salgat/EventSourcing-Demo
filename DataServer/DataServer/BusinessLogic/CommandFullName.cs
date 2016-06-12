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
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<string> CreateFullName(string firstName, string lastName)
        {
            var fullName = new FullName();
            var events = fullName.CreateFullName(firstName, lastName);
            await RaiseEvents(events);
            return fullName.Id;
        }

        public async Task<bool> UpdateData(string id, string firstName, string lastName)
        {
            var fullName = await _dataRepository.GetById(id);
            var events = fullName.UpdateFullName(firstName, lastName);
            await RaiseEvents(events);
            return true;
        }

        public async Task<FullName> GetById(string id)
        {
            return await _dataRepository.GetById(id);
        }

        #region Event Handling

        private async Task<bool> RaiseEvents(IList<IEvent> events)
        {
            foreach (var eventItem in events)
            {
                await _dataRepository.SaveEvent(eventItem);
            }

            return true;
        }

        #endregion

    }
}