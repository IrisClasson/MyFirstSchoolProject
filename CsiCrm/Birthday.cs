using System;

namespace CsiCrm
{
    [Serializable]
    public class Birthday
    {
        public DateTime DateTimeStart { get; set; }

        public Person WhosBirthday { get; set; }
        public Birthday (Person addMyDay)
        {
            WhosBirthday = addMyDay;
        }

        public override string ToString()
        {
            return DateTimeStart.ToShortDateString() + " " + WhosBirthday.FirstName;
        }
    }
}
