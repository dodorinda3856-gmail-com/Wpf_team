using AdminProgram.ViewModels;
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
        }

        /*Data Grid Row 더블 클릭 시 이벤트 처리*/
        private void Row_DoubleClick(object sender, EventArgs args)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                //PatientDetail detailWindow = new PatientDetail();
                //detailWindow.ShowDialog(); //해당 창을 닫기 전까지는 뒤에 있는 창으로 이동 못함

                //DataContext = new LiveChartEx();
            }
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                MessageBox.Show(sender.ToString().Substring(33));
        }

        private void DBConnection(object sender, RoutedEventArgs e)
        {
            //DB 연결
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

            string sql = "select PATIENT_ID, PATIENT_NAME, GENDER from PATIENT";

            /*Connection, Command, DataReader를 통한 데이터 추출*/
            //OracleCommand : SQL 서버에 어떤 명령을 내리기 위해 사용하는 클래스 ===== 명령문 실행 용도
            //데이타를 가져오거나(SELECT), 테이블 내용을 삽입(INSERT), 갱신(UPDATE), 삭제(DELETE) 하기 위해
            //이 클래스를 사용할 수 있으며, 저장 프로시져(Stored Procedure)를 사용할 때도 사용
            OracleCommand comm = new OracleCommand();
            if (conn == null)
                DBConnection(this, null);
            comm.Connection = conn;
            comm.CommandText = sql;

            //ExecuteReader : Sends the CommandText to the Connection and builds a SqlDataReader.
            //SQL Server와 연결을 유지한 상태에서 한번에 한 레코드(One Row)씩 데이타를 가져오는데 사용
            //DataReader는 하나의 Connection에 하나만 Open되어 있어야 하며, 사용이 끝나면 Close() 메서드를 호출하여 닫아 준다.
            OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            List<PatientViewModel> datas = new List<PatientViewModel>(); //listView에 데이터 뿌리기 위한 틀

            int a = 0;

            while (reader.Read())// 다음 레코드 계속 가져와서 루핑 - Advances the SqlDataReader to the next record. true if there are more rows; otherwise false.
            {
                datas.Add(new PatientViewModel()
                {
                    //GetString : Gets the column ordinal, given the name of the column.
                    //가져올 데이터의 컬럼명을 인수로 넣어줌
                    PatientNumber = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                    UserName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")), //listView의 textblock id(?)
                    Gender = reader.GetString(reader.GetOrdinal("GENDER"))
                });
            }

            dataGrid.ItemsSource = datas; //listview의 데이터 바인딩 진행

            //reader.Close();
        }
    }
}
