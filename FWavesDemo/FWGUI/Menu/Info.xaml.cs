using LiveCharts;
using LiveCharts.Wpf;
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
using WFGUI;

namespace WPFPageSwitch
{
	public partial class Information : UserControl, ISwitchable
	{
		public Information()
		{
			// Required to initialize variables
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

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

            //using (fwavesEntities db = new fwavesEntities())
            //{
            //    var data = db.GetTotalHeight();
            //    LiveCharts.Wpf.ColumnSeries col = new LiveCharts.Wpf.ColumnSeries() { DataLabels = true, Values = new ChartValues<int>(), LabelPoint = point => point.Y.ToString() };
            //    Axis ax = new Axis() { Separator = new LiveCharts.Wpf.Separator() { Step = 1 } };
            //    ax.Labels = new List<string>();
            //    foreach (var x in data)
            //    {
            //        col.Values.Add(x.Total.Value);
            //        ax.Labels.Add(x.Height.ToString());
            //    }
            //    cartesianChartHeight.Series.Add(col);
            //    cartesianChartHeight.AxisX.Add(ax);
            //    cartesianChartHeight.AxisY.Add(new Axis
            //    {
            //        LabelFormatter = value => value.ToString(),
            //        Separator = new LiveCharts.Wpf.Separator()
            //    });
            //}

        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        #endregion
	}
}