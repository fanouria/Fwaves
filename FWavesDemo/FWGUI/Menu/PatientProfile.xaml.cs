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
    public partial class PatientProfilePage : System.Windows.Controls.UserControl, ISwitchable
    {
        List<Patient> person = new List<Patient>();
        List<MeasurementID> measurementID = new List<MeasurementID>();
        List<Measurement> measurement = new List<Measurement>();
        List<PlotModel> model = new List<PlotModel>();

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

        public PatientProfilePage()
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

        public PatientProfilePage(List<Patient> search_person) : this()
        {
            // Required to initialize variables
            person.Add(search_person[0]);
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
                measurement = db.GetMeasurements(measurementID[0].PK); //measurement from a date is stored in that list
                                                                       //FCVx();
                TestDatesComboBox.ItemsSource = LoadTestDatesComboBoxData();
            }
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
            WFGUI.Index.AMKADateIndex = person[0].AMKA;
            Switcher.Switch(new PatientProfileChartPage(person, WFGUI.Index.DateIndex));
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

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void Comments_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(person[0].Comment);
        }

        private void AddExcel_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddExcel(person));
        }

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new EditPatientPage(person));
        }

        private void MakeModels(long PK)
        {
            DataAccess db = new DataAccess();
            measurement = db.GetMeasurements(PK);

            var fcvHistogram = Histogram.CreateEmpty(10, 80, 40);
            double count = 0;
            foreach (var x in measurement)
            {
                if (x.FCV != 0)
                {
                    fcvHistogram.Increment((int)x.FCV);
                    count++;// add individual data points
                }
            }

            OxyPlot.Axes.CategoryAxis xaxis = new OxyPlot.Axes.CategoryAxis();
            xaxis.Title = "FCV(m/s)";
            xaxis.TitleFontWeight = 700;
            xaxis.Position = AxisPosition.Bottom;
            xaxis.MajorGridlineStyle = LineStyle.Solid;
            xaxis.MinorGridlineStyle = LineStyle.Solid;


            OxyPlot.Axes.LinearAxis yaxis = new OxyPlot.Axes.LinearAxis();
            yaxis.Title = "%";
            yaxis.TitleFontWeight = 700;
            yaxis.Minimum = 0;
            yaxis.Maximum = 100;
            yaxis.Position = AxisPosition.Left;
            yaxis.MajorGridlineStyle = LineStyle.Solid;
            yaxis.MinorGridlineStyle = LineStyle.Solid;

            OxyPlot.Series.ColumnSeries s1 = new OxyPlot.Series.ColumnSeries();
            s1.IsStacked = true;
            s1.StrokeColor = OxyColors.Black;
            s1.FillColor = OxyColors.Black;
            s1.Background = OxyColors.Beige;
            count = 100.0 / count;
            foreach (var x in fcvHistogram.BinsAndValues)
            {
                s1.Items.Add(new ColumnItem(x.Value * count));
                xaxis.Labels.Add(x.Key.ToString().Substring(0, 3));
            }

            var Model = new PlotModel();
            Model.Title = "Conduction Velocities";
            Model.Background = OxyColors.GhostWhite;


            Model.Axes.Add(xaxis);
            Model.Axes.Add(yaxis);
            Model.Series.Add(s1);
            model.Add(Model);
        }
               
        private void SavePDF_Click(object sender, RoutedEventArgs e)
        {
            DataAccess db = new DataAccess();

            int count = measurementID.Count - 1;
            for (int i=0; i < count; i++)
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
                        doc.Add(new iTextSharp.text.Phrase($" AMKA:{AMKAtxt.Text}   Sex:{sextxt.Text}   "));
                        doc.Add(new Paragraph("Histograms"));


                        for (int i = 0; i < count; i++)
                        {
                            OxyPlot.Wpf.PngExporter.Export(model[i], "test.png", 1500, 800, OxyColors.White, 200);
                            png = Image.GetInstance("test.png");
                            png.ScalePercent(24f);
                            png.Alignment = Image.ALIGN_CENTER;
                            doc.Add(png);
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

    }
}