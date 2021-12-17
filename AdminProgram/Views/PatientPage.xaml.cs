using AdminProgram.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace AdminProgram
{
    /// <summary>
    /// PatientPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PatientPage : Page
    {
        OracleConnection connn;

        public PatientPage()
        {
            InitializeComponent();
            gender_combobox.SelectedIndex = 0; //콤보박스 기본값설정
        }

        private void SelectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                MessageBox.Show("No Date");
            }
            else
            {
                //날짜 가져오는 부분
                MessageBox.Show(date.Value.ToShortDateString());
            }
        }

        private void AddPatient_Btn(object sender, RoutedEventArgs e)
        {
            AddPatient addPatient = new();
            addPatient.ShowDialog();
        }


        //나이 계산하는 함수
        private int Calculate_age(DateTime date)
        {
            int now = 2022;
            string str_tmp = date.ToString("yyyy");
            int age = Convert.ToInt32(str_tmp);
            age = now - age + 1;

            return age;
        }

        private string Get_birthyear(string age)
        {
            int a = Convert.ToInt32(age);
            string ret;
            a = 2022 - a;
            ret = a.ToString();
            return ret;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";
                connn = new OracleConnection(strCon);
                connn.Open();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

            string? sql = null;
            if (patientNum_txtbox.Text != null || patientNum_txtbox.Text != null)
            {
                sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                    "where PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                    "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                    //"GENDER LIKE '%" + gender_combobox.SelectedItem.ToString() + "%'" +
                    //"DOB LIKE '%" + bod_txtbox.Text + "%' and " +
                    //"DOB LIKE Between '" + Get_birthyear(startAge_txtbox.Text) + "-01-01' and '" + Get_birthyear(endAge_txtbox.Text) + "-12-31' and" +
                    "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                    " order by PATIENT_ID";
            }
            else 
                sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT order by PATIENT_ID";

            /* Connection, Command, DataReader를 통한 데이터 추출 */
            //OracleCommand : SQL 서버에 어떤 명령을 내리기 위해 사용하는 클래스 ===== 명령문 실행 용도
            //데이타를 가져오거나(SELECT), 테이블 내용을 삽입(INSERT), 갱신(UPDATE), 삭제(DELETE) 하기 위해
            //이 클래스를 사용할 수 있으며, 저장 프로시져(Stored Procedure)를 사용할 때도 사용
            OracleCommand comm = new();
            /*if (comm == null)
                DBConnection(this, null);*/
            comm.Connection = connn;
            comm.CommandText = sql;

            //ExecuteReader : Sends the CommandText to the Connection and builds a SqlDataReader.
            //SQL Server와 연결을 유지한 상태에서 한번에 한 레코드(One Row)씩 데이타를 가져오는데 사용
            //DataReader는 하나의 Connection에 하나만 Open되어 있어야 하며, 사용이 끝나면 Close() 메서드를 호출하여 닫아 준다.
            OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            List<PMModel> datas = new(); //listView에 데이터 뿌리기 위한 틀

            while (reader.Read())// 다음 레코드 계속 가져와서 루핑 - Advances the SqlDataReader to the next record. true if there are more rows; otherwise false.
            {
                datas.Add(new PMModel()
                {
                    //GetString : Gets the column ordinal, given the name of the column.
                    //가져올 데이터의 컬럼명을 인수로 넣어줌
                    Patient_ID = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                    Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")), //listView의 textblock id(?)
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
                    Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
                    Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                    Age = Calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
                }) ;
            }
            dataGrid.ItemsSource = datas; //listview의 데이터 바인딩 진행

            reader.Close();
        }
    }

}
