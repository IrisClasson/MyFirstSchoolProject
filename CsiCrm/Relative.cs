using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsiCrm
{
    [Serializable]
    public class Relative : Person
    {
        public enum RelativeType
        {
            Partner,
            Child,
            Other
        }

        public RelativeType Relation { get; set; }
        public Relative()
        {
            Notes = new List<Note>();
        }

        public override string ToString()
        {
            return FirstName + " Relation: " + Relation;
        }
    }
}
