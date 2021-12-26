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

        public LiveChartEx()
        {
            InitializeComponent();

            ConnectDB();
            OracleCommand cmd = new();

            string sql = "SELECT count(t.d) AS month_total " +
                "FROM(SELECT TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY-MM') AS d " +
                "FROM WAITING w " +
                "WHERE TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY') BETWEEN TO_CHAR(SYSDATE, 'YYYY') AND TO_CHAR(SYSDATE, 'YYYY') " +
                "UNION ALL " +
                "SELECT TO_CHAR(r.RESERVATION_DATE, 'YYYY-MM') AS d " +
                "FROM RESERVATION r " +
                "WHERE TO_CHAR(r.RESERVATION_DATE, 'YYYY') BETWEEN TO_CHAR(SYSDATE, 'YYYY') AND TO_CHAR(SYSDATE, 'YYYY')) t , " +
                "(SELECT TO_CHAR(SYSDATE, 'YYYY') || '-' || LPAD(LEVEL, 2, '0') LV " +
                "FROM DUAL CONNECT BY LEVEL <= 12) b " +
                "WHERE b.LV = t.d(+) " +
                "GROUP BY b.LV " +
                "ORDER BY b.LV";
            cmd.Connection = conn;
            cmd.CommandText = sql;


            OracleDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            List<int> valuse = new List<int>();
            while (read.Read())
            {
                valuse.Add(read.GetInt32((read.GetOrdinal("month_total"))));
            }
            
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "환자 수",
                    Values = new ChartValues<int>(valuse)
                }
            };

            Labels = new[]

            {
                "1월", "2월", "3월", "4월", "5월", "6월",
                "7월", "8월", "9월", "10월", "11월", "12월"
            }; //월이 다 찍히지 않는 문제가...

            YFormatter = value => value + "명";

            DataContext = this;
        }
    }

}
