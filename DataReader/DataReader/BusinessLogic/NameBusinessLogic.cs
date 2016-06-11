using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataReader.Models;
using DataReader.Repository;

namespace DataReader.BusinessLogic
{
    public class NameBusinessLogic : INameBusinessLogic
    {
        public NameBusinessLogic()
        {
        }

        public NameDTO GetById(string id)
        {
            try
            {
                return NameRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get Name by Id {id} with exception: {ex.Message}");   
            }
        }

        public string[] GetAllNameIds()
        {
            try
            {
                return NameRepository.GetAllIds();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all Name ids with exception: {ex.Message}");
            }
        }
    }
}