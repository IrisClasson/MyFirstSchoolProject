using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsiCrm
{
    [Serializable]
    public class Customer
    {
        public Customer()
        {
            Contacts = new List<Contact>();
            VisitAdress = new Address();
            PostalAdress = new Address();
            Notes = new List<Note>();
        }

        public string VatNumber { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public string Email { get; set; }

        public string WebPage { get; set; }

        public string Telephone { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Note> Notes { get; set; }

        public CustomerType TypeOfCustomer { get; set; }

        public Address VisitAdress { get; set; }

        public Address PostalAdress { get; set; }

        public enum CustomerType
        {
            Active,
            Passive,
            Potential
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
