using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using AdminProgram.Models;
using AdminProgram.ViewModels;

namespace AdminProgram
{
    /// <summary>
    /// LiveChartEx.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LiveChartEx : UserControl
    {
        OracleConnection conn;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }


        public LiveChartEx()
        {
            InitializeComponent();
            List<int> liveChartData = new SPViewModel().LiveChartStart();

            SeriesCollection = new SeriesCollection { };
            SeriesCollection.Add(new LineSeries { Title = "환자 수", Values = new ChartValues<int>(liveChartData) });
            Labels = new[]
            {
                "1월", "2월", "3월", "4월", "5월", "6월",
                "7월", "8월", "9월", "10월", "11월", "12월"
            }; 
            YFormatter = value => value + "명";

            DataContext = this;
        }
    }

}
