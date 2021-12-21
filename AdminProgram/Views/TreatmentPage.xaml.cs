using AdminProgram.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdminProgram
{
    /// <summary>
    /// TreatmentPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TreatmentPage : Page
    {
        OracleConnection conn;

        public TreatmentPage()
        {
            InitializeComponent();
            //DBConn();
        }

        //==DB 연결==//
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

        //==Data Grid Row 더블 클릭 시 이벤트 처리==//
        private void Row_DoubleClick(object sender, EventArgs args)
        {
            //선택된 환자의 진료 상세정보 가져오기 - 진행 중
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                TreatDetailWindow tw = new TreatDetailWindow();
                tw.ShowDialog();
            }
        }

        //==환자 검색==//
        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string name = searchTextBox.Text;//textbox에 작성된 값 가져옴

                if (conn == null)
                    DBConn();

                string? sql = null;
                if (name != null)
                {
                    sql = "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.PHONE_NUM, t.TREAT_DETAILS " +
                    "FROM PATIENT p, TREATMENT t " +
                    "WHERE p.PATIENT_ID = t.PATIENT_ID AND p.PATIENT_NAME LIKE '%" + name + "%'";
                }
                else
                {
                    sql = "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.PHONE_NUM, t.TREAT_DETAILS " +
                    "FROM PATIENT p, TREATMENT t " +
                    "WHERE p.PATIENT_ID = t.PATIENT_ID AND p.PATIENT_NAME";
                }

                /*OracleCommand comm = new OracleCommand();
                comm.Connection = conn;
                comm.CommandText = sql;

                OracleDataReader reader = comm.ExecuteReader();
                List<TreatmentModel> datas0 = new List<TreatmentModel>();

                while (reader.Read())
                {
                    datas0.Add(new TreatmentModel()
                    {
                        PatientNumber = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                        PatientPhoneNum = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                        TreatDetail = reader.GetString(reader.GetOrdinal("TREAT_DETAILS"))
                    });
                }

                treatDataGrid.ItemsSource = datas0;
                reader.Close();*/

                using (OracleCommand comm = new OracleCommand())
                {
                    comm.Connection = conn;
                    comm.CommandText = sql;
                    using (OracleDataReader reader = comm.ExecuteReader())
                    {
                        List<TreatmentModel> datas0 = new List<TreatmentModel>();

                        while (reader.Read())
                        {
                            datas0.Add(new TreatmentModel()
                            {
                                PatientNumber = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                PatientPhoneNum = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                                TreatDetail = reader.GetString(reader.GetOrdinal("TREAT_DETAILS"))
                            });
                        }

                        treatDataGrid.ItemsSource = datas0;
                    }
                }
            }
        }

        /*private void DBConnectionBtn(object sender, RoutedEventArgs e)
        {
            if (conn == null)
                DBConn();

            string sql = "select PATIENT_ID, PATIENT_NAME, GENDER from PATIENT order by PATIENT_ID";

            *//*Connection, Command, DataReader를 통한 데이터 추출*//*
            //OracleCommand : SQL 서버에 어떤 명령을 내리기 위해 사용하는 클래스 ===== 명령문 실행 용도
            //데이타를 가져오거나(SELECT), 테이블 내용을 삽입(INSERT), 갱신(UPDATE), 삭제(DELETE) 하기 위해
            //이 클래스를 사용할 수 있으며, 저장 프로시져(Stored Procedure)를 사용할 때도 사용
            OracleCommand comm = new OracleCommand();
            comm.Connection = conn;
            comm.CommandText = sql;

            //ExecuteReader : Sends the CommandText to the Connection and builds a SqlDataReader.
            //SQL Server와 연결을 유지한 상태에서 한번에 한 레코드(One Row)씩 데이타를 가져오는데 사용
            //DataReader는 하나의 Connection에 하나만 Open되어 있어야 하며, 사용이 끝나면 Close() 메서드를 호출하여 닫아 준다.
            OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            List<TreatmentModel> datas = new List<TreatmentModel>(); //listView에 데이터 뿌리기 위한 틀

            while (reader.Read())// 다음 레코드 계속 가져와서 루핑 - Advances the SqlDataReader to the next record. true if there are more rows; otherwise false.
            {
                datas.Add(new TreatmentModel()
                {
                    //GetString : Gets the column ordinal, given the name of the column.
                    //가져올 데이터의 컬럼명을 인수로 넣어줌
                    PatientNumber = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                    PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")), 
                    Gender = reader.GetString(reader.GetOrdinal("GENDER"))
                });
            }
            treatDataGrid.ItemsSource = datas; //dataGrid의 데이터 바인딩 진행
            reader.Close();
        }*/
    }
}
