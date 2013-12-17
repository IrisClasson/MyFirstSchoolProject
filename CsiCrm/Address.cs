using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsiCrm
{
    [Serializable]
    public class Address
    {
        public string Street {get;set;}
        public string PostNumber { get; set; }
        public string City {get;set;}
        public string Country { get; set; }
    }
}
