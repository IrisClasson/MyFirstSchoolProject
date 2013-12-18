using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace CsiCrm
{
    public static class Controller
    {
        static Controller()
        {
            AllAppointments = new List<Appointment>();
            BirthdayGreetings = new List<Birthday>();
            AllContacts = new List<Contact>();
            AllNotesEver = new List<Note>();
            AllCustomers = new List<Customer>();
        }

        public static List<Customer> AllCustomers { get; private set; }

        public static List<Contact> AllContacts { get; set; }

        public static List<Birthday> BirthdayGreetings { get; set; }

        public static List<Appointment> AllAppointments { get; set; }

        public static List<Note> AllNotesEver { get; private set; }

        public static void AddNote(Note noteToAdd)
        {
            AllNotesEver.Add(noteToAdd);
            SaveNotes();
        }

        public static void SaveNotes()
        {
            var formatter = new BinaryFormatter();

            using (Stream output = File.Create("Notes.xml"))
            {
                formatter.Serialize(output, AllNotesEver);
            }
        }

        public static void LoadNotes()
        {
            var formatter = new BinaryFormatter();
            using (var input = File.Open("Notes.xml", FileMode.OpenOrCreate))
            {
                if (input.Length > 0)
                    AllNotesEver = (List<Note>)formatter.Deserialize(input);
            }
        }

        public static string ImageDir(string foldername)
        {
            var saveDir = Path.Combine(Environment.CurrentDirectory, foldername);

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            return saveDir;
        }

        public delegate void EventsMethodType(Appointment appointment);

        public static event EventsMethodType EventAdded;

        public static void AddAppointment(Appointment appointment)
        {
            AllAppointments.Add(appointment);
            if (EventAdded != null)
                EventAdded(appointment);
        }


        public static bool AddCustomer(Customer customerToAdd)
        {
            bool result = true;
            if (AllCustomers.Any(customer => customer.Name == customerToAdd.Name))
            {
                result = false;
            }
            else
            {
                AllCustomers.Add(customerToAdd);
                SaveCustomers();
            }
            return result;
        }

        public static string AvailableImageFormats()
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            var formatName = new List<string>();
            var fileExtension = new List<string>();
            string availableImageFormats = "";

            foreach (var imageCodecInfo in codecs)
            {
                formatName.Add(imageCodecInfo.FormatDescription);
                fileExtension.Add(imageCodecInfo.FilenameExtension);
            }

            for (var numberOfFormatsCounter = 0; numberOfFormatsCounter < formatName.Count; numberOfFormatsCounter++)
            {
                availableImageFormats += numberOfFormatsCounter != formatName.Count - 1
                            ? "{0}|{1}|".Args(formatName[numberOfFormatsCounter], fileExtension[numberOfFormatsCounter])
                            : "{0}|{1}".Args(formatName[numberOfFormatsCounter], fileExtension[numberOfFormatsCounter]);
            }
            return availableImageFormats;
        }

        public static string AddPicture(string pictureDirectory, string fileName)
        {
            var availableImageFormats = AvailableImageFormats();
            var savePath = "";
            var openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = availableImageFormats,
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var extension = Path.GetExtension(openFileDialog1.FileName);
                    savePath = ImageDir(pictureDirectory) + fileName + extension;
                    File.Copy(openFileDialog1.FileName, savePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            return savePath;
        }

        internal static void RemoveCustomer(Customer customer)
        {
            AllCustomers.Remove(customer);
            SaveCustomers();
        }

        internal static void SaveCustomers()
        {
            var formatter = new BinaryFormatter();

            using (Stream output = File.Create("Company.xml"))
            {
                formatter.Serialize(output, AllCustomers);
            }
        }

        internal static void LoadCustomers()
        {
            var formatter = new BinaryFormatter();

            using (var input = File.Open("Company.xml", FileMode.OpenOrCreate))
            {
                if (input.Length > 0)
                    AllCustomers = (List<Customer>)formatter.Deserialize(input);
            }
        }

        internal static string ChangeLogoName(string oldFileName, string newFileName)
        {
            var extension = Path.GetExtension(oldFileName);
            var oldPath = Path.GetDirectoryName(oldFileName);

            newFileName = oldPath + @"\" + newFileName + extension;
            File.Move(oldFileName, newFileName);
            return newFileName;
        }


        internal static void ChangeLogo(Customer customerToAddLogoTo)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = AvailableImageFormats(),
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Delete(customerToAddLogoTo.Logo);
                    var extension = Path.GetExtension(openFileDialog1.FileName);
                    var savePath = ImageDir("logo") + customerToAddLogoTo.Name + extension;
                    File.Copy(openFileDialog1.FileName, savePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        //=========================================================================================================00

        internal static void ChangePicture(Contact contactToAddPicTo)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = AvailableImageFormats(),
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Delete(contactToAddPicTo.Image);
                    var extension = Path.GetExtension(openFileDialog1.FileName);
                    var savePath = ImageDir("logo") + contactToAddPicTo.FirstName + extension;
                    File.Copy(openFileDialog1.FileName, savePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        internal static void AddBirthdayGreetings(Birthday birthday)
        {
            if (!BirthdayGreetings.Any(day => day.WhosBirthday == birthday.WhosBirthday))
                BirthdayGreetings.Add(birthday);
        }
    }
}
