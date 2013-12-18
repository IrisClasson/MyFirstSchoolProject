using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CsiCrm
{
    internal static class FileManager
    {
        internal static void SaveFileAsXml()
        {
            var formatter = new BinaryFormatter();

            using (Stream output = File.Create("Appointment.xml"))
            {
                formatter.Serialize(output, Controller.AllAppointments);    
            }
        }

        internal static void SaveContacts()
        {
            var formatter = new BinaryFormatter();

            using (Stream output = File.Create("Contacts.xml"))
            {
                formatter.Serialize(output, Controller.AllContacts);
            }
        }

        internal static List<Appointment> GetAllAppointments()
        {
            var appointment = new List<Appointment>();
            var formatter = new BinaryFormatter();

            using (var input = File.Open("Appointment.xml", FileMode.OpenOrCreate))
            {
                if (input.Length > 0)
                    appointment = (List<Appointment>)formatter.Deserialize(input);
            }

            return appointment;
        }

        internal static List<Contact> GetContactsFromXml()
        {
            var allContacts = new List<Contact>();
            var formatter = new BinaryFormatter();

            using (var input = File.Open("Contacts.xml", FileMode.OpenOrCreate))
            {
                if (input.Length > 0)
                    allContacts = (List<Contact>)formatter.Deserialize(input);
            }

            return allContacts;
        }

        internal static void SaveBirthdays()
        {
            var formatter = new BinaryFormatter();

            using (Stream output = File.Create("Birthdays.xml"))
            {
                formatter.Serialize(output, Controller.BirthdayGreetings);
            }
        }

        internal static List<Birthday> GetBirthdaysFromXml()
        {
            var birthdayGreetings =  new List<Birthday>();
            var formatter = new BinaryFormatter();

            using (var input = File.Open("Birthdays.xml", FileMode.OpenOrCreate))
            {
                if (input.Length > 0)
                    birthdayGreetings = (List<Birthday>)formatter.Deserialize(input);
            }

            return birthdayGreetings;
        }
    }
}
