using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFUI;

namespace WPFPageSwitch
{
	public partial class MainMenu : System.Windows.Controls.UserControl, ISwitchable
	{
 		public MainMenu()
		{
            
			InitializeComponent();

            DataAccess db = new DataAccess();
            int totalPersons = db.GetTotal();
            totalpersons.Text = totalPersons.ToString();
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
        private void NewGameButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new AddPatient());
		}

		private void LoadGameButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new SearchPersonPage());
		}

		private void OptionButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new DatabaseStatistics());
		}

        //private void ShowMessageBox(string title, string message, MessageBoxIcon icon)
        //{
            //MessageBoxChildWindow messageWindow =
            //    new MessageBoxChildWindow(title, message, MessageBoxButtons.Ok, icon);

            //messageWindow.Show();
        //}

        #region Event For Child Window
        private void LoginWindowForm_SubmitClicked(object sender, EventArgs e)
        {
            //ShowMessageBox("Login Successful", "Welcome, " + loginForm.NameText, MessageBoxIcon.Information);

        }

        private void RegisterForm_SubmitClicked(object sender, EventArgs e)
        {
        }


        #endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void LoginTextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	//Switcher.Switch(new Login());
        }

        private void RegisterTextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	//Switcher.Switch(new Register());
        }
        #endregion


    }
}