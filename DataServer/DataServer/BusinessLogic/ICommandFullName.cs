using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Models;

namespace DataServer.BusinessLogic
{
    public interface ICommandFullName
    {
        Task<string> CreateData(string data1, string data2);
        Task<bool> UpdateData(string id, string data1, string data2);
        Task<FullName> GetById(string id);
    }
}
