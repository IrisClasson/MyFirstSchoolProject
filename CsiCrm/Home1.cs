#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#endregion

namespace CsiCrm
{
    public partial class Home1 : Form
    {
        private Appointment _appointmentToEdit;
        private Contact _contactEdit;
        private string _logo;

        public Home1()
        {
            InitializeComponent();
            Controller.AllContacts = FileManager.GetContactsFromXml();
            Controller.LoadCustomers();
            Controller.LoadNotes();
            Controller.AllAppointments = FileManager.GetAllAppointments();
            Controller.BirthdayGreetings = FileManager.GetBirthdaysFromXml();
            cbCompanies.DataSource = Controller.AllCustomers();
            cbCompanies.SelectedItem = null;
            cbCustomerType.DataSource = Enum.GetNames(typeof (Customer.CustomerType)).ToList();
            cbCustomerType.SelectedItem = null;
            cbAppoinmentType.DataSource = Enum.GetValues(typeof (Appointment.AppointmentType));
            cbReminderType.DataSource = Enum.GetValues(typeof (Appointment.ReminderType));
            cbCompany.DataSource = Controller.AllCustomers();
            dgwCompanyContacts.DataSource = Controller.AllContacts.ToList();
            HideColumnsInContactDataGridView(dgwCompanyContacts);
            dataGridViewContacts.DataSource = Controller.AllContacts.ToList();
            HideColumnsInContactDataGridView(dataGridViewContacts);
            dgvViewAppoinments.DataSource = Controller.AllAppointments.ToList();
            var dataGridViewColumn = dgvViewAppoinments.Columns["DateTimeStart"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DisplayIndex = 0;
            var source = new BindingSource
                             {
                                 DataSource =
                                     Controller.AllAppointments.ToList().Where(
                                         apointment => apointment.DateTimeStart.Date >= DateTime.Now.Date).ToList()
                             };
            lbEvents.DataSource = source;
            Controller.EventAdded += OnEventAdded;
            clbInterests.DataSource = Enum.GetNames(typeof (Contact.Interest));
            cbCompanySearch.DataSource = Controller.AllCustomers();
            CreateRows();
            SetTimeInCalendar();

        }

        private void OnEventAdded(Appointment appointment)
        {
            var source = new BindingSource
                             {
                                 DataSource =
                                     Controller.AllAppointments.ToList().Where(
                                         apointment => apointment.DateTimeStart.Date >= DateTime.Now.Date).ToList()
                             };
            lbEvents.DataSource = source;
        }


        //----------------------------------------Add appoinment code--------------------------------------------------------


        private void BtHomeClick(object sender, EventArgs e)
        {
            tabCtrlUserManual.Hide();
            tabCtrlJournal.Hide();
            tabCtrlAppoinments.Hide();
            tabCtrlCompanies.Hide();
            tabCtrlContact.Hide();
            pbHome.Show();
        }


        private void BtSaveClick(object sender, EventArgs e)
        {
            var appointment = new Appointment
                                  {
                                      DateTimeStart = dtpAppointmentDateStart.Value.Date,
                                      TimeStart = dtpAppointmentTimeStart.Value,
                                      TimeEnd = dtpAppointmentTimeEnd.Value,
                                      Subject = tbSubject.Text,
                                      ReminderTime = dtpReminderTime.Value,
                                      Notes = {NoteText = rtbNotes.Text},
                                      Location =
                                          {
                                              Street = tbStreet.Text,
                                              PostNumber = tbPostnumber.Text,
                                              City = tbCity.Text,
                                              Country = tbCountry.Text
                                          },
                                      TypeOfAppointment = (Appointment.AppointmentType) cbAppoinmentType.SelectedItem,
                                      Company = (Customer) cbCompany.SelectedItem,
                                      TypeOfReminder = (Appointment.ReminderType) cbReminderType.SelectedItem,
                                      ReminderDate = dtpReminderDate.Value.Date,
                                  };


            appointment.ReminderTime = dtpReminderTime.Value;
            if (CheckDateTimeCollision(appointment.TimeStart, appointment.TimeEnd, appointment.DateTimeStart))
            {
                if (tbSubject.Text == "")
                {
                    errorProvider1.SetError(tbSubject, "Please enter subject");
                }

                else
                {
                    errorProvider1.Clear();
                    Controller.AddAppointment(appointment);
                    ClearAppointmentBoxes();
                }
            }

            else
            {
                errorProvider1.SetError(dtpAppointmentDateStart, "Not available date or time, check calendar");
            }


            FileManager.SaveFileAsXml();
        }

        private void ClearAppointmentBoxes()
        {
            tbStreet.Clear();
            tbPostnumber.Clear();
            tbCity.Clear();
            tbCountry.Clear();
            tbSubject.Clear();
            rtbNotes.Clear();
        }

        private void CbCompanySelectedIndexChanged(object sender, EventArgs e)
        {
            clbContacts.DataSource = null;
            if (cbCompany.SelectedItem != null)
                clbContacts.DataSource = ((Customer) cbCompany.SelectedItem).Contacts;
        }

        //-------------------------------------------Contact Form Mthods------------------------------------------------------

        private void HideColumnsInContactDataGridView(DataGridView dataGridView)
        {
            dataGridView.DataSource = null;
            dataGridView.DataSource = Controller.AllContacts.ToList();

            var dataGridViewColumn = dataGridView.Columns["Image"];
            if (dataGridViewColumn != null) dataGridViewColumn.Visible = false;
            dataGridViewColumn = dataGridView.Columns["ContactAddress"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.Visible = false;
            dataGridViewColumn = dataGridView.Columns["FirstName"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DisplayIndex = 0;
            dataGridViewColumn = dataGridView.Columns["SurName"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DisplayIndex = 1;
            dataGridViewColumn = dataGridView.Columns["Ssn"];
            if (dataGridViewColumn != null) dataGridViewColumn.DisplayIndex = 2;
            dataGridViewColumn = dataGridView.Columns["Email"];
            if (dataGridViewColumn != null) dataGridViewColumn.DisplayIndex = 3;
            dataGridViewColumn = dataGridView.Columns["HomeTelephone"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DisplayIndex = 4;
            dataGridViewColumn = dataGridView.Columns["MobileTelephone"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DisplayIndex = 5;
            dataGridViewColumn = dataGridView.Columns["DateOfBirth"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DisplayIndex = 6;
        }

        private string AddLogo(string fileName)
        {
            const string pictureType = "Logo";
            return Controller.AddPicture(pictureType, fileName);
        }

        private void ClearContactInformation()
        {
            tbxName.Clear();
            tbxSurname.Clear();
            tbxSsn.Clear();
            tbxEmail.Clear();
            tbxHomeNr.Clear();
            tbxMobile.Clear();
            tbxStreet.Clear();
            tbxPostnr.Clear();
            tbxCity.Clear();
            tbxCountry.Clear();
        }

        private bool CheckAddContactButton()
        {
            bool okInput = false;
            {
                if (tbxName.TextLength > 0
                    && tbxSurname.TextLength > 0
                    && tbxSsn.TextLength > 0
                    )
                    okInput = true;

                return okInput;
            }
        }

        //What a lovely little method :)
        private bool CheckSsn(string ssn)
        {
            return Controller.AllContacts.All(contact => ssn != contact.Ssn);
        }


        private bool CheckSaveRelativeButton()
        {
            bool checkbutton = false;
            {
                if (tbxRelaName.TextLength > 0
                    && tbxRelaSurname.TextLength > 0
                    && cbContactToAddDetail != null
                    && cbxRelType.Text == (string) cbxRelType.SelectedItem
                    )
                    checkbutton = true;

                return checkbutton;
            }
        }

        private void ClearAddDetails()
        {
            tbxRelaName.Clear();
            tbxRelaSurname.Clear();
            cbxRelType.SelectedItem = null;
        }

        private void GetSelectedContactToEdit()
        {
            if (dataGridViewContacts.SelectedRows.Count > 0)
            {
                _contactEdit =
                    Controller.AllContacts.Where(
                        contact => contact.Ssn == (string) dataGridViewContacts.SelectedRows[0].Cells["Ssn"].Value).
                        First();

                tbxName.Text = _contactEdit.FirstName;
                tbxSurname.Text = _contactEdit.Surname;
                dtpRelDateOfBirth.Value = _contactEdit.DateOfBirth;
                tbxSsn.Text = _contactEdit.Ssn;
                tbxEmail.Text = _contactEdit.Email;
                tbxHomeNr.Text = _contactEdit.HomeTelephone;
                tbxMobile.Text = _contactEdit.MobileTelephone;
                tbxStreet.Text = _contactEdit.ContactAddress.Street;
                tbxPostnr.Text = _contactEdit.ContactAddress.PostNumber;
                tbxCity.Text = _contactEdit.ContactAddress.City;
                tbxCountry.Text = _contactEdit.ContactAddress.Country;
                if (!string.IsNullOrEmpty(_contactEdit.Image))
                {
                    var ms = new MemoryStream(File.ReadAllBytes(_contactEdit.Image));
                    pbxImage.Image = Image.FromStream(ms);
                    pbxImage.Show();
                }
            }
        }


        //-------------------------------------------------Contact Events--------------------------------------------------


        private void BtContactsClick(object sender, EventArgs e)
        {
            tabCtrlUserManual.Hide();
            tabCtrlContact.Show();
            cbxRelType.DataSource = Enum.GetNames((typeof (Relative.RelativeType)));
            lbRelaName.Hide();
            lbRelaSurname.Hide();
            lbRelDob.Hide();
            dtpRelDateOfBirth.Hide();
            lbRelType.Hide();
            tbxRelaName.Hide();
            tbxRelaSurname.Hide();
            cbxRelType.Hide();
            tabCtrlAppoinments.Hide();
            tabCtrlCompanies.Hide();
            btSaveEditedContact.Hide();
            pbHome.Hide();
            btCreateRela.Hide();
            if (Controller.AllContacts.Count == 0)
                btDeleteContact.Hide();
            tabCtrlJournal.Hide();
        }

        internal void BtCreateClick(object sender, EventArgs e)
        {
            if (CheckAddContactButton() && CheckSsn(tbxSsn.Text))
            {
                _contactEdit = new Contact
                                   {
                                       FirstName = tbxName.Text,
                                       Surname = tbxSurname.Text,
                                       DateOfBirth = dtpDateOfBirth.Value.Date,
                                       Ssn = tbxSsn.Text,
                                       Email = tbxEmail.Text,
                                       HomeTelephone = tbxHomeNr.Text,
                                       MobileTelephone = tbxMobile.Text,
                                       ContactAddress = new Address
                                                            {
                                                                Street = tbxStreet.Text,
                                                                PostNumber = tbxPostnr.Text,
                                                                City = tbxCity.Text,
                                                                Country = tbxCountry.Text
                                                            }
                                   };
                Controller.AllContacts.Add(_contactEdit);
                Controller.AddBirthdayGreetings(new Birthday(_contactEdit)
                                                    {
                                                        DateTimeStart = dtpDateOfBirth.Value.Date
                                                    });

                FileManager.SaveContacts();

                dataGridViewContacts.DataSource = null;
                dataGridViewContacts.DataSource = Controller.AllContacts.ToList();
                HideColumnsInContactDataGridView(dataGridViewContacts);
                errorProvider.Clear();
                ClearContactInformation();
                btDeleteContact.Show();
                _contactEdit = null;
            }
            else if (CheckAddContactButton() == false)
                errorProvider.SetError(btCreate,
                                       "Fill in First Name, Surname, and" + Environment.NewLine +
                                       " Social Security number to create contact");

            else if (CheckSsn(tbxSsn.Text) == false)
                errorProvider.SetError(btCreate,
                                       "The social security number you entered" + Environment.NewLine +
                                       "already exists in your contact list");
        }

        private void TbxNameValidating(object sender, CancelEventArgs e)
        {
            if (tbxName.Text == "")
            {
                errorProvider.SetError(tbxName, "Please enter First Name");
            }
            else
                errorProvider.Clear();
        }

        private void TbxSurnameValidating(object sender, CancelEventArgs e)
        {
            if (tbxSurname.Text == "")
            {
                errorProvider.SetError(tbxSurname, "Please enter Surname");
            }
            else
                errorProvider.Clear();
        }

        private void TbxSsnValidating(object sender, CancelEventArgs e)
        {
            if (tbxSsn.Text == "")
            {
                errorProvider.SetError(tbxSsn, "Please enter your" + Environment.NewLine + "Social Security Number");
            }

            else
                errorProvider.Clear();
        }

        private void TbxStreetValidating(object sender, CancelEventArgs e)
        {
            if (tbxStreet.Text == "")
            {
                errorProvider.SetError(tbxStreet, "Please enter Street");
            }
            else
                errorProvider.Clear();
        }

        private void TbxPostnrValidating(object sender, CancelEventArgs e)
        {
            if (tbxPostnr.Text == "")
            {
                errorProvider.SetError(tbxPostnr, "Please enter Postnr");
            }

            else errorProvider.Clear();
        }

        private void TbxCityValidating(object sender, CancelEventArgs e)
        {
            if (tbxCity.Text == "")
            {
                errorProvider.SetError(tbxCity, "Please enter City");
            }
            else
                errorProvider.Clear();
        }

        private void TbxCountryValidating(object sender, CancelEventArgs e)
        {
            if (tbxCountry.Text == "")
            {
                errorProvider.SetError(tbxCountry, "Please enter Country");
            }
            else
                errorProvider.Clear();
        }

        private void TbxEmailValidating(object sender, CancelEventArgs e)
        {
            if (tbxEmail.Text == "" || !tbxEmail.Text.Contains("@"))
            {
                errorProvider.SetError(tbxEmail, "Please enter a valid Email");
            }
            else
                errorProvider.Clear();
        }

        private void BtUploadClick(object sender, EventArgs e)
        {
            _contactEdit =
                Controller.AllContacts.Where(
                    contact => contact.Ssn == (string) dataGridViewContacts.SelectedRows[0].Cells["Ssn"].Value).
                    First();

            _logo = AddProfilePic(tbxSsn.Text);
            if (!string.IsNullOrEmpty(_logo))
            {
                var ms = new MemoryStream(File.ReadAllBytes(_logo));
                pbxImage.Image = Image.FromStream(ms);
                _contactEdit.Image = _logo;
            }

            else
                FileManager.SaveContacts();
            _logo = String.Empty;
        }

        private string AddProfilePic(string fileName)
        {
            const string pictureType = "Profile";
            return Controller.AddPicture(pictureType, fileName);
        }

        private void CbxAddRelCheckedChanged(object sender, EventArgs e)
        {
            if (cbxAddRel.Checked)
            {
                lbRelaName.Show();
                lbRelaSurname.Show();
                lbRelDob.Show();
                btCreateRela.Show();
                dtpRelDateOfBirth.Show();
                lbRelType.Show();
                tbxRelaName.Show();
                tbxRelaSurname.Show();
                cbxRelType.Show();
                btCreateRela.Show();
            }
            else
            {
                lbRelaName.Hide();
                lbRelaSurname.Hide();
                lbRelDob.Hide();
                dtpRelDateOfBirth.Hide();
                lbRelType.Hide();
                tbxRelaName.Hide();
                tbxRelaSurname.Hide();
                cbxRelType.Hide();
                btCreateRela.Hide();
            }
        }

        private void BtCreateRelaClick1(object sender, EventArgs e)
        {
            var contactToEdit = (Contact) cbContactToAddDetail.SelectedItem;

            if (CheckSaveRelativeButton() && cbContactToAddDetail.SelectedItem != null)
            {
                var relativeToAdd = new Relative
                                        {
                                            FirstName = tbxRelaName.Text,
                                            Surname = tbxRelaSurname.Text,
                                            DateOfBirth = dtpRelDateOfBirth.Value.Date,
                                            Relation =
                                                (Relative.RelativeType)
                                                Enum.Parse(typeof (Relative.RelativeType),
                                                           cbxRelType.SelectedItem.ToString())
                                        };
                var birthDay = new Birthday(relativeToAdd) {DateTimeStart = dtpRelDateOfBirth.Value.Date};
                Controller.AddBirthdayGreetings(birthDay);
                contactToEdit.Relatives.Add(relativeToAdd);
                lbxRelatives.DataSource = null;
                lbxRelatives.DataSource = contactToEdit.Relatives.ToList();
                errorProvider.Clear();
                ClearAddDetails();
            }

            else
                errorProvider.SetError(btCreateRela,
                                       "Must Select or Create Contact" + Environment.NewLine + "and fill in all fields");
            FileManager.SaveContacts();
            FileManager.SaveBirthdays();
        }

        private void BtEditClick1(object sender, EventArgs e)
        {
            tabCtrlContact.SelectedTab = tabViewContacts;
            btDeleteContact.Show();
        }

        private void BtSaveEditedContactClick(object sender, EventArgs e)
        {
            _contactEdit.FirstName = tbxName.Text;
            _contactEdit.Surname = tbxSurname.Text;
            _contactEdit.DateOfBirth = dtpDateOfBirth.Value.Date;
            _contactEdit.Ssn = tbxSsn.Text;
            _contactEdit.Email = tbxEmail.Text;
            _contactEdit.HomeTelephone = tbxHomeNr.Text;
            _contactEdit.MobileTelephone = tbxMobile.Text;
            _contactEdit.ContactAddress.Street = tbxStreet.Text;
            _contactEdit.ContactAddress.PostNumber = tbxPostnr.Text;
            _contactEdit.ContactAddress.City = tbxCity.Text;
            _contactEdit.ContactAddress.Country = tbxCountry.Text;


            FileManager.SaveContacts();
            FileManager.SaveBirthdays();
            pbxImage.Image = null;
            dataGridViewContacts.DataSource = null;
            dataGridViewContacts.DataSource = Controller.AllContacts.ToList();
            HideColumnsInContactDataGridView(dataGridViewContacts);
            ClearContactInformation();
            btSaveEditedContact.Hide();
            btEdit.Show();
        }

        private void BtDeleteContactClick(object sender, EventArgs e)
        {
            if (dataGridViewContacts.SelectedRows.Count > 0)
            {
                Contact contactToRemove =
                    Controller.AllContacts.Where(
                        contact => contact.Ssn == (string) dataGridViewContacts.SelectedRows[0].Cells["Ssn"].Value).
                        First();

                Controller.AllContacts.Remove(contactToRemove);

                FileManager.SaveContacts();

                dataGridViewContacts.DataSource = null;
                dataGridViewContacts.DataSource = Controller.AllContacts.ToList();
                HideColumnsInContactDataGridView(dataGridViewContacts);
            }
        }

        private void BtSelectContactClick(object sender, EventArgs e)
        {
            GetSelectedContactToEdit();
            tabCtrlContact.SelectedTab = tabAddContact;
            btEdit.Hide();
            btSaveEditedContact.Show();
        }

        private void CbContactToAddDetailSelectedIndexChanged(object sender, EventArgs e)
        {
            clbInterests.DataSource = null;
            clbInterests.DataSource = Enum.GetNames(typeof (Contact.Interest));

            if (cbContactToAddDetail.SelectedItem != null)
            {
                lbxRelatives.DataSource = null;
                lbxRelatives.DataSource = ((Contact) cbContactToAddDetail.SelectedItem).Relatives;
                foreach (var interest in ((Contact) cbContactToAddDetail.SelectedItem).Inrests)
                {
                    clbInterests.SelectedIndex =
                        clbInterests.FindStringExact(Enum.GetName(typeof (Contact.Interest), interest));
                    clbInterests.SetItemCheckState(clbInterests.SelectedIndex, CheckState.Checked);
                }
            }
        }

        private void TabCtrlContactSelectedIndexChanged1(object sender, EventArgs e)
        {
            if (tabCtrlContact.SelectedTab == tabAddDetails)
            {
                cbContactToAddDetail.DataSource = null;
                cbContactToAddDetail.DataSource = Controller.AllContacts;
            }
            if (tabCtrlContact.SelectedTab != tabContactNote) return;
            cbContactNoteContact.DataSource = null;
            cbContactNoteRelative.DataSource = null;
            cbContactNoteContact.DataSource = Controller.AllContacts;
            cbContactNoteContact.SelectedItem = null;
        }

        private void BtCompaniesClick(object sender, EventArgs e)
        {
            tabCtrlCompanies.Show();
            tabCtrlJournal.Hide();
            tabCtrlAppoinments.Hide();
            tabCtrlContact.Hide();
            pbHome.Hide();
            tabCtrlUserManual.Hide();
        }

        private void TabCtrlAppoinmentsSelected(object sender, TabControlEventArgs e)
        {
            dgvViewAppoinments.DataSource = null;
            dgvViewAppoinments.DataSource = Controller.AllAppointments.ToList();
            HideColumnsInDataGridView();
        }

        private void HideColumnsInDataGridView()
        {
            var column = dgvViewAppoinments.Columns["TimeEnd"];
            if (column != null) column.Visible = false;
            column = dgvViewAppoinments.Columns["Location"];
            if (column != null) column.Visible = false;
            column = dgvViewAppoinments.Columns["TimeStart"];
            if (column != null)
                column.Visible = false;
            column = dgvViewAppoinments.Columns["TypeOfReminder"];
            if (column != null)
                column.Visible = false;
            column = dgvViewAppoinments.Columns["ReminderTime"];
            if (column != null)
                column.Visible = false;
            column = dgvViewAppoinments.Columns["ReminderDate"];
            if (column != null)
                column.Visible = false;
            column = dgvViewAppoinments.Columns["Notes"];
            if (column != null) column.Visible = false;
        }

        private void BtAddCompanyClick(object sender, EventArgs e)
        {
            Customer customer;
            bool nameIsOk = false;
            errorProvider.Clear();
            if (btAddCompany.Text == "Add")
            {
                customer = new Customer();
                nameIsOk = true;
            }
            else
            {
                customer = ((Customer) cbCompanies.SelectedItem);
                if (customer.Name == tbCompanyName.Text ||
                    Controller.AllCustomers().Where(customername => customername.Name == tbCompanyName.Text).Count() ==
                    0)
                {
                    nameIsOk = true;
                    Controller.RemoveCustomer(customer);
                }

                if (!string.IsNullOrEmpty(customer.Logo) && customer.Name != tbCompanyName.Text && nameIsOk)
                {
                    SetLogo(customer);
                }
            }
            if (!nameIsOk)
            {
                errorProvider.SetError(tbCompanyName, "Name already exists");
            }
            else
            {
                customer.Name = tbCompanyName.Text;
                customer.VatNumber = tbVatNr.Text;
                customer.WebPage = tbWebPage.Text;
                customer.Email = tbEmail.Text;
                customer.Telephone = tbTelephone.Text;
                customer.VisitAdress.Country = tbVisitCountry.Text;
                customer.VisitAdress.PostNumber = tbVisitPostNumber.Text;
                customer.VisitAdress.Street = tbVisitStreet.Text;
                customer.VisitAdress.City = tbVisitCity.Text;
                customer.PostalAdress.City = tbPostalCity.Text;
                customer.PostalAdress.Country = tbPostalCountry.Text;
                customer.PostalAdress.PostNumber = tbPostalPostNumber.Text;
                customer.PostalAdress.Street = tbPostalStreet.Text;

                if (cbCustomerType.SelectedItem != null)
                    customer.TypeOfCustomer =
                        (Customer.CustomerType) Enum.Parse(
                            typeof (Customer.CustomerType),
                            cbCustomerType.SelectedItem.ToString());


                //if (Controller.AddCustomer(customer))
                //{
                cbCompanies.UpdateInfo();
                cbCompanies.SelectedItem =
                    (Controller.AllCustomers()).Where(name => name.Name == customer.Name).First();
                //}
                //else
                //{
                //    errorProvider.SetError(tbCompanyName, "Name already exists");
                //}
            }
        }

        private void CbCompaniesSelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider.Clear();
            pbxCompanyLogo.Image = null;
            lbxContacts.DataSource = null;
            cbxSameAdress.CheckState = CheckState.Unchecked;
            btCompanyAddLogo.Text = "Add logo";

            if (cbCompanies.SelectedItem != null)
            {
                btAddCompany.Text = "Update";
                btAddContactToCompany.Show();
                Customer selectedCustomer =
                    (Controller.AllCustomers()).Where(name => name.Name == cbCompanies.Text).First();
                btDelCompany.Show();
                btClearSelection.Show();
                btCompanyAddLogo.Show();
                lbxContacts.DataSource = selectedCustomer.Contacts;
                tbCompanyName.Text = selectedCustomer.Name;
                tbVatNr.Text = selectedCustomer.VatNumber;
                cbCustomerType.SelectedItem = selectedCustomer.TypeOfCustomer.ToString();
                tbVisitCity.Text = selectedCustomer.VisitAdress.City;
                tbVisitCountry.Text = selectedCustomer.VisitAdress.Country;
                tbVisitPostNumber.Text = selectedCustomer.VisitAdress.PostNumber;
                tbVisitStreet.Text = selectedCustomer.VisitAdress.Street;
                tbPostalCity.Text = selectedCustomer.PostalAdress.City;
                tbPostalCountry.Text = selectedCustomer.PostalAdress.Country;
                tbPostalPostNumber.Text = selectedCustomer.PostalAdress.PostNumber;
                tbPostalStreet.Text = selectedCustomer.PostalAdress.Street;
                tbTelephone.Text = selectedCustomer.Telephone;
                tbEmail.Text = selectedCustomer.Email;
                tbWebPage.Text = selectedCustomer.WebPage;
                if (!string.IsNullOrEmpty(selectedCustomer.Logo))
                {
                    //pbxCompanyLogo.Image = Image.FromFile(selectedCustomer.Logo);
                    btCompanyAddLogo.Text = "Change";
                    var ms = new MemoryStream(File.ReadAllBytes(selectedCustomer.Logo));
                    pbxCompanyLogo.Image = Image.FromStream(ms);
                    pbxCompanyLogo.Show();
                }
                //using (Image image1 = Image.FromFile(selectedCustomer.Logo))
                //{
                //    pbxCompanyLogo.Image = image1;
                //    pbxCompanyLogo.Show();
                //}
            }
            else
            {
                btAddContactToCompany.Hide();
                btDelCompany.Hide();
                btClearSelection.Hide();
                btAddCompany.Text = "Add";
                ClearAllCompanyTb();
                pbxCompanyLogo.Hide();
                btCompanyAddLogo.Hide();
            }
        }

        private void ClearAllCompanyTb()
        {
            tbCompanyName.Text = string.Empty;
            tbVatNr.Text = string.Empty;
            cbCustomerType.SelectedItem = null;
            tbVisitCity.Text = string.Empty;
            tbVisitCountry.Text = string.Empty;
            tbVisitPostNumber.Text = string.Empty;
            tbVisitStreet.Text = string.Empty;
            tbPostalCity.Text = string.Empty;
            tbPostalCountry.Text = string.Empty;
            tbPostalPostNumber.Text = string.Empty;
            tbPostalStreet.Text = string.Empty;
            tbTelephone.Text = string.Empty;
            tbEmail.Text = string.Empty;
            tbWebPage.Text = string.Empty;
        }

        private void BtSaveEditedAppointmentClick(object sender, EventArgs e)
        {
            _appointmentToEdit.Location.Street = tbStreet.Text;
            _appointmentToEdit.Location.PostNumber = tbPostnumber.Text;
            _appointmentToEdit.Location.City = tbCity.Text;
            _appointmentToEdit.Location.Country = tbCountry.Text;
            _appointmentToEdit.Subject = tbSubject.Text;
            _appointmentToEdit.TypeOfAppointment = (Appointment.AppointmentType) cbAppoinmentType.SelectedItem;
            _appointmentToEdit.Company = (Customer) cbCompany.SelectedItem;
            _appointmentToEdit.TypeOfReminder = (Appointment.ReminderType) cbReminderType.SelectedItem;
            _appointmentToEdit.ReminderDate = dtpReminderDate.Value.Date;
            _appointmentToEdit.ReminderTime = dtpReminderTime.Value;
            _appointmentToEdit.Notes.NoteText = rtbNotes.Text;
            FileManager.SaveFileAsXml();
            dgvViewAppoinments.DataSource = null;
            dgvViewAppoinments.DataSource = Controller.AllAppointments.ToList();

            ClearAppointmentBoxes();
        }

        private void GetSelectedAppointment()
        {
            if (dgvViewAppoinments.SelectedRows.Count > 0)
            {
                _appointmentToEdit =
                    Controller.AllAppointments.Where(
                        appointment =>
                        appointment.Subject == (string) dgvViewAppoinments.SelectedRows[0].Cells["Subject"].Value).First
                        ();

                dtpAppointmentDateStart.Value = _appointmentToEdit.DateTimeStart;
                dtpAppointmentTimeStart.Value = _appointmentToEdit.TimeStart;
                dtpAppointmentTimeStart.Value = _appointmentToEdit.TimeEnd;
                tbSubject.Text = _appointmentToEdit.Subject;
                rtbNotes.Text = _appointmentToEdit.Notes.NoteText;
                dtpReminderTime.Value = _appointmentToEdit.ReminderTime;
                tbStreet.Text = _appointmentToEdit.Location.Street;
                tbPostnumber.Text = _appointmentToEdit.Location.PostNumber;
                tbCity.Text = _appointmentToEdit.Location.City;
                tbCountry.Text = _appointmentToEdit.Location.Country;
                cbAppoinmentType.SelectedItem = _appointmentToEdit.TypeOfAppointment;
                cbCompany.SelectedItem = _appointmentToEdit.Company;
                cbReminderType.SelectedItem = _appointmentToEdit.TypeOfReminder;
                dtpReminderDate.Value = _appointmentToEdit.ReminderDate;
                dtpReminderTime.Value = _appointmentToEdit.ReminderTime;
            }
        }

        private void GetAppointmentDetails()
        {
            if (dgvViewAppoinments.SelectedRows.Count > 0)
            {
                _appointmentToEdit =
                    Controller.AllAppointments.Where(
                        appointment =>
                        appointment.Subject == (string) dgvViewAppoinments.SelectedRows[0].Cells["Subject"].Value).First
                        ();

                lbViewAppointmentStartDate.Text = _appointmentToEdit.DateTimeStart.ToShortDateString();
                lbViewAppointmentStartTime.Text = _appointmentToEdit.TimeStart.ToShortTimeString();
                lbViewAppointmentEndTime.Text = _appointmentToEdit.TimeEnd.ToShortTimeString();
                lbViewAppointmentSubject.Text = _appointmentToEdit.Subject;
                rtxbViewAppointmentNote.Text = _appointmentToEdit.Notes.NoteText;
                lbViewAppointmentStreet.Text = _appointmentToEdit.Location.Street;
                lbViewAppointmentCity.Text = _appointmentToEdit.Location.PostNumber;
                lbViewAppointmentCity.Text = _appointmentToEdit.Location.City;
                lbViewAppointmentCountry.Text = _appointmentToEdit.Location.Country;
                lbViewAppointmentType.Text = _appointmentToEdit.TypeOfAppointment.ToString();
                lbViewAppointmentCompany.Text = _appointmentToEdit.Company.ToString();
                lbViewAppointmentReminderType.Text = _appointmentToEdit.TypeOfReminder.ToString();
                lbViewAppointmentReminderDate.Text = _appointmentToEdit.ReminderDate.ToShortDateString();
                lbViewAppointmentReminderTime.Text = _appointmentToEdit.ReminderTime.ToShortTimeString();
                lbViewAppointmentContacts.DataSource = new BindingSource
                                                           {DataSource = _appointmentToEdit.Company.Contacts.ToList()};
            }
        }

        private void BtClearSelectionClick(object sender, EventArgs e)
        {
            cbCompanies.SelectedItem = null;
        }

        private void CbSameAdressCheckedChanged(object sender, EventArgs e)
        {
            if (cbxSameAdress.Checked)
            {
                SetVisitAdressToPostalAdress();
            }
            else
            {
                AllowChangeToPostalAdress();
            }
        }

        private void AllowChangeToPostalAdress()
        {
            tbPostalCity.ReadOnly = false;
            tbPostalCountry.ReadOnly = false;
            tbPostalStreet.ReadOnly = false;
            tbPostalPostNumber.ReadOnly = false;
        }

        private void SetVisitAdressToPostalAdress()
        {
            tbPostalCity.Text = tbVisitCity.Text;
            tbPostalStreet.Text = tbVisitStreet.Text;
            tbPostalCountry.Text = tbVisitCountry.Text;
            tbPostalPostNumber.Text = tbVisitPostNumber.Text;
            tbPostalCity.ReadOnly = true;
            tbPostalCountry.ReadOnly = true;
            tbPostalStreet.ReadOnly = true;
            tbPostalPostNumber.ReadOnly = true;
        }

        private void TbCompanyNameTextChanged(object sender, EventArgs e)
        {
            btAddCompany.Enabled = tbCompanyName.TextLength > 0;
        }

        private void BtAddContactToCompanyClick(object sender, EventArgs e)
        {
            if (dgwCompanyContacts.SelectedRows.Count > 0)
            {
                var contactToAdd =
                    (Controller.AllContacts).Where(
                        contact => contact.Ssn == (string) dgwCompanyContacts.SelectedRows[0].Cells["Ssn"].Value).First();
                if (!((Customer) cbCompanies.SelectedItem).Contacts.Contains(contactToAdd))
                    ((Customer) cbCompanies.SelectedItem).Contacts.Add(contactToAdd);

                CbCompaniesSelectedIndexChanged(this, e);
            }
            Controller.SaveCustomers();
        }


        private void BtDelContactFromCompanyClick(object sender, EventArgs e)
        {
            ((Customer) cbCompanies.SelectedItem).Contacts.Remove((Contact) lbxContacts.SelectedItem);
            lbxContacts.DataSource = null;
            lbxContacts.DataSource = ((Customer) cbCompanies.SelectedItem).Contacts;
        }

        private void BtAddCompanyContactsDoneClick(object sender, EventArgs e)
        {
            tabCtrlCompanies.SelectedTab = tabAddCompany;
        }

        private void BtDelCompanyClick(object sender, EventArgs e)
        {
            var customerToRemove = ((Customer) cbCompanies.SelectedItem);

            if (!string.IsNullOrEmpty(customerToRemove.Logo))
                File.Delete(customerToRemove.Logo);
            Controller.RemoveCustomer(customerToRemove);
            cbCompanies.UpdateInfo();
        }

        private void CbNoteSelectCompanySelectedIndexChanged(object sender, EventArgs e)
        {
            cbNoteCompanyNote.DataSource = null;
            if (cbNoteSelectCompany.SelectedItem != null)
            {
                cbNoteCompanyNote.SelectedItem = null;
                cbNoteCompanyNote.DataSource = null;
                cbNoteCompanyNote.DataSource = ((Customer) cbNoteSelectCompany.SelectedItem).Notes;
                cbNoteCompanyNote.UpdateInfo();
                cbNoteCompanyNote.SelectedItem = null;
                cbNoteCompanyNote.Visible = true;
                lbNoteInfo.Visible = true;
                lbNoteSelectNote.Visible = true;
            }
            else
            {
                cbNoteCompanyNote.Visible = false;
                lbNoteInfo.Visible = false;
                lbNoteSelectNote.Visible = false;
            }
        }

        private void SaveCompanyNoteClick(object sender, EventArgs e)
        {
            Note noteToAdd;
            if (btSaveCompanyNote.Text == "Save")
            {
                noteToAdd = new Note();
            }
            else
            {
                noteToAdd = (Note) cbNoteCompanyNote.SelectedItem;
                ((Customer) cbNoteSelectCompany.SelectedItem).Notes.Remove(noteToAdd);
            }
            noteToAdd.Subject = tbNoteCompanySubject.Text;
            noteToAdd.NoteText = rtxNoteCompanyBody.Text;
            ((Customer) cbNoteSelectCompany.SelectedItem).Notes.Add(noteToAdd);
            cbNoteSelectCompany.UpdateInfo();

            Controller.SaveCustomers();
            Controller.AddNote(noteToAdd);
            Controller.SaveNotes();
        }

        private void CbNoteCompanyNoteSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbNoteCompanyNote.SelectedItem != null)
            {
                tbNoteCompanySubject.Text = ((Note) cbNoteCompanyNote.SelectedItem).Subject;
                rtxNoteCompanyBody.Text = ((Note) cbNoteCompanyNote.SelectedItem).NoteText;
                btSaveCompanyNote.Text = "Update";
                btNoteCompanyRemoveNote.Visible = true;
            }
            else
            {
                btSaveCompanyNote.Text = "Save";
                tbNoteCompanySubject.Text = string.Empty;
                rtxNoteCompanyBody.Text = string.Empty;
                btNoteCompanyRemoveNote.Visible = false;
            }
        }

        private void BtCompanyNoteClearClick(object sender, EventArgs e)
        {
            tbNoteCompanySubject.Text = String.Empty;
            cbNoteCompanyNote.SelectedItem = null;
            rtxNoteCompanyBody.Text = String.Empty;
        }

        private void BtNoteCompanyRemoveNoteClick(object sender, EventArgs e)
        {
            ((Customer) cbNoteSelectCompany.SelectedItem).Notes.Remove((Note) (cbNoteCompanyNote.SelectedItem));
            cbNoteSelectCompany.UpdateInfo();
            Controller.SaveCustomers();
        }

        private void BtContactToCompanyClick(object sender, EventArgs e)
        {
            tabCtrlCompanies.SelectedTab = tabViewCompanyContacts;
        }

        private void TabCtrlCompaniesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtrlCompanies.SelectedTab == tabViewCompanyNote)
            {
                cbNoteSelectCompany.DataSource = null;
                cbNoteSelectCompany.DataSource = Controller.AllCustomers();
                cbNoteSelectCompany.SelectedItem = null;
                cbNoteCompanyNote.SelectedItem = null;
            }
            if (tabCtrlCompanies.SelectedTab == tabViewCompanyContacts)
            {
                dgwCompanyContacts.ClearSelection();
                dgwCompanyContacts.DataSource = null;
                dgwCompanyContacts.DataSource = Controller.AllContacts.ToList();
                dgwCompanyContacts.Columns["Image"].Visible = false;
            }
        }

        private void TbSearchAppointmentsWithFilterClick(object sender, EventArgs e)
        {
            if (rbtSearchwordAppointment.Checked)
            {
                var searchQuery = tbSearchQuery.Text.ToLower();
                var chosenAppointment = from appointment in Controller.AllAppointments
                                        where appointment.Subject.ToLower().Contains(searchQuery)
                                        select appointment;

                dgvViewAppoinments.DataSource = chosenAppointment.ToList();
                HideColumnsInDataGridView();
            }

            else if (rbtSearchByAppointmentDate.Checked)
            {
                var searchQuery = mcAppointments.SelectionStart.Date;

                var chosenAppointment = from appointment in Controller.AllAppointments
                                        where appointment.DateTimeStart == searchQuery
                                        select appointment;

                dgvViewAppoinments.DataSource = chosenAppointment.ToList();
                HideColumnsInDataGridView();
            }

            else if (rtbSearchAppointmentsByContacts.Checked)
            {
                var searchQuery = tbSearchAppointmentsByContact.Text.ToLower();


                var selected = (from apointment in Controller.AllAppointments
                                from contact in apointment.Company.Contacts
                                where
                                    contact.FirstName.ToLower().Contains(searchQuery) ||
                                    contact.Surname.ToLower().Contains(searchQuery)
                                select apointment).ToList();

                dgvViewAppoinments.DataSource = selected.ToList();
            }

            else if (rbtSearchAppointmentsByCompany.Checked)
            {
                var searchQuery = cbCompanySearch.Text;

                var chosenAppointment = from appointment in Controller.AllAppointments
                                        where appointment.Company.Name == searchQuery
                                        select appointment;

                dgvViewAppoinments.DataSource = chosenAppointment.ToList();
            }
        }

        private void BtDelAppointmentClick(object sender, EventArgs e)
        {
            if (dgvViewAppoinments.SelectedRows.Count > 0)
            {
                Appointment appointmentToRemove =
                    Controller.AllAppointments.Where(
                        appointment =>
                        appointment.Subject == (string) dgvViewAppoinments.SelectedRows[0].Cells["Subject"].Value).First
                        ();

                Controller.AllAppointments.Remove(appointmentToRemove);

                FileManager.SaveFileAsXml();

                dgvViewAppoinments.DataSource = null;
                dgvViewAppoinments.DataSource = Controller.AllAppointments;

                HideColumnsInDataGridView();
            }
        }

        private void BtEditAppointmentClick(object sender, EventArgs e)
        {
            if (dgvViewAppoinments.SelectedRows.Count > 0)
            {
                GetSelectedAppointment();
                tabCtrlAppoinments.SelectedTab = tabAddAppointment;
            }
        }

        private void BtCompanyAddLogoClick(object sender, EventArgs e)
        {
            var customerToAddLogoTo = ((Customer) cbCompanies.SelectedItem);
            if (btCompanyAddLogo.Text == "Add logo")
            {
                _logo = AddLogo(tbCompanyName.Text);
                if (!string.IsNullOrEmpty(_logo))
                {
                    var ms = new MemoryStream(File.ReadAllBytes(_logo));
                    pbxCompanyLogo.Image = Image.FromStream(ms);
                    pbxCompanyLogo.Show();
                    ((Customer) cbCompanies.SelectedItem).Logo = _logo;
                    btCompanyAddLogo.Text = "Change";
                }
            }
            else
            {
                Controller.ChangeLogo(customerToAddLogoTo);
                SetLogo(customerToAddLogoTo);
            }
            _logo = String.Empty;
        }

        private void SetLogo(Customer customer)
        {
            customer.Logo = Controller.ChangeLogoName(customer.Logo, tbCompanyName.Text);
            var ms = new MemoryStream(File.ReadAllBytes(customer.Logo));
            pbxCompanyLogo.Image = Image.FromStream(ms);
            pbxCompanyLogo.Show();
        }


        private void McForDayCalendarDateSelected(object sender, DateRangeEventArgs e)
        {
            ClearCalendar();
            AddAppointmentToCalendar();
        }

        private void AddAppointmentToCalendar()
        {
            Color myColor = Color.FromArgb(255, 231, 66);

            lbChosenDayInCalendar.Text = mcForDayCalendar.SelectionStart.Date.ToShortDateString();

            DateTime searchQuery = mcForDayCalendar.SelectionStart.Date;

            var chosenAppointment = from appointment in Controller.AllAppointments
                                    where appointment.DateTimeStart == searchQuery
                                    select appointment;

            foreach (var appointment in chosenAppointment)
            {
                var startTime = appointment.TimeStart.Hour;
                var endTime = appointment.TimeEnd.Hour;

                for (var x = startTime; x < endTime; x++)
                {
                    dataGridView1.Rows[x].Cells[1].Value = appointment.Subject;
                    dataGridView1.Rows[x].DefaultCellStyle.BackColor = myColor;
                }
            }
        }

        private void CreateRows()
        {
            for (var i = 0; i < 25; i++)
            {
                dataGridView1.Rows.Add();
            }
        }

        private void SetTimeInCalendar()
        {
            for (var i = 0; i < 25; i++)
            {
                dataGridView1[0, i].Value = i;
            }
        }

        private void ClearCalendar()
        {
            for (var i = 0; i < 25; i++)
            {
                dataGridView1[1, i].Value = "";
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void BtJournalClick1(object sender, EventArgs e)
        {
            tabCtrlJournal.Show();
            tabCtrlAppoinments.Hide();
            tabCtrlCompanies.Hide();
            tabCtrlContact.Hide();
            pbHome.Hide();
            tabCtrlUserManual.Hide();
            lbxBirthDays.DataSource = new BindingSource
                                          {
                                              DataSource =
                                                  Controller.BirthdayGreetings.Where(
                                                      reminder =>
                                                      reminder.DateTimeStart.Date.DayOfYear ==
                                                      DateTime.Now.Date.DayOfYear).ToList()
                                          };
            lbxAllNotes.DataSource = null;
            lbxAllNotes.DataSource = new BindingSource {DataSource = Controller.AllNotesEver().ToList()};
            lbCompanyJournal.SelectedItem = null;
            lbContactsJournal.DataSource = new BindingSource {DataSource = Controller.AllContacts};
            lbContactsJournal.SelectedItem = null;
        }

        private void LbCompanyJournalSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbCompanyJournal.SelectedItem != null)
            {
                lbxJournalCompanyNotes.DataSource = null;
                lbxJournalCompanyNotes.DataSource = ((Customer) lbCompanyJournal.SelectedItem).Notes.ToList();
            }
        }

        private void LbxJournalCompanyNotesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxJournalCompanyNotes.SelectedItem != null)
            {
                rtxbCompanyJournalNoteDate.Text =
                    ((Note) lbxJournalCompanyNotes.SelectedItem).DateAdded.ToShortDateString();
                rtxbCompanyJournalNoteText.Text = ((Note) lbxJournalCompanyNotes.SelectedItem).NoteText;
            }
        }

        private void LbxJournalContactNotesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxJournalContactNotes.SelectedItem != null)
            {
                rtxbContactJournalNoteDate.Text =
                    ((Note) lbxJournalContactNotes.SelectedItem).DateAdded.ToShortDateString();
                rtbxContactNote.Text = ((Note) lbxJournalContactNotes.SelectedItem).NoteText;
            }
        }

        private void BtViewDetailsOfAppointmentClick(object sender, EventArgs e)
        {
            if (dgvViewAppoinments.SelectedRows.Count > 0)
            {
                GetAppointmentDetails();
                tabCtrlAppoinments.SelectedTab = tabAppointmentDetails;
            }
        }

        private bool CheckDateTimeCollision(DateTime start, DateTime end, DateTime date)
        {
            return
                Controller.AllAppointments.Where(appointment => date == appointment.DateTimeStart.Date).All(
                    appointment =>
                    (start.Hour < appointment.TimeStart.Hour && end.Hour < appointment.TimeStart.Hour) &&
                    (start.Hour <= appointment.TimeEnd.Hour));
        }


        private void BtBookAppointmentClick(object sender, EventArgs e)

        {
            errorProvider1.Clear();
            var allRowsEmpty = true;

            if (dataGridView1.SelectedRows.Count <= 0) return;
            for (var rowNumber = 0; rowNumber < dataGridView1.SelectedRows.Count; rowNumber++)
                if (!(string.IsNullOrEmpty((string) dataGridView1.SelectedRows[rowNumber].Cells["Activity"].Value)))
                    allRowsEmpty = false;

            if (allRowsEmpty)
            {
                var firstValueInSelection = Int32.Parse(dataGridView1.SelectedRows[0].Cells["Time"].Value.ToString());
                var lastValueInSelection =
                    Int32.Parse(
                        dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].Cells["Time"].Value.
                            ToString());

                if (firstValueInSelection > lastValueInSelection)
                {
                    var temp = firstValueInSelection;
                    firstValueInSelection = lastValueInSelection;
                    lastValueInSelection = temp;
                }

                BtAppointmentsClick(null, null);
                dtpAppointmentDateStart.Value = mcForDayCalendar.SelectionStart;

                dtpAppointmentTimeStart.Value = new DateTime(dtpAppointmentDateStart.Value.Date.Year,
                                                             dtpAppointmentDateStart.Value.Date.Month,
                                                             dtpAppointmentDateStart.Value.Date.Day,
                                                             firstValueInSelection, 0, 0);
                dtpAppointmentTimeEnd.Value = new DateTime(dtpAppointmentDateStart.Value.Date.Year,
                                                           dtpAppointmentDateStart.Value.Date.Month,
                                                           dtpAppointmentDateStart.Value.Date.Day,
                                                           lastValueInSelection + 1, 0, 0);
                tabCtrlAppoinments.SelectedTab = tabAddAppointment;
            }
            else
            {
                errorProvider1.SetError(btBookAppointment, "Not available date or time, check calendar");
            }
        }

        private void BtUserManualClick(object sender, EventArgs e)
        {
            tabCtrlUserManual.Show();
            tabCtrlJournal.Hide();
            tabCtrlAppoinments.Hide();
            tabCtrlCompanies.Hide();
            tabCtrlContact.Hide();
            pbHome.Hide();
            webUserManual.Navigate((Directory.GetCurrentDirectory()) + @"\UserManual.pdf");

        }

        private void BtAddIntrestestClick(object sender, EventArgs e)
        {
            ((Contact) cbContactToAddDetail.SelectedItem).Inrests.Clear();

            foreach (var intrests in clbInterests.CheckedItems)
            {
                var interest = (Contact.Interest) Enum.Parse(typeof (Contact.Interest), intrests.ToString());

                ((Contact) cbContactToAddDetail.SelectedItem).Inrests.Add(interest);
            }
        }

        private void CbContactNoteContactSelectedIndexChanged(object sender, EventArgs e)
        {
            cbContactNoteRelative.DataSource = null;
            if (cbContactNoteContact.SelectedItem != null)
            {
                cbContactNoteRelative.DataSource = ((Contact) cbContactNoteContact.SelectedItem).Relatives;
                cbContactNoteRelative.SelectedItem = null;
                cbContactNoteNote.DataSource = ((Contact) cbContactNoteContact.SelectedItem).Notes;
                cbContactNoteNote.SelectedItem = null;
                cbContactNoteRelative.Visible = true;
                lbSelectRelative.Visible = true;
                lbRelativeInfo.Visible = true;
                lbContactNoteSelectNote.Visible = true;
                cbNoteCompanyNote.Visible = true;
                cbContactNoteNote.Visible = true;
                lbAddContactNoteInfo.Visible = true;
            }

            else
            {
                cbContactNoteRelative.Visible = false;
                lbSelectRelative.Visible = false;
                lbRelativeInfo.Visible = false;
                lbContactNoteSelectNote.Visible = false;
                cbNoteCompanyNote.Visible = false;
                cbContactNoteNote.Visible = false;
                lbAddContactNoteInfo.Visible = false;
            }
        }


        private void BtContactNoteSaveClick(object sender, EventArgs e)
        {
            Person personToEdit;
            if (cbContactNoteRelative.SelectedItem != null)
                personToEdit = ((Relative) cbContactNoteRelative.SelectedItem);
            else
            {
                personToEdit = ((Person) cbContactNoteContact.SelectedItem);
            }
            Note noteToAdd;
            if (btContactNoteSave.Text == "Save")
            {
                noteToAdd = new Note();
            }
            else
            {
                noteToAdd = (Note) cbContactNoteNote.SelectedItem;
                personToEdit.Notes.Remove(noteToAdd);
            }

            noteToAdd.Subject = tbContactNoteSubject.Text;
            noteToAdd.NoteText = rtbContactNoteBody.Text;


            personToEdit.Notes.Add(noteToAdd);

            cbContactNoteContact.UpdateInfo();
            cbContactNoteNote.UpdateInfo();
            tbContactNoteSubject.Text = string.Empty;
            rtbContactNoteBody.Text = string.Empty;

            FileManager.SaveContacts();
        }

        private void CbContactNoteNoteSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbContactNoteNote.SelectedItem != null)
            {
                tbContactNoteSubject.Text = ((Note) cbContactNoteNote.SelectedItem).Subject;
                rtbContactNoteBody.Text = ((Note) cbContactNoteNote.SelectedItem).NoteText;
                btContactNoteSave.Text = "Update";
                btContactNoteDel.Visible = true;
            }
            else
            {
                btContactNoteSave.Text = "Save";
                tbContactNoteSubject.Text = string.Empty;
                rtbContactNoteBody.Text = string.Empty;
                btContactNoteDel.Visible = false;
            }
        }

        private void CbContactNoteRelativeSelectedIndexChanged(object sender, EventArgs e)
        {
            cbContactNoteNote.DataSource = null;
            if (cbContactNoteRelative.SelectedItem != null)
            {
                cbContactNoteNote.DataSource = ((Relative) cbContactNoteRelative.SelectedItem).Notes;
                cbContactNoteNote.SelectedItem = null;
            }
        }

        private void BtContactNoteClearClick(object sender, EventArgs e)
        {
            tbContactNoteSubject.Text = String.Empty;
            cbContactNoteNote.SelectedItem = null;
            cbContactNoteRelative.SelectedItem = null;
            rtbContactNoteBody.Text = String.Empty;
        }

        private void BtContactNoteDelClick(object sender, EventArgs e)
        {
            Person personToEdit;
            if (cbContactNoteRelative.SelectedItem != null)
                personToEdit = ((Relative) cbContactNoteRelative.SelectedItem);
            else
            {
                personToEdit = ((Person) cbContactNoteContact.SelectedItem);
            }

            personToEdit.Notes.Remove((Note) (cbContactNoteNote.SelectedItem));
            cbContactNoteContact.UpdateInfo();
            Controller.SaveNotes();
            FileManager.SaveContacts();
        }

        private void TabCtrlJournalSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtrlJournal.SelectedTab == tabSearchAllNotes)
            {
                lbxAllNotes.DataSource = null;
                lbxAllNotes.DataSource = new BindingSource {DataSource = Controller.AllNotesEver().ToList()};
            }
            if (tabCtrlJournal.SelectedTab == tabJournalCompany)
            {
                lbCompanyJournal.DataSource = new BindingSource {DataSource = Controller.AllCustomers()};
            }
            if (tabCtrlJournal.SelectedTab == tabContactJournal)
                lbContactsJournal.DataSource = new BindingSource {DataSource = Controller.AllContacts};
        }

        private void LbxAllNotesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxAllNotes.SelectedItem != null)
            {
                tbAllNotesDate.Text = ((Note) lbxAllNotes.SelectedItem).DateAdded.ToShortDateString();
                rtxbAllNotesText.Text = ((Note) lbxAllNotes.SelectedItem).NoteText;
            }
            else
            {
                tbAllNotesDate.Text = string.Empty;
                rtxbAllNotesText.Text = string.Empty;
            }
        }

        private void BtSearchNotesClick(object sender, EventArgs e)
        {
            var searchQuery = txbSearchAllNotes.Text.ToLower();

            var foundNotes = new BindingSource
                                 {
                                     DataSource =
                                         (from note in Controller.AllNotesEver()
                                          where
                                              note.NoteText.ToLower().Contains(searchQuery) ||
                                              note.Subject.ToLower().Contains(searchQuery)
                                          select note).ToList()
                                 };

            lbxAllNotes.DataSource = null;
            lbxAllNotes.Items.Clear();
            lbxAllNotes.DataSource = foundNotes;
        }

        private void McInSearchNotesDateChanged(object sender, DateRangeEventArgs e)
        {
            var searchQuery = mcInSearchNotes.SelectionStart.Date;
            var noteByDate = new BindingSource
                                 {
                                     DataSource = (from note in Controller.AllNotesEver()
                                                   where note.DateAdded.Date == searchQuery.Date
                                                   select note).ToList()
                                 };

            lbxAllNotes.DataSource = null;
            lbxAllNotes.Items.Clear();
            lbxAllNotes.DataSource = noteByDate;
        }

        private void LbContactsJournalSelectedIndexChanged1(object sender, EventArgs e)
        {
            if (lbContactsJournal.SelectedItem != null)
            {
                lbxJournalContactNotes.DataSource = new BindingSource
                                                        {
                                                            DataSource =
                                                                ((Contact) lbContactsJournal.SelectedItem).Notes.ToList()
                                                        };
            }
        }

        private void McForDayCalendarDateChanged(object sender, DateRangeEventArgs e)
        {
            lbxBirthDays.DataSource = null;
            lbxBirthDays.DataSource = new BindingSource
                                          {
                                              DataSource =
                                                  Controller.BirthdayGreetings.Where(
                                                      reminder =>
                                                      reminder.DateTimeStart.Month ==
                                                      mcForDayCalendar.SelectionStart.Month &&
                                                      reminder.DateTimeStart.Day == mcForDayCalendar.SelectionStart.Day
                                                  ).ToList()
                                          };
        }

        private void BtAppointmentsClick(object sender, EventArgs e)
        {
            tabCtrlAppoinments.Show();
            tabCtrlJournal.Hide();
            tabCtrlContact.Hide();
            tabCtrlCompanies.Hide();
            pbHome.Hide();
            cbCompany.UpdateInfo();
            tabCtrlUserManual.Hide();
        }
    }
}