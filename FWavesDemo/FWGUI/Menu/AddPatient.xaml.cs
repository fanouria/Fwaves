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
    public partial class AddPatient: UserControl, ISwitchable
    {
        List<Patient> person = new List<Patient>();
        public AddPatient()
        {
            InitializeComponent();
            DiagnosisComboBox.ItemsSource = LoadDiagnosisComboBoxData();

        }
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //System.Windows.Controls.UserControl usc = null;


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
            Switcher.Switch(new MainMenu());
        }



        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataAccess db = new DataAccess();

            int sex = 0;            

            if (AMKAtxt.Text == "" || heighttxt.Text == "" )
            {
                MessageBox.Show("Please fill the form.", "Warning");

            }
            else if (!(armLengthtxt.Text != "" || legLengthtxt.Text != ""))
            {
                MessageBox.Show("Please fill in the arm or leg length", "Warning");
                AMKAtxt.Text = "";
            }
            else if (AMKAtxt.Text.Length != 11)
            {
                MessageBox.Show("AMKA should consist of 11 digits", "Warning");
                AMKAtxt.Text = "";
            }            
            else if (Malebutton.IsChecked != true && Femalebutton.IsChecked != true)
            {
                MessageBox.Show("Please check one of the Buttons", "Warning");
            }           
            else
            {
                bool successfullyParsedAMKA = long.TryParse(AMKAtxt.Text, out long AMKA);
                bool successfullyParsedHeight = float.TryParse(heighttxt.Text, out float height);
                bool successfullyParsedArmLength = float.TryParse(armLengthtxt.Text, out float ArmLength);
                bool successfullyParsedLegLength = float.TryParse(legLengthtxt.Text, out float LegLength);

                if (successfullyParsedAMKA && successfullyParsedHeight && successfullyParsedArmLength  && successfullyParsedLegLength)
                {
                    if (Malebutton.IsChecked == true)
                    {
                        sex = 1;
                    }
                    else if (Femalebutton.IsChecked == true)
                    {
                        sex = 2;
                    }


                    if (legLengthtxt.Text=="")
                    {
                        legLengthtxt.Text = "0";
                    }

                    if (armLengthtxt.Text == "")
                    {
                        armLengthtxt.Text = "0";
                    }

                    //if user has inserted decimal point as "," replace that with "."

                    legLengthtxt.Text = legLengthtxt.Text.Replace(",", ".");
                    armLengthtxt.Text = armLengthtxt.Text.Replace(",", ".");

                    int selectedIndex = -1;
                    selectedIndex = DiagnosisComboBox.SelectedIndex + 1;
                    if (selectedIndex == -1)
                    {

                        db.InsertPerson(long.Parse(AMKAtxt.Text), sex, float.Parse(heighttxt.Text), float.Parse(armLengthtxt.Text), float.Parse(legLengthtxt.Text), commenttxt.Text);
                    }
                    else
                    {
                        db.InsertPerson(long.Parse(AMKAtxt.Text),long.Parse(Yeartxt.Text), sex, float.Parse(heighttxt.Text), float.Parse(armLengthtxt.Text), float.Parse(legLengthtxt.Text), selectedIndex, commenttxt.Text);
                    }

                    person = db.GetPerson(long.Parse(AMKAtxt.Text));
                    if (person.Count == 0)
                    {
                        MessageBox.Show("Patient has not been added", "Warning");
                        AMKAtxt.Text = "";
                    }
                    else
                    {
                        Switcher.Switch(new PatientProfilePage(person));
                    }
                }
                else
                {
                    if (!successfullyParsedAMKA)
                    {
                        MessageBox.Show("AMKA should be a number", "Warning");
                        AMKAtxt.Text = "";
                    }
                    else if (!successfullyParsedHeight)
                    {
                        MessageBox.Show("Height should be a number", "Warning");
                        heighttxt.Text = "";
                    }
                    else if (!successfullyParsedArmLength)
                    {
                        MessageBox.Show("Arm Length should be a number", "Warning");
                        armLengthtxt.Text = "0";
                    }
                    else if (!successfullyParsedLegLength)
                    {
                        MessageBox.Show("Leg Length should be a number", "Warning");
                        legLengthtxt.Text = "0";
                    }
                }
            }





            //UpdateBinding();

        }
    }
}