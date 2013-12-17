using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsiCrm
{
    [Serializable]
    public class Contact : Person
    {
         public enum Interest
        {
            Sport,
            Art,
            Cooking,
            Fashion,
            Politics,
            Science,
            Music,
            Travel,
            Movies,
            Shopping,
            Reading,
            Other
        }

        public Contact()
        {
            ContactAddress = new Address();
            Relatives = new List<Relative>();
            Appointments = new List<Appointment>();
            Notes = new List<Note>();
        }

        public string Image { get; set; }

        public string Email { get; set; }

        public string HomeTelephone { get; set; }

        public string MobileTelephone { get; set; }

        public string Ssn { get; set; }

        public Address ContactAddress { get; set; }

        public List<Relative> Relatives = new List<Relative>();

        public List<Appointment> Appointments { get; set; }

        public List<Contact> Contacts = new List<Contact>();

        public override string ToString()
        {
            return FirstName + " " + Surname;
        }
    }
}
