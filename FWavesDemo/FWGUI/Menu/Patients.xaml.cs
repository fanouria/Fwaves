using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
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
	public partial class PatientsPage : UserControl, ISwitchable
	{
        List<Patient> person = new List<Patient>();
        public PatientsPage()
		{
			// Required to initialize variables
			InitializeComponent();

            DataAccess db = new DataAccess();
            int totalPersons = db.GetTotal();
            totalPersonsTxt.Text = totalPersons.ToString();
            person = db.GetPatients();
            PatientsGrid.ItemsSource = person;
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



        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void Export_Meas_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Information());
        }
	}
}