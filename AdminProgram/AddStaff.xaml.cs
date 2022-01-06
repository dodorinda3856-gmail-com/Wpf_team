using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
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
    /// AddStaff.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddStaff : Window
    {
        OracleConnection connn;


        public AddStaff()
        {
            InitializeComponent();
        }


        //성별 확인 함수
        public string checkgen()
        {
            if (male.IsChecked == true)
                return "M";
            else
                return "F";
        }

        //직책 확인 함수
        public string checkmajor()
        {
            if (doctor.IsChecked == true)
                return "D";
            else
                return "N";
        }

        //취소버튼 클릭 이벤트
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("취소하시겠습니까?", "취소", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
                Close();
        }

        //숫자만 있는지 확인하는 함수, 휴대폰 번호 유효성 체크시 사용
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
            if (staffName.Text == "")
            {
                MessageBox.Show("이름을 입력해주세요.");
                return true;
            }
            
            else if (medi_subject.Text == "")
            {
                MessageBox.Show("진료과목을 입력해주세요.");
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
            else if (doctor.IsChecked == false && nurse.IsChecked == false)
            {
                MessageBox.Show("직책을 선택해주세요.");
                return true;
            }

            else if (email.Text == "")
            {
                MessageBox.Show("이메일을 입력해주세요.");
                return true;
            }

            else if (staffLoginId.Text == "")
            {
                MessageBox.Show("아이디를 입력해주세요.");
                return true;
            }

            else if (staffPassword.Password == "")
            {
                MessageBox.Show("패스워드를 입력해주세요.");
                return true;
            }

            return false;
        }

        //sql문 작성 및 db 전달
        private void InsertSQL()
        {
            OracleCommand comm = new();


            comm.Connection = connn;
            comm.CommandText = "INSERT INTO MEDI_STAFF(STAFF_ID" +
                                                    ", STAFF_NAME" +
                                                    ", GENDER" +
                                                    ", MEDI_SUBJECT" +
                                                    ", PHONE_NUM" +
                                                    ", STAFF_EMAIL" +
                                                    ", POSITION" +
                                                    ")" +
                                " VALUES(MEDI_STAFF_SEQ.NEXTVAL" +
                                        ", '" + staffName.Text + "'" +
                                        ", '" + checkgen() + "'" +
                                        ", '" + medi_subject.Text + "'" +
                                        ", '" + phoneNum.Text + "'" +
                                        ", '" + email.Text + "'" +
                                        ", '" + checkmajor() + "'" +
                                        ")";          
            //실행시키는기능
            comm.ExecuteNonQuery();

            comm.CommandText = "INSERT INTO MEDI_STAFF_LOGIN(STAFF_ID, STAFF_LOGIN_ID, STAFF_LOGIN_PW) VALUES(MEDI_LOGIN_SEQ.NEXTVAL, " +
                                "'" + staffLoginId.Text + "'" +
                                ", '" + staffPassword.Password + "'" + ")";
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
