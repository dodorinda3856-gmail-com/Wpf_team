using AdminProgram.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

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

        private void Row_DoubleClick(object sender, EventArgs args)
        {
            //선택된 환자의 진료 상세정보 가져오기 - 진행 중
            var row = sender as DataGridRow;

            if (row != null && row.IsSelected)
            {
                ModifiyPatient tw = new ModifiyPatient();
                tw.ShowDialog();
            }
        }

        //날짜 
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

        //생년월일로 나이 구하는 함수
        private int Calculate_age(DateTime date)
        {
            int now = 2022;
            string str_tmp = date.ToString("yyyy");
            int age = Convert.ToInt32(str_tmp);
            age = now - age + 1;

            return age;
        }

        //나이로 출생년도 구하는 함수
        private string Get_birthyear(string age)
        {
            int a = Convert.ToInt32(age);
            string ret;
            a = 2022 - a;
            ret = a.ToString();
            return ret;
        }

        //제대로된 값이 들어오는지 확인하는 함수
        private bool CheckRightValue()
        {
            if (bod_txtbox.Text != "" && bod_txtbox.Text.Length != 8)
            {
                MessageBox.Show("나이: 8자리로 입력해주세요(ex 19990101)");
                return true;
            }
            if (bod_txtbox.Text != "" && (startAge_txtbox.Text != "" || endAge_txtbox.Text != ""))
            {
                MessageBox.Show("생년월일과 나이범위를 함께 쓸 수 없습니다.");
                bod_txtbox.Clear();
                startAge_txtbox.Clear();
                endAge_txtbox.Clear();
                return true;
            }
            return false;
        } 

        //DB연결
        private void ConnectDB()
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
        }

        //DB로 보낼 sql문 작성
        private void MakeSQL(ref string? sql, string endyear, string startyear)
        {
            if (patientNum_txtbox.Text != null || patientName_txtbox.Text != null || phoneNum_txtbox.Text != null)
            {
                if (gender_combobox.SelectedItem.ToString()[(gender_combobox.SelectedValue.ToString().Length - 1)..] == "-")
                {
                    if (bod_txtbox.Text == "")
                        sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                        "where PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                        "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                        "DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
                        "DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
                        "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                        " order by PATIENT_ID";
                    else
                        sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                        "where PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                        "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                        "DOB like To_Date('" + bod_txtbox.Text + "', 'yyyyMMDD') and " +
                        "DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
                        "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                        " order by PATIENT_ID";
                }
                else
                {
                    if (bod_txtbox.Text == "")
                        sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                        "where PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                        "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                        "GENDER = '" + gender_combobox.SelectedItem.ToString()[(gender_combobox.SelectedValue.ToString().Length - 1)..] + "' and " +
                        "DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
                        "DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
                        "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                        " order by PATIENT_ID";
                    else
                        sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                        "where PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                        "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                        "GENDER = '" + gender_combobox.SelectedItem.ToString()[(gender_combobox.SelectedValue.ToString().Length - 1)..] + "' and " +
                        "DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
                        "DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
                        "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                        " order by PATIENT_ID";
                }
            }
            //sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT order by PATIENT_ID";
        }

        //나이 범위 초기화 함수
        private void InitAge(ref string? startyear, ref string? endyear)
        {
            if (startAge_txtbox.Text == "") //빈칸으로 넘기면 null이 아니라 ""으로 들어있음
                endyear = "2030";
            else
                endyear = Get_birthyear(startAge_txtbox.Text);


            if (endAge_txtbox.Text == "")
                startyear = "1800";
            else
                startyear = Get_birthyear(endAge_txtbox.Text);
        }

        //검색 클릭 이벤트
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            string? startyear = null;
            string? endyear = null;
            string? sql = null;


            if (CheckRightValue())
                return;

            ConnectDB();

            InitAge(ref startyear, ref endyear);

            MakeSQL(ref sql, endyear, startyear);

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

            /*using (OracleCommand comm = new OracleCommand())
            {
                comm.Connection = conn;
                comm.CommandText = sql;

                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    List<PMModel> datas = new List<PMModel>();

                    while (reader.Read())
                    {
                        datas.Add(new PMModel()
                        {
                            Patient_ID = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                            Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")), //listView의 textblock id(?)
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
                            Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
                            Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                            Age = calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
                        });
                    }
                    dataGrid.ItemsSource = datas;
                }
            }*/
        }
    }
    
}

//테스트용
/*if (bod_txtbox.Text == "")
    MessageBox.Show(bod_txtbox.Text);*/