using AdminProgram.Models;
using Oracle.ManagedDataAccess.Client;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminProgram
{
    /// <summary>
    /// StaffPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StaffPage : Page
    {
        OracleConnection connn;

        public StaffPage()
        {
            LogRecord.LogWrite("의료진정보페이지 오픈");
            InitializeComponent();
            
        }

        //선택된 의료진 상세정보 가져오기
        private void Row_DoubleClick(object sender, EventArgs args)
        {
           
            var row = sender as DataGridRow;

            if (row != null && row.IsSelected)
            {
                StaffModel tmp = (StaffModel)row.Item;
                
                ModifyStaff.Passvalue = tmp;
                ModifyStaff tw = new ModifyStaff();
                
                tw.ShowDialog();
                
            }
        }



        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddStaff addStaff = new();
            LogRecord.LogWrite("의료진 추가 버튼 클릭");
            addStaff.ShowDialog();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("의료진 검색 버튼 클릭");
            string? checkgen = "";
            string? position = "";

            Debug.WriteLine("스탭아이디 : " + staffId_txtbox.Text);
            Debug.WriteLine("스탭이름 : " + staffName_txtbox.Text);
            Debug.WriteLine("스탭성별 : " + staffGender_combobox.Text);
            if (staffGender_combobox.Text.Equals("남"))
            {
                checkgen = "M";
            }
            else if(staffGender_combobox.Text.Equals("여"))
            {
                checkgen = "F";
            }
            Debug.WriteLine("진료과목 : " + staffMajor_combobox.Text);
            Debug.WriteLine("폰번호 : " + staffPhoneNumber_txtbox.Text);
            Debug.WriteLine("직책 : " + staffPosition_combobox.Text);
            
            if (staffPosition_combobox.Text.Equals("의사"))
            {
                position = "D";
            }
            else if (staffPosition_combobox.Text.Equals("간호사"))
            {
                position = "N";
            }
            

            string sql = "SELECT STAFF_ID, STAFF_NAME, GENDER, MEDI_SUBJECT, PHONE_NUM, STAFF_EMAIL, POSITION " +
                            "FROM MEDI_STAFF " +
                            "WHERE STAFF_ID LIKE '%" + staffId_txtbox.Text + "%' AND " +
                            "STAFF_NAME LIKE '%" + staffName_txtbox.Text + "%' AND " +
                            "GENDER LIKE '%" + checkgen + "%' AND " +
                            "MEDI_SUBJECT LIKE '%" + staffMajor_combobox.Text + "%' AND " +
                            "PHONE_NUM LIKE '%" + staffPhoneNumber_txtbox.Text + "%' AND " +
                            "POSITION LIKE '%" + position + "%' AND " +
                            "MEDI_STAFF_STATUS = 'T' " +
                            "order by staff_id";
            
            ConnectDB();
            OracleCommand comm = new();
            comm.Connection = connn;
            comm.CommandText = sql;

            OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            List<StaffModel> datas = new();

            while (reader.Read())
            {
                datas.Add(new StaffModel()
                {
                    Staff_id = reader.GetInt32(reader.GetOrdinal("STAFF_ID")),
                    Staff_name = reader.GetString(reader.GetOrdinal("STAFF_NAME")),
                    Gender = reader.GetString(reader.GetOrdinal("GENDER")),
                    Medi_subject = reader.GetString(reader.GetOrdinal("MEDI_SUBJECT")),
                    Phone_num = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                    Staff_email = reader.GetString(reader.GetOrdinal("STAFF_EMAIL")),
                    Position = reader.GetString(reader.GetOrdinal("POSITION"))
                });
            }
            dataGrid.ItemsSource = datas;

            reader.Close();
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


        //TO-DO : DB로 보낼 SQL문을 작성 할것. 

    }
}
