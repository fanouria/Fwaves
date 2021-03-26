using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFUI;

namespace WPFPageSwitch
{
    public partial class EditPatientPage: UserControl, ISwitchable
    {
        List<Patient> person = new List<Patient>();
        public EditPatientPage()
        {
            InitializeComponent();
            DiagnosisComboBox.ItemsSource = LoadDiagnosisComboBoxData();

        }
        public EditPatientPage(List<Patient> search_person) : this()
        {
            // Required to initialize variables
            person.Add(search_person[0]);
            AMKAtxt.Text = $"{person[0].AMKA}";

        }
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((System.Windows.Controls.ListViewItem)((System.Windows.Controls.ListView)sender).SelectedItem).Name)
            {
                case "Search":
                    Switcher.Switch(new SearchPersonPage());
                    break;
                case "Add":
                    Switcher.Switch(new AddPatient());
                    break;
                case "Database":
                    Switcher.Switch(new DatabaseStatistics());
                    break;
                case "Patients":
                    Switcher.Switch(new PatientsPage());
                    break;
                case "Info":
                    Switcher.Switch(new Information());
                    break;
                default:
                    break;
            }
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private string[] LoadDiagnosisComboBoxData()
        {
            string[] diagnosisArray = {
                "ALS Amyotrophic Lateral Sclerosis",
                "MN Mononeuropathy",
                "MN-CTS Carpal Tunnel Syndrome",
                "PN-D Polyneuropathy Demyalinative",
                "PN-A Hereditary Polyneuropathy",
                "PN-H Hereditary Polyneuropathy",
                "R Radiculopathy",
                "MY Myopathy",
                "N Normal",
                "UMN Pyramidal",
                "O Other",
                "MG Myasthenia",
                "ANS",
                "PLX Plexus",
            };
            return diagnosisArray;
        }


        private void Back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new PatientProfilePage(person));
        }



        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataAccess db = new DataAccess();          


            int selectedIndex = DiagnosisComboBox.SelectedIndex + 1;
            db.EditPerson(person[0].AMKA, selectedIndex, commenttxt.Text);
            person = db.GetPerson(person[0].AMKA);


            Switcher.Switch(new PatientProfilePage(person));
                
        }
    }
}