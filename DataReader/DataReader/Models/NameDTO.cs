using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReader.Models
{
    public class NameDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }

        public NameDTO()
        {
        }

        public NameDTO(string id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}