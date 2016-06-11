using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServer.Models
{
    public class FullName
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public FullName()
        {
        }

        public FullName(string id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}