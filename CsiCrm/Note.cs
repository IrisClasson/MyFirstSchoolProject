using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsiCrm
{
    [Serializable]
    public class Note
    {

        public Note()
        {
            DateAdded = DateTime.Now;
        }
        public string NoteText { get; set; }

       

        public DateTime DateAdded { get; set; }

        public string Subject { get; set; }

        public override string ToString()
        {
            return Subject;
        }
    }
}
