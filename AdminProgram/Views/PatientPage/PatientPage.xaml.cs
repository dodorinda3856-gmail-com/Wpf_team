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
            LogRecord.LogWrite("환자페이지 오픈");
            gender_combobox.SelectedIndex = 0;

        }

          
        //환자정보에서 안받아왔던 마케팅동의 알람, 집전화번호받아오는 함수
        void GetMarketingNum(ref string sql, ref PMModel tmp)
        {
            ConnectDB();
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = sql;

            OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                tmp.Agreemarketing = reader["AGREE_OF_ALARM"] as string;
                tmp.Home_Num = reader["HOME_NUM"] as string;
            }
            reader.Close();
        }

        //선택된 환자상세정보 가져오기
        private void Row_DoubleClick(object sender, EventArgs args)
        {
            var row = sender as DataGridRow;

            if (row != null && row.IsSelected)
            {
                PMModel tmp = (PMModel)row.Item;
                LogRecord.LogWrite("'" + tmp.Patient_Name + "' 환자정보 상세페이지 들어감");
                string? sql = "select AGREE_OF_ALARM, HOME_NUM from PATIENT where PATIENT_ID = " + tmp.Patient_ID;
                GetMarketingNum(ref sql, ref tmp);
               
                ModifiyPatient.Passvalue = tmp;
                ModifiyPatient.Passvalue.Agreemarketing = tmp.Agreemarketing;
                ModifiyPatient.Passvalue.Home_Num = tmp.Home_Num;
                ModifiyPatient tw = new ModifiyPatient();

                tw.ShowDialog();
                search.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        //날짜선택기능
        private void SelectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                MessageBox.Show("No Date");
            }
        }

        //환자추가하기 버튼이벤트
        private void AddPatient_Btn(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("환자페이지 추가 버튼 클릭");
            AddPatient addPatient = new();
            addPatient.ShowDialog();
            search.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
                MessageBox.Show("생년월일: 8자리로 입력해주세요(ex 19990101)");
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
                        "where PATIENT_STATUS_VAL = 'T' and " +
                        "PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                        "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                        "DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
                        "DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
                        "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                        " order by PATIENT_ID";
                    else
                        sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                        "where PATIENT_STATUS_VAL = 'T' and " + 
                        "PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
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
                        "where PATIENT_STATUS_VAL = 'T' and " +
                        "PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
                        "PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
                        "GENDER = '" + gender_combobox.SelectedItem.ToString()[(gender_combobox.SelectedValue.ToString().Length - 1)..] + "' and " +
                        "DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
                        "DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
                        "PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
                        " order by PATIENT_ID";
                    else
                        sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                        "where PATIENT_STATUS_VAL = 'T' and " +                                                                                                                                                                                                                                                                                                                   
                        "PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
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
        private void Search_Button_Click(object? sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("환자정보페이지 검색 버튼 클릭");
            string? startyear = null;
            string? endyear = null;
            string? sql = null;


            if (CheckRightValue())
                return;

            ConnectDB();

            InitAge(ref startyear, ref endyear);

            MakeSQL(ref sql, endyear, startyear);

            OracleCommand comm = new();
            
            comm.Connection = connn;
            comm.CommandText = sql;

            OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            List<PMModel> datas = new();

            while (reader.Read())
            {
                datas.Add(new PMModel()
                {
                    
                    Patient_ID = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                    Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")).Substring(0,7) + "*****",
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
                    Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
                    Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                    Age = Calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
                }) ;
            }
            dataGridPatient.ItemsSource = datas;

            reader.Close();

        }

        /*private void dataGridPatient_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridPatient.ItemsSource = start_value; //안돼유..
            bool test=dataGridPatient.IsEnabled;
            bool test2 =dataGridPatient.IsVisible;
        }*/
    }
    
}
