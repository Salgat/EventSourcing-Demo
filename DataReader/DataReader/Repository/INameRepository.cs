using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataReader.Models;

namespace DataReader.Repository
{
    public interface INameRepository
    {
        NameDTO GetById(string id);
        string[] GetAllIds();
    }
}