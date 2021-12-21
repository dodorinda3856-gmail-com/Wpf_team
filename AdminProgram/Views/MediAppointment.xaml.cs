﻿using AdminProgram.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AdminProgram
{
    /// <summary>
    /// MediAppointmentPage.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MediAppointmentPage : Page
    {
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        public MediAppointmentPage()
        {
            InitializeComponent();
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
            {//날짜 가져오는 부분
                MessageBox.Show(date.Value.ToShortDateString());
            }
        }
        
        //==예약 환자 리스트 업데이트(임시)==//
        private void ReservationUpdateBtn(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT ms.STAFF_NAME FROM RESERVATION r JOIN MEDI_STAFF ms ON r.MEDICAL_STAFF_ID = ms.STAFF_ID";
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            try
                            {
                                List<MAModel> datas = new List<MAModel>();

                                while (reader.Read())
                                {
                                    datas.Add(new MAModel()
                                    {
                                        StaffName = reader.GetString(reader.GetOrdinal("STAFF_NAME"))
                                    });
                                }
                                reservationGrid.ItemsSource = datas;
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        //==예약 등록 윈도우로 이동==//
        private void addAppointmentBtn(object sender, RoutedEventArgs e)
        {
            AddAppointment aa = new AddAppointment();
            aa.Title = "진료 예약 등록";
            aa.ShowDialog();
        }

        //==예약 수정, 삭제 윈도우로 이동==//
        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                AppointmentDetailWindow ad = new AppointmentDetailWindow();
                ad.Title = "진료 예약 수정 / 삭제 페이지";
                ad.ShowDialog();
            }
        }
    }
}
