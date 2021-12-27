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

        //마케팅 동의확인 함수
        private string checksms()
        {
            if (smsCheck.IsChecked == true)
                return "T";
            else
                return "F";
        }

        //성별 확인 함수
        private string checkgen()
        {
            if (male.IsChecked == true)
                return "M";
            else
                return "F";
        }
        //취소버튼 클릭 이벤트
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("취소하시겠습니까?", "취소", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
                Close();
        }

        //숫자만 있는지 확인하는 함수
        private bool NotOnlyNum(string str)
        {
            for (int j = 0; j < str.Length; j++)
            {
                if (!(str[j] >= '0' && str[j] <= '9'))
                    return true;
                j++;
            }
            return false;
        }

        //db연결
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


        //sql문 작성 및 db 전달
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

            //실행시키는기능
            comm.ExecuteNonQuery();
            connn.Close();
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

                MessageBox.Show("저장되었습니다.");
                Close();
            }
        }        
    }
}
