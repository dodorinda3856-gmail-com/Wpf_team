using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminProgram.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.Specialized;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace AdminProgram.ViewModels
{
    public class SPViewModel : ObservableRecipient
    {
        private readonly ILogger _logger;
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        private ObservableCollection<TMTModel> tmtModel;
        public ObservableCollection<TMTModel> TmtModels
        {
            get { return tmtModel; }
            set { SetProperty(ref tmtModel, value); }
        }
        public SPViewModel()
        {
            TmtModels = new ObservableCollection<TMTModel>();
            TmtModels.CollectionChanged += ContentCollectionChanged;

            TodayReservation2();
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= ProductOnPropertyChanged;
                    ////_loger.LogInformation("리스트 삭제");
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                    ////_loger.LogInformation("리스트 불러옴");
                }
            }
        }

        // 데이터 변화 감지
        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var rModels = sender as ReservationListModel;
            if (rModels != null)
            {
                //_loger.LogInformation("{@rModels}", rModels);
                WeakReferenceMessenger.Default.Send(TmtModels); //이거 필수
                //_loger.LogInformation("ReservationList send 성공");
            }
        }

        public List<TMTModel> PieChartStart()
        {
            List<TMTModel> treatCount = new();

            string sql = "SELECT tc.TREAT_TYPE , count(*) AS TREAT_COUNT " +
                "FROM(" +
                "(SELECT r.PATIENT_ID, r.TREAT_TYPE " +
                "FROM RESERVATION r LEFT JOIN PATIENT p ON p.PATIENT_ID = r.PATIENT_ID " +
                "WHERE p.PATIENT_STATUS_VAL = 'T'  AND r.TREAT_TYPE IS NOT NULL AND TO_CHAR(r.RESERVATION_DATE, 'YYYY-MM') BETWEEN TO_CHAR(SYSDATE, 'YYYY-MM') AND TO_CHAR(SYSDATE, 'YYYY-MM')) " +
                "UNION ALL " +
                "(SELECT w.PATIENT_ID, w.TREAT_TYPE " +
                "FROM WAITING w LEFT JOIN PATIENT p ON p.PATIENT_ID = w.PATIENT_ID " +
                "WHERE p.PATIENT_STATUS_VAL = 'T' AND w.TREAT_TYPE IS NOT NULL AND TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY-MM') BETWEEN TO_CHAR(SYSDATE, 'YYYY-MM') AND TO_CHAR(SYSDATE, 'YYYY-MM') ) ) tc " +
                "GROUP BY tc.TREAT_TYPE";


            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {

                            try
                            {
                                while (reader.Read())
                                {
                                    treatCount.Add(new TMTModel()
                                    {
                                        TREAT_TYPE = reader.GetString(reader.GetOrdinal("TREAT_TYPE")),
                                        TREAT_COUNT = reader.GetInt32(reader.GetOrdinal("TREAT_COUNT"))
                                    });
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    _logger.LogInformation(e.ToString());
                }
            }
            return treatCount;
        }

        public List<int> LiveChartStart() {

            List<int> monthCount = new();

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

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {

                            try
                            {
                                while (reader.Read())
                                {
                                    monthCount.Add((reader.GetInt16(reader.GetOrdinal("month_total"))));
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    _logger.LogInformation(e.ToString());
                }
            }
            return monthCount;
        }
        public int WaitCount()
        {
            TMTModel tm = new TMTModel(); 
            string sql = "SELECT count(WATING_ID) AS WAIT_COUNT " +
                "FROM WAITING " +
                "WHERE WAIT_STATUS_VAL = 'T' AND TO_CHAR(REQUEST_TO_WAIT, 'yyyy-mm-dd') BETWEEN '" + (DateTime.Now).ToString("yyyy-MM-dd") + "'  AND '"+ (DateTime.Now).ToString("yyyy-MM-dd") + "' "
                ;

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read()) {
                                    tm.WAIT_COUNT = reader.GetInt16(reader.GetOrdinal("WAIT_COUNT"));
                                }

                            }
                            catch (Exception e) {
                                return 0;
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            return tm.WAIT_COUNT;
        }

        

        public void TodayReservation2()
        {
            string sql = "SELECT (SELECT PATIENT_NAME FROM PATIENT p WHERE p.PATIENT_ID = r.PATIENT_ID) AS patientName , TO_CHAR(r.RESERVATION_DATE , 'HH24:MI:SS') as reservationDT " +
                        "FROM RESERVATION r " +
                        "WHERE r.RESERVE_STATUS_VAL = 'T' AND TO_CHAR(r.RESERVATION_DATE , 'yyyy-mm-dd') BETWEEN '" + (DateTime.Now).ToString("yyyy-MM-dd") + "' AND '" + (DateTime.Now).ToString("yyyy-MM-dd") + "' " +
                        "ORDER BY RESERVATION_DATE";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    TmtModels = new ObservableCollection<TMTModel>();
                    TmtModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;
                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    TmtModels.Add(new TMTModel()
                                    {
                                        PATIENTNAME = reader.GetString(reader.GetOrdinal("patientName")),
                                        RESERVATIONDT = reader.GetString(reader.GetOrdinal("reservationDT"))
                                    });
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    //_logger.LogInformation(e.ToString());
                    TmtModels.Add(new TMTModel()
                    {
                        PATIENTNAME = "",
                        RESERVATIONDT = ""
                    });
                }
            }
        }

        private RelayCommand todayReservation22;
        public ICommand TodayReservation22 => todayReservation22 ??= new RelayCommand(TodayReservation2);

        public List<TMTModel> TodayReservation()
        {

            List<TMTModel> todayList = new();

            string sql = "SELECT (SELECT PATIENT_NAME FROM PATIENT p WHERE p.PATIENT_ID = r.PATIENT_ID) AS patientName , TO_CHAR(r.RESERVATION_DATE , 'HH24:MI:SS') as reservationDT " +
                        "FROM RESERVATION r " +
                        "WHERE r.RESERVE_STATUS_VAL = 'T' AND TO_CHAR(r.RESERVATION_DATE , 'yyyy-mm-dd') BETWEEN '" + (DateTime.Now).ToString("yyyy-MM-dd") + "' AND '" + (DateTime.Now).ToString("yyyy-MM-dd") + "' " +
                        "ORDER BY RESERVATION_DATE";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;
                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    todayList.Add(new TMTModel()
                                    {
                                        PATIENTNAME = reader.GetString(reader.GetOrdinal("patientName")),
                                        RESERVATIONDT = reader.GetString(reader.GetOrdinal("reservationDT"))
                                    });
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    //_logger.LogInformation(e.ToString());
                    todayList.Add(new TMTModel()
                    {
                        PATIENTNAME = "",
                        RESERVATIONDT =  ""
                    });
                }
            }
            return todayList;
        }
    }
}