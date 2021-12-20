using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace AdminProgram
{
    /// <summary>
    /// AddPatient.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddPatient : Window
    {
        OracleConnection connn;

        public AddPatient()
        {
            InitializeComponent();
        }

        private void selectedDate(object sender, SelectionChangedEventArgs e)
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Copy_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Copy1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Copy2_Checked(object sender, RoutedEventArgs e)
        {

        }

        //취소버튼 클릭 이벤트
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("취소하시겠습니까?", "취소", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
                Close();
        }

        //입력이 잘되었는지 판단하는 함수
        private bool CheckRightValue()
        {
            if (patientName.Text == "")
            {
                MessageBox.Show("이름을 입력해주세요.");
                return true;
            }
            else if (securityNum.Text == "" || securityNum.Text.Length != 13)
            {
                MessageBox.Show("주민번호는 13자리로 입력해주세요.");
                return true;
            }
            else if (address.Text == "")
            {
                MessageBox.Show("주소를 입력해주세요.");
                return true;
            }
            else if (phoneNum.Text == "")
            {
                MessageBox.Show("핸드폰 번호를 입력해주세요.");
                return true;
            }
            else if (male.IsChecked == false && female.IsChecked == false)
            {
                MessageBox.Show("성별을 선택해주세요.");
                return true;
            }
            else if (yang.IsChecked == false && ymm.IsChecked == false)
            {
                MessageBox.Show("양력/음력을 선택해주세요.");
                return true;
            }
            else if (datePicker.Text == "")
            {
                MessageBox.Show("생일을 선택해주세요.");
                return true;
            }
            return false;
        }



        private void InsertSQL()
        {
            string? sql = null;
            OracleCommand comm = new();

            sql = "INSERT INTO PATIENT (PATIENT_ID, " +
                                       "PATIENT_NAME, " +
                                       "RESIDENT_REGIST_NUM, " +
                                       "ADDRESS, " +
                                       "PHONE_NUM, " +
                                       "GENDER, " +
                                       "DOB, " +
                                       "AGREE_OF_ALARM, " +
                                       "HOME_NUM, " +
                                       "REGIST_DATE) " +
                        "VALUES (PATIENT_SEQ.NEXTVAL, " +
                                ":name, " +
                                ":securityNum, " +
                                ":address, " +
                                ":phoneNum, " +
                                ":gender, " +
                                ":date, " +
                                ":smsCheck, " +
                                ":homeNum, " +
                                "'2017-09-28')";

            comm.Parameters.Add(new OracleParameter("name", patientName.Text));
            comm.Parameters.Add(new OracleParameter("securityNum", securityNum.Text));
            comm.Parameters.Add(new OracleParameter("address", address.Text));
            comm.Parameters.Add(new OracleParameter("phoneNum", phoneNum.Text));
            if (homeNum.Text == "")
                comm.Parameters.Add(new OracleParameter("homeNum", null));
            else
                comm.Parameters.Add(new OracleParameter("homeNum", homeNum.Text));
            if (smsCheck.IsChecked == true)
                comm.Parameters.Add(new OracleParameter("smsCheck", "T"));
            else
                comm.Parameters.Add(new OracleParameter("smsCheck", "F"));
            if (male.IsChecked == true)
                comm.Parameters.Add(new OracleParameter("gender", "M"));
            else
                comm.Parameters.Add(new OracleParameter("gender", "F"));
            comm.Parameters.Add(new OracleParameter("date", datePicker.Text));

            
            comm.Connection = connn;
            comm.CommandText = sql;

            // SQL문 지정 및 INSERT 실행
            comm.ExecuteNonQuery(); //에러뜨는데 현재 , 문제라는데 나는 잘못한게 없음 아마도 들어는 값을 확인해봐야할것같다.
            connn.Close();
            connn = null;
        }

        //저장버튼 클릭 이벤트
        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                if (CheckRightValue())
                    return;

                ConnectDB();

                InsertSQL();
            }
        }        
    }
}
