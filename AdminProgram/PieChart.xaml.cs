using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows;
using AdminProgram.ViewModels;
using System.Windows.Controls;
using AdminProgram.Models;
using LiveCharts.Defaults;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AdminProgram
{
    /// <summary>
    /// PieChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PieChart : UserControl
    {

        private readonly ILogger _logger;
        public SeriesCollection _seriesCollection { get; set; }

        public PieChart()
        {

            InitializeComponent();
            List<TMTModel> pieDatas = new SPViewModel().PieChartStart();

            _seriesCollection = new SeriesCollection{};
            foreach (TMTModel item in pieDatas) { 
                _seriesCollection.Add(new PieSeries { Title = item.TREAT_TYPE, Values = new ChartValues<ObservableValue> { new ObservableValue(item.TREAT_COUNT) }, DataLabels = true });
            }


            DataContext = this;
        }
        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
