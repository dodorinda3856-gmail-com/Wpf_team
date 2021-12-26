using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using AdminProgram.Models;
using LiveCharts.Defaults;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections;

namespace AdminProgram
{
    /// <summary>
    /// PieChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PieChart : UserControl
    {
        OracleConnection conn;

        DateTime search = DateTime.Now;

        private void ConnectDB()
        {
            try
            {
                string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";
                conn = new OracleConnection(strCon);
                conn.Open();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void PieChartStart(DateTime t) 
        {
            ConnectDB();
            OracleCommand cmd = new();
            TMTModel type = new TMTModel();

            string sql = "SELECT count(*)" +
                "FROM(" +
                "(SELECT r.PATIENT_ID, r.TREAT_TYPE " +
                "FROM RESERVATION r LEFT JOIN PATIENT p ON p.PATIENT_ID = r.PATIENT_ID " +
                "WHERE p.PATIENT_STATUS_VAL = 'T' AND TO_CHAR(r.RESERVATION_DATE, 'YYYY-MM') BETWEEN TO_CHAR(SYSDATE, 'YYYY-MM') AND TO_CHAR(SYSDATE, 'YYYY-MM')) " +
                "UNION ALL " +
                "(SELECT w.PATIENT_ID, w.TREAT_TYPE " +
                "FROM WAITING w LEFT JOIN PATIENT p ON p.PATIENT_ID = w.PATIENT_ID " +
                "WHERE p.PATIENT_STATUS_VAL = 'T' AND TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY-MM') BETWEEN TO_CHAR(SYSDATE, 'YYYY-MM') AND TO_CHAR(SYSDATE, 'YYYY-MM') ) ) tc " +
                "GROUP BY tc.TREAT_TYPE";
            cmd.Connection = conn;
            cmd.CommandText = sql;


            OracleDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            ArrayList treatType = new ArrayList();
            while (read.Read())
            {
                treatType.Add(read.GetInt32(0));
            }


            SeriesCollection = new SeriesCollection {
                new PieSeries
                {
                    Title = "미용",
                    Values = new ChartValues<ObservableValue> { new ObservableValue ((int)treatType[0]) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "알러지",
                    Values = new ChartValues<ObservableValue> { new ObservableValue ((int)treatType[1]) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "두드러기",
                    Values = new ChartValues<ObservableValue> { new ObservableValue ((int)treatType[2]) },
                    DataLabels = true,

        }
            };

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }

        public PieChart()
        {

            InitializeComponent();

            PieChartStart(search);           
        }


        private void Next_Month(object sender, RoutedEventArgs e) {
            if (DateTime.Now.ToString("yyyy-MM") == search.ToString("yyyy-MM"))
            {
                MessageBox.Show("이번 달의 정보입니다.");
            }
            else {
                search.AddMonths(1);

                PieChartStart(search);
            }
        }

        private void Prev_Month(object sender, RoutedEventArgs e)
        {
            if (DateTime.Now.ToString("yyyy-MM") == search.ToString("yyyy-MM"))
            {
                MessageBox.Show("이번 달의 정보입니다.");
            }
            else
            {
                search.AddMonths(-1);

                PieChartStart(search);
            }
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
