namespace WFGUI
{
    using Extreme.DataAnalysis;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System.Collections.Generic;
    using WPFUI;

    public class VelocitiesViewModel
    {

        List<MeasurementID> measurementID = new List<MeasurementID>();
        List<Measurement> measurement = new List<Measurement>();
        List<DiagnosisHistogram> diagnosis1 = new List<DiagnosisHistogram>();
        List<DiagnosisHistogram> diagnosis9 = new List<DiagnosisHistogram>();
        public PlotModel Model { get; set; }
        public PlotModel ModelALS { get; set; }
        public PlotModel ModelNormal { get; set; }
        public string Title { get; private set; }

        /// <summary>
        /// <c>VelocitiesViewModel()</c> provides the model-histogram for the PatientProfileChart
        /// </summary>
        public VelocitiesViewModel()
        {
            /***** Retrieve the measurements from a particular Date *****/

            DataAccess db = new DataAccess();

            measurementID = db.GetMeasurementIDs(Index.AMKADateIndex); //get the info about this particular study
            diagnosis1 = db.GetDiagnosisHistogram(1);
            diagnosis9 = db.GetDiagnosisHistogram(9);
            if (measurementID.Count != 0)
            {
                measurement = db.GetMeasurements(measurementID[Index.DateIndex].PK); //get the measurements from dbo.Measurements according to the PK from dbo.MeasurementsID
            }


            /***** Create the Patient's Histogram Bins *****/
            int minHist = 0;
            int maxHist = 84;
            int minYaxis = 0;
            int maxYaxis = 100;

            var fcvHistogram = Histogram.CreateEmpty( minHist, maxHist);
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
                    fcvHistogram.Increment((int)x.FCV); //(int)1.5 = 1. I want to increment
                    count++;// add individual data points
                }
            }

            /***** Create the visual part of the Histogram ******/

            this.Title = "Conduction Velocities";

            OxyPlot.Axes.LinearAxis xaxis = new OxyPlot.Axes.LinearAxis();
            xaxis.Title = "FCV(m/s)";
            xaxis.Minimum = 30;
            xaxis.Maximum = maxHist;
            xaxis.MajorStep = 2;
            xaxis.MinimumMinorStep = 2;
            xaxis.TitleFontWeight = 700;
            xaxis.Position = AxisPosition.Bottom;
            xaxis.MajorGridlineStyle = LineStyle.Solid;
            xaxis.MinorGridlineStyle = LineStyle.Solid;
            xaxis.MinorGridlineThickness = 1.5;

            OxyPlot.Axes.LinearAxis yaxis = new OxyPlot.Axes.LinearAxis();
            yaxis.Title = "%";
            yaxis.Minimum = minYaxis;
            yaxis.Maximum = maxYaxis;
            yaxis.MajorStep = 20;
            yaxis.MajorTickSize = 10;
            yaxis.TitleFontWeight = 700;
            yaxis.Position = AxisPosition.Left;
            yaxis.MajorGridlineStyle = LineStyle.Solid;
            yaxis.MinorGridlineStyle = LineStyle.Solid;

            OxyPlot.Axes.LinearAxis xaxis2 = new OxyPlot.Axes.LinearAxis();
            xaxis2.Title = "FCV(m/s)";
            xaxis2.Minimum = 30;
            xaxis2.Maximum = maxHist;
            xaxis2.MajorStep = 2;
            xaxis2.MinimumMinorStep = 2;
            xaxis2.TitleFontWeight = 700;
            xaxis2.Position = AxisPosition.Bottom;
            xaxis2.MajorGridlineStyle = LineStyle.Solid;
            xaxis2.MinorGridlineStyle = LineStyle.Solid;
            xaxis2.MinorGridlineThickness = 1.5;

            OxyPlot.Axes.LinearAxis yaxis2 = new OxyPlot.Axes.LinearAxis();
            yaxis2.Title = "%";
            yaxis2.TitleFontWeight = 700;
            yaxis2.MajorStep = 5;
            yaxis2.MajorTickSize = 10;
            //yaxis2.StringFormat = "0.00%";            
            yaxis2.Minimum = minYaxis;
            yaxis2.Maximum = 25;
            yaxis2.Position = AxisPosition.Left;
            yaxis2.MajorGridlineStyle = LineStyle.Solid;
            yaxis2.MinorGridlineStyle = LineStyle.Solid;

            OxyPlot.Axes.LinearAxis xaxis3 = new OxyPlot.Axes.LinearAxis();
            xaxis3.Title = "FCV(m/s)";
            xaxis3.Minimum = 30;
            xaxis3.Maximum = maxHist;
            xaxis3.MajorStep = 2;
            xaxis3.MinimumMinorStep = 2;
            xaxis3.TitleFontWeight = 700;
            xaxis3.Position = AxisPosition.Bottom;
            xaxis3.MajorGridlineStyle = LineStyle.Solid;
            xaxis3.MinorGridlineStyle = LineStyle.Solid;
            xaxis3.MinorGridlineThickness = 1.5;

            OxyPlot.Axes.LinearAxis yaxis3 = new OxyPlot.Axes.LinearAxis();
            yaxis3.Title = "%";
            yaxis3.TitleFontWeight = 700;
            yaxis3.MajorStep = 5;
            yaxis3.MajorTickSize = 10;
            yaxis3.Minimum = minYaxis;
            yaxis3.Maximum = 25;
            yaxis3.Position = AxisPosition.Left;
            yaxis3.MajorGridlineStyle = LineStyle.Solid;
            yaxis3.MinorGridlineStyle = LineStyle.Solid;

            /* Searched Patients' Bars */

            OxyPlot.Series.RectangleBarSeries bars = new OxyPlot.Series.RectangleBarSeries();
            bars.FillColor = OxyColors.Gray;
            bars.StrokeColor = OxyColors.Black;
            bars.StrokeThickness = 1;
            bars.Title="Patient";

            count = 100.0/count;
            double x0 = 0, temp=0;

            foreach (var x in fcvHistogram.BinsAndValues)
            {
                if (x.Key % 2 != 0)
                {
                    bars.Items.Add(new RectangleBarItem(x0, 0.0, x0 + 2, (x.Value + temp) * count));
                    x0 = x0 + 2;
                    
                }
                temp = x.Value;
            }


            /* ALS Patients' Bars */

            OxyPlot.Series.RectangleBarSeries bars2 = new OxyPlot.Series.RectangleBarSeries();
            bars2.StrokeColor = OxyColors.Black;
            bars2.StrokeThickness = 1;
            bars2.FillColor = OxyColors.Maroon;
            bars2.Title = "ALS";

            foreach (var x in diagnosis1 )
            {
                if (x.Percentage != 0)
                {
                    bars2.Items.Add(new RectangleBarItem(x.Interval , 0.0, x.Interval + 2, x.Percentage * 100));
                }           


            }

            /* Normal Patients' Bars */

            OxyPlot.Series.RectangleBarSeries bars3 = new OxyPlot.Series.RectangleBarSeries();
            bars3.StrokeColor = OxyColors.Black;
            bars3.StrokeThickness = 1;
            bars3.FillColor = OxyColors.MidnightBlue;
            bars3.Title = "Normal";
            
            foreach (var x in diagnosis9)
            {

                bars3.Items.Add(new RectangleBarItem(x.Interval, 0.0, x.Interval + 2, x.Percentage * 100));


            }

            /* View Model's assemblage */

            Model = new PlotModel();
            Model.Title = "Conduction Velocities";
            Model.Background = OxyColors.White;        
            Model.Axes.Add(xaxis);
            Model.Axes.Add(yaxis);
            Model.Series.Add(bars);

            ModelALS = new PlotModel();
            ModelALS.Background = OxyColors.White;
            ModelALS.Axes.Add(xaxis2);
            ModelALS.Axes.Add(yaxis2);
            ModelALS.Series.Add(bars2);


            ModelNormal = new PlotModel();
            ModelNormal.Background = OxyColors.White;
            ModelNormal.Axes.Add(xaxis3);
            ModelNormal.Axes.Add(yaxis3);
            ModelNormal.Series.Add(bars3);



        }
    }
}


/* Three Histograms Into The Same Model */

//public VelocitiesViewModel() 
//{
//    /***** Retrieve the measurements from a particular Date *****/

//    DataAccess db = new DataAccess();

//    measurementID = db.GetMeasurementIDs(Index.AMKADateIndex); //get the info about this particular study
//    diagnosis1 = db.GetDiagnosisHistogram(1);
//    diagnosis9 = db.GetDiagnosisHistogram(9);
//    if (measurementID.Count != 0)
//    {
//        measurement = db.GetMeasurements(measurementID[Index.DateIndex].PK); //get the measurements from dbo.Measurements according to the PK from dbo.MeasurementsID
//    }


//    /***** Create the Patient's Histogram Bins *****/

//    var fcvHistogram = Histogram.CreateEmpty(0, 86);
//    double count = 0;
//    float xmin = 10000;
//    float xmax = 0;
//    foreach (var x in measurement)
//    {
//        if (x.FCV < xmin)
//        {
//            xmin = x.FCV;
//        }
//        if (x.FCV > xmax)
//        {
//            xmax = x.FCV;
//        }
//        if (x.FCV != 0)
//        {
//            fcvHistogram.Increment((int)x.FCV); //(int)1.5 = 1. I want to inceremen
//            count++;// add individual data points
//        }
//    }

//    /***** Create the visual part of the Histogram ******/

//    this.Title = "Conduction Velocities";

//    OxyPlot.Axes.LinearAxis xaxis = new OxyPlot.Axes.LinearAxis();
//    xaxis.Title = "FCV(m/s)";
//    xaxis.Minimum = 30;
//    xaxis.Maximum = 86;
//    xaxis.TitleFontWeight = 700;
//    xaxis.Position = AxisPosition.Bottom;
//    xaxis.MajorGridlineStyle = LineStyle.Solid;
//    xaxis.MinorGridlineStyle = LineStyle.Solid;
//    xaxis.MinorGridlineThickness = 1.5;

//    OxyPlot.Axes.LinearAxis yaxis = new OxyPlot.Axes.LinearAxis();
//    yaxis.Title = "%";
//    yaxis.TitleFontWeight = 700;
//    yaxis.Position = AxisPosition.Left;
//    yaxis.MajorGridlineStyle = LineStyle.Solid;
//    yaxis.MinorGridlineStyle = LineStyle.Solid;

//    /* Searched Patients' Bars */

//    OxyPlot.Series.RectangleBarSeries bars = new OxyPlot.Series.RectangleBarSeries();
//    bars.FillColor = OxyColors.Gray;
//    bars.StrokeColor = OxyColors.Black;
//    bars.StrokeThickness = 1;
//    bars.Title = "Patient";

//    count = 100.0 / count;
//    double x0 = 0, temp = 0;

//    foreach (var x in fcvHistogram.BinsAndValues)
//    {

//        if (x.Key % 2 != 0)
//        {
//            bars.Items.Add(new RectangleBarItem(x0, 0.0, x0 + 2, (x.Value + temp) * count));
//            x0 = x0 + 2;

//        }
//        temp = x.Value;
//    }


//    /* ALS Patients' Bars */

//    OxyPlot.Series.RectangleBarSeries bars2 = new OxyPlot.Series.RectangleBarSeries();
//    bars2.StrokeColor = OxyColors.Red;
//    bars2.StrokeThickness = 1.5;
//    bars2.FillColor = OxyColors.Transparent;
//    bars2.Title = "ALS";

//    foreach (var x in diagnosis1)
//    {
//        if (x.Percentage != 0)
//        {
//            bars2.Items.Add(new RectangleBarItem(x.Interval, 0.0, x.Interval + 2, x.Percentage * 100));
//        }


//    }

//    /* Normal Patients' Bars */

//    OxyPlot.Series.RectangleBarSeries bars3 = new OxyPlot.Series.RectangleBarSeries();
//    bars3.StrokeColor = OxyColors.Blue;
//    bars3.StrokeThickness = 1.5;
//    bars3.FillColor = OxyColors.Transparent;
//    bars3.Title = "Normal";

//    foreach (var x in diagnosis9)
//    {

//        bars3.Items.Add(new RectangleBarItem(x.Interval, 0.0, x.Interval + 2, x.Percentage * 100));


//    }

//    /* View Model's assemblage */

//    Model = new PlotModel();
//    Model.Title = "Conduction Velocities";
//    Model.Background = OxyColors.White;
//    Model.Axes.Add(xaxis);
//    Model.Axes.Add(yaxis);
//    Model.Series.Add(bars);
//    Model.Series.Add(bars2);
//    Model.Series.Add(bars3);


//}

