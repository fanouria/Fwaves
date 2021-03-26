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
using WFGUI;

namespace WPFPageSwitch
{

	public partial class SearchPersonPage : UserControl, ISwitchable
	{
        public List<MeasurementID> measurementID = new List<MeasurementID>();
        List<Patient> person= new List<Patient>();

        public SearchPersonPage()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
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

        private void Back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Switcher.Switch(new MainMenu());
        }

        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataAccess db = new DataAccess();
            if (AMKAtext.Text == "")
            {
                MessageBox.Show("Please insert AMKA number.", "Warning");
                AMKAtext.Text = "";
            }
            else
            {
                bool successfullyParsed = long.TryParse(AMKAtext.Text, out long AMKA);
                if (successfullyParsed)
                {
                    person = db.GetPerson(AMKA);
                    if (person.Count == 0)
                    {
                        MessageBox.Show("Person not found", "Warning");
                        AMKAtext.Text = "";
                    }
                    else
                    {                    
                        measurementID = db.GetMeasurementIDs(person[0].AMKA);
                        if (measurementID.Count != 0)
                        {
                            WFGUI.Index.DateIndex = 0;
                            WFGUI.Index.AMKADateIndex = person[0].AMKA;
                            Switcher.Switch(new PatientProfileChartPage(person, WFGUI.Index.DateIndex));
                        }
                        else
                        {
                            Switcher.Switch(new PatientProfilePage(person));
                        }                   
                    }
                    
                }
                else
                {
                    MessageBox.Show("Please insert a number.", "Warning");
                    AMKAtext.Text = "";
                }
                
            }            
        }
        #endregion
    }
}