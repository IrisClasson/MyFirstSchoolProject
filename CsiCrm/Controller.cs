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

        public static List<Appointment> AllAppointments = new List<Appointment>();
        
        private static List<Customer> _allCustomers = new List<Customer>();
        
        public static List<Contact> AllContacts = new List<Contact>();
        
        public static List<Birthday> BirthdayGreetings = new List<Birthday>();

        private static List<Note> _allNotesEver = new List<Note>();

        public static List<Note> AllNotesEver()
        {
            return _allNotesEver;
        }

        public static void AddNote(Note noteToAdd)
        {
            _allNotesEver.Add(noteToAdd);
            SaveNotes();
        }

        public static void SaveNotes()
        {
            var formatter = new BinaryFormatter();

            Stream output = File.Create("Notes.xml");

            formatter.Serialize(output, _allNotesEver);

            output.Close();
        }

        public static void LoadNotes()
        {
            var formatter = new BinaryFormatter();
            var input = File.Exists("Notes.xml") ? File.OpenRead("Notes.xml") : File.Create("Notes.xml");
            if (input.Length > 0)
                _allNotesEver = (List<Note>)formatter.Deserialize(input);

            input.Close();

        }

        public static string ImageDir(string foldername)
        {
            var outputDir = Environment.CurrentDirectory;

            var saveDir = new DirectoryInfo(outputDir + @"\{0}\".Args(foldername));

            if (!saveDir.Exists)
            {
                Directory.CreateDirectory(outputDir + @"\{0}\".Args(foldername));
            }
            return saveDir.ToString();
        }

        public delegate void EventsMethodType(Appointment appointment);

        public static event EventsMethodType EventAdded;

        public static void AddAppointment(Appointment appointment)
        {

            AllAppointments.Add(appointment);
            if (EventAdded != null)
                EventAdded(appointment);
        }

        public static List<Customer> AllCustomers()
        {
            return _allCustomers;
        }

        public static bool AddCustomer(Customer customerToAdd)
        {
            bool result = true;
            if (_allCustomers.Where(customer => customer.Name == customerToAdd.Name).Count() != 0)
            {
                result = false;
            }
            else
            {
                _allCustomers.Add(customerToAdd);
                SaveCustomers();
            }
            return result;


        }


        public static string AvalibleImageFormats()
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            var formatName = new List<string>();
            var fileExtension = new List<string>();
            string avalibleImageFormats = "";

            foreach (var imageCodecInfo in codecs)
            {
                formatName.Add(imageCodecInfo.FormatDescription);
                fileExtension.Add(imageCodecInfo.FilenameExtension);
            }

            for (var numberOfFormatsCounter = 0; numberOfFormatsCounter < formatName.Count; numberOfFormatsCounter++)
            {
                avalibleImageFormats += numberOfFormatsCounter != formatName.Count - 1
                            ? "{0}|{1}|".Args(formatName[numberOfFormatsCounter], fileExtension[numberOfFormatsCounter])
                            : "{0}|{1}".Args(formatName[numberOfFormatsCounter], fileExtension[numberOfFormatsCounter]);
            }
            return avalibleImageFormats;
        }

        public static string AddPicture(string pictureDirectory, string fileName)
        {
            var avalibleImageFormats = AvalibleImageFormats();
            var savePath = "";
            var openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = avalibleImageFormats,
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
            _allCustomers.Remove(customer);
            SaveCustomers();
        }

        internal static void SaveCustomers()
        {
            var formatter = new BinaryFormatter();

            Stream output = File.Create("Company.xml");

            formatter.Serialize(output, _allCustomers);

            output.Close();
        }

        internal static void LoadCustomers()
        {
            var formatter = new BinaryFormatter();

            
            var input = File.Exists("Company.xml") ? File.OpenRead("Company.xml") : File.Create("Company.xml");
            if (input.Length > 0)
                 _allCustomers = (List<Customer>)formatter.Deserialize(input);

            input.Close();

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
                Filter = AvalibleImageFormats(),
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
                Filter = AvalibleImageFormats(),
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
            if (BirthdayGreetings.Where(day => day.WhosBirthday == birthday.WhosBirthday).Count() < 1)
                BirthdayGreetings.Add(birthday);
        }
    }
}
