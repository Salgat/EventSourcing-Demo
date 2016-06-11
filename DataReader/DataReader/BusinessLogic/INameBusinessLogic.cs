using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReader.Models;

namespace DataReader.BusinessLogic
{
    public interface INameBusinessLogic
    {
        NameDTO GetById(string id);
        string[] GetAllNameIds();
    }
}
