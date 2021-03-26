
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WPFUI;
using WFGUI;
using Extreme.DataAnalysis;
using Extreme.Mathematics;
using Extreme.Statistics;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using Image = iTextSharp.text.Image;



namespace WPFPageSwitch
{
    public partial class PatientProfileChartPage : System.Windows.Controls.UserControl, ISwitchable
    {
        List<Patient> person = new List<Patient>();
        public List<MeasurementID> measurementID = new List<MeasurementID>();
        List<Measurement> measurement = new List<Measurement>();
        List<VelocitiesViewModel> velocitiesViewModels = new List<VelocitiesViewModel>();
        List<PlotModel> model = new List<PlotModel>();
        int dateIndex = 0; 
     
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

        public PatientProfileChartPage()
        {
            // Required to initialize variables
            InitializeComponent();      
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Switcher.Switch(new Login());
        }
        #endregion

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


        public PatientProfileChartPage(List<Patient> search_person, int index) : this()
        {
            // Required to initialize variables
            person.Add(search_person[0]);
            dateIndex = index;
            AMKAtxt.Text = $"{person[0].AMKA}";
            if (person[0].Sex == 1)
            {
                sextxt.Text = "Male";
            }
            else if (person[0].Sex == 2)
            {
                sextxt.Text = "Female";
            }

            heighttxt.Text = $"{ person[0].Height} cm";
            armLengthtxt.Text = $"{ person[0].ArmLength} cm";
            legLengthtxt.Text = $"{ person[0].LegLength} cm";

            // if Diagnosis not NULL, add Diagnosis to profile 
            if (person[0].Diagnosis != 0)
            {
                diagnosistxt.Text = $"{ diagnosisArray[person[0].Diagnosis - 1]}";
            }
            else
            {
                diagnosistxt.Text = $" - ";
            }
            //PatientInfotxt.Text = $"AMKA: {person[0].AMKA}        Sex: Female        Height: {person[0].Height}cm        Arm Length: {person[0].ArmLength}cm        Leg Length: {person[0].LegLength}cm"; //+Environment.NewLine + $"Comment: {person[0].Comment}"
            PatientCommenttxt.Text = $"{person[0].Comment}";
            DataAccess db = new DataAccess(); 

            measurementID = db.GetMeasurementIDs(person[0].AMKA);
            if (measurementID.Count != 0)
            {
                measurement = db.GetMeasurements(measurementID[dateIndex].PK); //measurement from a date is stored in that list
                TestDatesComboBox.ItemsSource = LoadTestDatesComboBoxData();
            }
            TestDatesComboBox.SelectedIndex = index;

            int diagnosis = db.Prediction(person[0].AMKA);
            MlDiagnosistxt.Text = $"{diagnosisArray[diagnosis-1]}";
        }

        private string[] LoadTestDatesComboBoxData()
        {
            string[] testDatesArray = new string[measurementID.Count];
            for (int i = 0; i < measurementID.Count; i++)
            {
                testDatesArray[i] = measurementID[i].Date;

            }
            return testDatesArray;
        }

        private void StudyDateLoaded_Click(object sender, RoutedEventArgs e)
        {
            WFGUI.Index.DateIndex = TestDatesComboBox.SelectedIndex;
            Switcher.Switch(new PatientProfileChartPage(person, WFGUI.Index.DateIndex));
        }
        
        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void AddExcel_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddExcel(person));
        }

        private void MakeModels(long PK)
        {
            DataAccess db = new DataAccess();
            measurement = db.GetMeasurements(PK);

            /***** Create the Histogram Bins *****/

            var fcvHistogram = Histogram.CreateEmpty(0, 80, 80);
            double count = 0;
            float xmin = 10000;
            float xmax = 0;
            foreach (var x in measurement)
            {
                if (x.FCV < xmin)
                {
                    xmin = x.FCV;
                }
                if (x.FCV > xmax)
                {
                    xmax = x.FCV;
                }
                if (x.FCV != 0)
                {
                    fcvHistogram.Increment((int)x.FCV); //(int)1.5 = 1. I want to inceremen
                    count++;// add individual data points
                }
            }

            /***** Create the visual part of the Histogram ******/

            //this.Title = "Conduction Velocities";

            OxyPlot.Axes.LinearAxis xaxis = new OxyPlot.Axes.LinearAxis();
            
            xaxis.Title = "FCV(m/s)";
            xaxis.Minimum = (int)xmin - 5;
            xaxis.Maximum = (int)xmax + 5;
            xaxis.TitleFontWeight = 700;
            xaxis.Position = AxisPosition.Bottom;
            xaxis.MajorGridlineStyle = LineStyle.Solid;
            xaxis.MinorGridlineStyle = LineStyle.Solid;

            OxyPlot.Axes.LinearAxis yaxis = new OxyPlot.Axes.LinearAxis();
            yaxis.Title = "%";
            yaxis.TitleFontWeight = 700;
            yaxis.Position = AxisPosition.Left;
            yaxis.MajorGridlineStyle = LineStyle.Solid;
            yaxis.MinorGridlineStyle = LineStyle.Solid;

            OxyPlot.Series.RectangleBarSeries bars = new OxyPlot.Series.RectangleBarSeries();
            //s1.IsStacked = true;
            bars.StrokeColor = OxyColors.Black;
            bars.FillColor = OxyColors.DarkOliveGreen;
            bars.Background = OxyColors.GhostWhite;
            count = 100.0 / count;


            double x0 = 0, x1 = 1;
            foreach (var x in fcvHistogram.BinsAndValues)
            {
                //s1.Items.Add(new ColumnItem(x.Value*count));
                bars.Items.Add(new RectangleBarItem(x0, 0.0, x1, x.Value * count));
                //xaxis.Labels.Add(x.Key.ToString().Substring(0, 3));
                x0++;
                x1++;
            }

            var Model = new PlotModel();
            Model.Title = "Conduction Velocities";
            Model.Background = OxyColors.GhostWhite;
            Model.Axes.Add(xaxis);
            Model.Axes.Add(yaxis);
            Model.Series.Add(bars);
            model.Add(Model);
        }

        private void SavePDF_Click(object sender, RoutedEventArgs e)
        {
            DataAccess db = new DataAccess();

            int count = measurementID.Count;
            for (int i = 0; i < count; i++)
            {
                MakeModels(measurementID[i].PK);//measurement from a date is stored in that list
            }


            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF file|*.pdf", ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    /* Create PDF Document */
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    try
                    {
                        Image png;

                        PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        doc.Open();

                        doc.Add(new iTextSharp.text.Phrase("    "));
                        doc.Add(new iTextSharp.text.Phrase($" AMKA:{ AMKAtxt.Text }   Sex:{ sextxt.Text }   "));
                        doc.Add(new Paragraph("Histograms"));
                        doc.Add(new iTextSharp.text.Phrase($"Study Date: "));

                        for (int i = 0; i < count; i++)
                        {
                            OxyPlot.Wpf.PngExporter.Export(model[i], "test.png", 1500, 800, OxyColors.White, 200); // export plot to png
                            png = Image.GetInstance("test.png");
                            png.ScalePercent(20f); //scale the image
                            png.Alignment = Image.ALIGN_CENTER;
                            doc.Add(png); //add image to pdf
                        }

                        doc.Close();
                    }
                    catch (Exception ex)
                    {

                        System.Windows.MessageBox.Show(ex.Message, "Message");
                    }
                    finally
                    {
                        doc.Close();
                    }

                }
            }
        }

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new EditPatientPage(person));
        }

        private void DeleteP_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this patient?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DataAccess db = new DataAccess();
                int success = db.DeletePerson(person[0].AMKA);
                if (success == 1)
                {
                    Switcher.Switch(new MainMenu());
                }
                
            }

        }

        private void DeleteExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this study?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DataAccess db = new DataAccess();
                int success = db.DeleteMeasurement(person[0].AMKA, measurementID[WFGUI.Index.DateIndex].PK);
                if (success == 1)
                {
                    Switcher.Switch(new PatientProfilePage(person));
                }

            }
        }
    }
}