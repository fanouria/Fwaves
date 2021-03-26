using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WFGUI;
using WPFUI;

namespace WPFPageSwitch
{
    
    public partial class AddExcel : UserControl
    {
        List<Patient> person = new List<Patient>();
        public string fileName;
        List<MeasurementID> measurementID = new List<MeasurementID>();

        public AddExcel(List<Patient> search_person)
        {
            person.Add(search_person[0]);
            InitializeComponent();            
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

        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int neuron = 0, side = 0; 
            DataAccess db = new DataAccess();
            DateTime date = new DateTime();
            date = DateTime.Today;
            string todayDate = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                  
            if (Ulnarbutton.IsChecked != true && Medianbutton.IsChecked != true && Peronealbutton.IsChecked != true && Tibialbutton.IsChecked != true)
            {
                MessageBox.Show("Please check one of the Side Buttons", "Warning");
            }
            else
            {
                if (Ulnarbutton.IsChecked == true)
                {
                    neuron = 1;
                }
                else if (Medianbutton.IsChecked == true)
                {
                    neuron = 2;
                }
                else if (Peronealbutton.IsChecked == true)
                {
                    neuron = 3;
                }
                else if (Tibialbutton.IsChecked == true)
                {
                    neuron = 4;
                }
            }
            if (Rightbutton.IsChecked != true && Leftbutton.IsChecked != true)
            {
                MessageBox.Show("Please check one of the Side Buttons", "Warning");
            }
            else
            {
                if (Rightbutton.IsChecked == true)
                {
                    side = 1;
                }
                else if (Leftbutton.IsChecked == true)
                {
                    side = 2;
                }
            }

            //if user has inserted decimal point as "," replace that with "."

            mlattxt.Text = mlattxt.Text.Replace(",", ".");
            mareatxt.Text = mareatxt.Text.Replace(",", ".");
            mamptxt.Text = mamptxt.Text.Replace(",", ".");
            fpertxt.Text = fpertxt.Text.Replace(",", ".");
            freppertxt.Text = freppertxt.Text.Replace(",", ".");

            //check if user has inserted correct type of data
            bool successfullyParsedmlat       = float.TryParse(mlattxt.Text, out float mlat);
            bool successfullyParsedmarea      = float.TryParse(mareatxt.Text, out float marea );
            bool successfullyParsedmamp       = float.TryParse(mamptxt.Text, out float mamp );
            bool successfullyParsedfper       = float.TryParse(fpertxt.Text, out float fper);
            bool successfullyParsedfrepper    = float.TryParse(freppertxt.Text, out float frepper);
            bool successfullyParsedrns        = int.TryParse(rnstxt.Text, out int rns);
            bool successfullyParsedrn5rep     = int.TryParse(rn5reptxt.Text, out int rn5rep);
            bool successfullyParsedfrepstotal = int.TryParse(frepstotaltxt.Text, out int frepstotal);

            
            if (successfullyParsedmlat && successfullyParsedmarea && successfullyParsedmamp  && successfullyParsedfper && successfullyParsedfrepper && successfullyParsedrns && successfullyParsedrn5rep && successfullyParsedfrepstotal)
            {
                db.InsertExcel(person[0].AMKA,person[0].ArmLength, todayDate, FileNameTextBox.Text, neuron, side, float.Parse(mlattxt.Text), float.Parse(mareatxt.Text), float.Parse(mamptxt.Text), 
                float.Parse(fpertxt.Text) , float.Parse(freppertxt.Text), int.Parse(rnstxt.Text), int.Parse(rn5reptxt.Text), int.Parse(frepstotaltxt.Text)); // bad manners - too many args
                measurementID = db.GetMeasurementIDs(person[0].AMKA);            

                if (measurementID.Count != 0)
                {
                    WFGUI.Index.DateIndex = measurementID.Count - 1;
                    WFGUI.Index.AMKADateIndex = person[0].AMKA; // You need to update the person Viewmodel refers to. It is not passed to ViewModel through the new PatientProfileChartPage but through the WFGUI.Index
                    Switcher.Switch(new PatientProfileChartPage(person, WFGUI.Index.DateIndex));
                }
                else
                {
                    Switcher.Switch(new PatientProfilePage(person));
                }
            }
            else
            {
                MessageBox.Show("Data should be numbers.", "Warning");
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new PatientProfilePage(person));
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Excel Files (*.xlsx)|*.xlsx|Excel Files (*.xls)|*.xls";
            openFileDlg.Title = "Select Attendance Record";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;                                 

            }
        }
    }
}