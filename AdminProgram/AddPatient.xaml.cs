using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        //취소버튼 클릭 이벤트
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("취소하시겠습니까?", "취소", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
                Close();
        }

        private bool NotOnlyNum(string str)
        {
            int i = 0;
            for (int j = 0; j < str.Length; j++)
            {
                if ((str[j] >= 'a' && str[j] <= 'z') || (str[j] >= 'A' && str[j] <= 'Z'))
                    return true;
                j++;
            }
            return false;
        }

        //입력이 잘되었는지 판단하는 함수
        private bool CheckRightValue()
        {
            if (patientName.Text == "")
            {
                MessageBox.Show("이름을 입력해주세요.");
                return true;
            }
            else if (securityNum.Text == "" || securityNum.Text.Length != 13 || NotOnlyNum(securityNum.Text))
            {
                MessageBox.Show("주민번호는 숫자 13자리로 입력해주세요.\nex)95092810XXXXX");
                return true;
            }
            else if (address.Text == "")
            {
                MessageBox.Show("주소를 입력해주세요.");
                return true;
            }
            else if (phoneNum.Text == "" || NotOnlyNum(phoneNum.Text))
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
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "INSERT INTO PATIENT(PATIENT_ID" +
                                                    ", RESIDENT_REGIST_NUM" +
                                                    ", PATIENT_NAME" +
                                                    ", ADDRESS" +
                                                    ", PHONE_NUM" +
                                                    ", GENDER" +
                                                    ", DOB" +
                                                    ", AGREE_OF_ALARM" +
                                                    ", HOME_NUM" +
                                                    ", REGIST_DATE" +
                                                    ", PATIENT_STATUS_VAL) " +
                                "VALUES(PATIENT_SEQ.NEXTVAL" +
                                        ", '" + securityNum.Text + "'" +
                                        ", '" + patientName.Text + "'" +
                                        ", '" + address.Text + "'" +
                                        ", '" + phoneNum.Text + "'" +
                                        ", '" + checkgen() + "'" +
                                        ", To_Date('" + datePicker.Text + "', 'yyyy-MM-dd')" +
                                        ", '" + checksms() + "'" +
                                        ", '" + homeNum.Text + "'" +
                                        ", To_Date('2021-12-20', 'yyyy-MM-dd')" +
                                        ", 'T')";
                                        
            /*  
                                        ", :securityNum" +
                                        ", :name" +
                                        ", :address" +
                                        ", :phoneNum" +
                                        ", :gender" +
                                        ", To_Date(:date, 'yyyy-MM-dd')" +
                                        ", :smsCheck" +
                                        ", :homeNum" +
                                        ", To_Date('2021-12-20', 'yyyy-MM-dd'))";

            comm.Parameters.Add(new OracleParameter("name", patientName.Text));
            comm.Parameters.Add(new OracleParameter("securityNum", securityNum.Text));
            comm.Parameters.Add(new OracleParameter("address", address.Text));
            comm.Parameters.Add(new OracleParameter("phoneNum", phoneNum.Text));
            comm.Parameters.Add(new OracleParameter("homeNum", homeNum.Text));
            comm.Parameters.Add(new OracleParameter("smsCheck", checksms()));
            comm.Parameters.Add(new OracleParameter("gender", checkgen()));
            comm.Parameters.Add(new OracleParameter("date", datePicker.Text)); */

            // SQL문 지정 및 INSERT 실행
            comm.ExecuteNonQuery();
            connn.Close();
        }

        private string checksms()
        {
            if (smsCheck.IsChecked == true)
                return "T";
            else
                return "F";
        }

        private string checkgen()
        {
            if (male.IsChecked == true)
                return "M";
            else
                return "F";
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

                Close();
            }
        }        
    }
}
