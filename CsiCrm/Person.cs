using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsiCrm
{
    [Serializable]
    public abstract class Person
    {
        public string FirstName { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public List<Note> Notes { get; set; }
        
        public List<Contact.Interest> Inrests = new List<Contact.Interest>();

    }
}
