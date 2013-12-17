using System;

namespace CsiCrm
{

    [Serializable]
    public class Appointment
    {


        public enum AppointmentType
        {
            Conference,
            Meeting,
            Telephone,
            Other
        }
        public Appointment()
        {
            Location = new Address();
            Notes = new Note();

        }

        public AppointmentType TypeOfAppointment { get; set; }

        public Address Location { get; set; }

        public Customer Company { get; set; }

        public DateTime DateTimeStart { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime TimeEnd { get; set; }

        public string Subject { get; set; }

        public Note Notes { get; set; }

        public enum ReminderType
        {
            Email,
            Phone,
            Post,
            Facebook,
            Sms
        }

        public ReminderType TypeOfReminder { get; set; }

        public DateTime ReminderDate { get; set; }

        public DateTime ReminderTime { get; set; }

        public override string ToString()
        {
            return Subject;
        }


    }

}
