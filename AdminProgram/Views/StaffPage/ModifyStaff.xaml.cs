using AdminProgram.Models;
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
    /// ModifyStaff.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyStaff : Window
    {
        OracleConnection connn;
        int staffId = 0;
        //static으로 다른 클래스의 객체정보 받아올 변수
        private static StaffModel Form2_value;
        public static StaffModel Passvalue
        {
            get { return Form2_value; }
            set { Form2_value = value; }
        }

        //실무진 정보 시작과 동시에 초기화함수
        void InitStaff()
        {
            staffName.Text = Passvalue.Staff_name;
            medi_subject.Text = Passvalue.Medi_subject;
            email.Text = Passvalue.Staff_email;
            phoneNum.Text = Passvalue.Phone_num;
            staffId = Passvalue.Staff_id;
            if (Passvalue.Gender == "M")
            {
                male.IsChecked = true;
            }
            else
            {
                female.IsChecked = true;
            }
                
            if (Passvalue.Position == "D")
            {
                doctor.IsChecked = true;
            }
            else
            {
                nurse.IsChecked = true;
            }
        }


        public ModifyStaff()
        {
            InitializeComponent();
            InitStaff();
            LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 상세 페이지 오픈");
        }

        //DB 연결
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

        //수정버전 sql
        private void ModifiedInsertSQL()
        {
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "UPDATE MEDI_STAFF " +
                                "SET STAFF_NAME = '" + staffName.Text + "'" +
                                  ", MEDI_SUBJECT = '" + medi_subject.Text + "'" +
                                  ", PHONE_NUM = '" + phoneNum.Text + "'" +
                                  ", STAFF_EMAIL = '" + email.Text + "' " +
                                "where STAFF_ID = " + staffId;

            //실행시키는기능
            comm.ExecuteNonQuery();
            connn.Close();
        }

        //삭제버전 sql medi_Staff 및 medi_staff_login 상태를 둘다 F로 변경.
        private void DeleteInsertSQL()
        {
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "update MEDI_STAFF " +
                                "set MEDI_STAFF_STATUS = 'F' " +
                                "where STAFF_ID = " + staffId;

            //실행시키는기능
            comm.ExecuteNonQuery();

            comm.CommandText = "update MEDI_STAFF_LOGIN " +
                                "set MEDI_STAFF_LOGIN_STATUS = 'F' " +
                                "where STAFF_ID = " + staffId;
            comm.ExecuteNonQuery();
            connn.Close();
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
            else if (email.Text == "")
            {
                MessageBox.Show("이메일을 입력해주세요.");
                return true;
            }
            else if (phoneNum.Text == "" || NotOnlyNum(phoneNum.Text) || phoneNum.Text.Length != 11)
            {
                MessageBox.Show("핸드폰 번호 11자리를 입력해주세요.\nex) 010XXXXXXXX");
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

            

            return false;
        }

        //수정버튼 이벤트
        private void Modify_btn_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 정보 수정 버튼 클릭");
            var result = MessageBox.Show("위 내용으로 수정하시겠습니까?", "수정", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                if (CheckRightValue())
                    return;
                LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 정보 수정 완료 버튼 클릭");
                ConnectDB();

                ModifiedInsertSQL();

                MessageBox.Show("수정되었습니다.");
                Close();
            }
            else
                LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 정보 수정 취소 버튼 클릭");
        }

        //삭제버튼 이벤트
        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 정보 삭제 버튼 클릭");
            var result = MessageBox.Show("의료진 정보를 삭제하시겠습니까?", "삭제", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 정보 삭제 완료버튼 클릭");
                ConnectDB();

                DeleteInsertSQL();

                MessageBox.Show("해당 의료진이 삭제되었습니다.");
                Close();
            }
            else
                LogRecord.LogWrite("'" + Form2_value.Staff_name + "' 의료진 정보 삭제 취소 버튼 클릭");
        }
    }
}
