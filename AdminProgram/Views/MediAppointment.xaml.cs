using AdminProgram.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace AdminProgram
{
    /// <summary>
    /// MediAppointmentPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MediAppointmentPage : Page
    {
        OracleConnection conn;

        public MediAppointmentPage()
        {
            InitializeComponent();
            DBConn();
        }

        public void DBConn()
        {
            try
            {
                string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";
                conn = new OracleConnection(strCon);
                conn.Open();

                MessageBox.Show("DB Connection OK...");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        //혹시나 날짜 값이 필요할 경우 이거를 활용해서 할 것
        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if(date == null)
            {
                MessageBox.Show("No Date");
            }
            else
            {
                //날짜 가져오는 부분
                MessageBox.Show(date.Value.ToShortDateString());
            }
            
        }

        //==예약 환자 리스트 업데이트(임시)==//
        private void ReservationUpdateBtn(object sender, RoutedEventArgs e)
        {
            if(conn == null)
            {
                DBConn();
            }

            string sql = "SELECT ms.STAFF_NAME FROM RESERVATION r, MEDI_STAFF ms WHERE r.MEDICAL_STAFF_ID = ms.STAFF_ID ;";

            OracleCommand comm = new OracleCommand();
            comm.Connection = conn;
            comm.CommandText = sql;

            OracleDataReader reader = comm.ExecuteReader();
            List<ReservationModel> datas = new List<ReservationModel>();

            while (reader.Read())
            {
                datas.Add(new ReservationModel()
                {
                    StaffName = reader.GetString(reader.GetOrdinal("STAFF_NAME"))
                });
            }

            reservationGrid.ItemsSource = datas;
        }

        //==진료 예약 등록 페이지 이동==//
        private void addAppointmentBtn(object sender, RoutedEventArgs e)
        {
            AddAppointment aa = new AddAppointment();
            aa.Title = "진료 예약 등록";
            aa.ShowDialog();
        }
    }
}
