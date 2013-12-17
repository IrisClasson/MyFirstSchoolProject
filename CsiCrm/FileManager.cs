using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CsiCrm
{
    public class FileManager
    {
        internal static void SaveFileAsXml()
        {
            var formatter = new BinaryFormatter();

            Stream output = File.Create("Appointment.xml");

            formatter.Serialize(output, Controller.AllAppointments);

            output.Close();
        }

        internal static void SaveContacts()
        {
            var formatter = new BinaryFormatter();

            Stream output = File.Create("Contacts.xml");

            formatter.Serialize(output, Controller.AllContacts);

            output.Close();
        }

        internal static List<Appointment> GetAllAppointments()
        {
            var appointment = new List<Appointment>();
            var formatter = new BinaryFormatter();
            var input = File.Exists("Appointment.xml") ? File.OpenRead("Appointment.xml") : File.Create("Appointment.xml");
            if (input.Length > 0)
                appointment = (List<Appointment>)formatter.Deserialize(input);
            input.Close();

            return appointment;
        }

        internal static List<Contact> GetContactsFromXml()
        {
            var allContacts = new List<Contact>();
            var formatter = new BinaryFormatter();
            var input = File.Exists("Contacts.xml") ? File.OpenRead("Contacts.xml") : File.Create("Contacts.xml");
            if (input.Length>0)
                allContacts = (List<Contact>)formatter.Deserialize(input);

            input.Close();

            return allContacts;
        }

        internal static void SaveBirthdays()
        {
            var formatter = new BinaryFormatter();

            Stream output = File.Create("Birthdays.xml");

            formatter.Serialize(output, Controller.BirthdayGreetings);

            output.Close();
        }

        internal static List<Birthday> GetBirthdaysFromXml()
        {
            var birthdayGreetings =  new List<Birthday>();
            var formatter = new BinaryFormatter();
            var input = File.Exists("Birthdays.xml") ? File.OpenRead("Birthdays.xml") : File.Create("Birthdays.xml");
            if (input.Length>0)
                birthdayGreetings = (List<Birthday>) formatter.Deserialize(input);

            input.Close();

            return birthdayGreetings;
        }
    }
}
