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
    /// DiseaseManagementPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DiseaseManagementPage : Page
    {
        OracleConnection connn;

        public DiseaseManagementPage()
        {
            InitializeComponent();
            LogRecord.LogWrite("상병/시술페이지 오픈");
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

        //상병 SQL
        private void MakeSQL(ref string? sql)
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

            }
            //sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT order by PATIENT_ID";
        }

        //상병코드검색
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("상병 검색 버튼 클릭");
            string? sql = null;


            //if (CheckRightValue())
                return;

            ConnectDB();

            MakeSQL(ref sql);

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
                    Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
                    Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
                    Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                    Age = Calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
                });
            }
            dataGrid.ItemsSource = datas;

            reader.Close();

        }

        //시술 SQL
        private void MakeSQL(ref string? sql)
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

            }
            //sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT order by PATIENT_ID";
        }

        //시술코드검색
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("상병 검색 버튼 클릭");
            string? sql = null;


            //if (CheckRightValue())
            return;

            ConnectDB();

            MakeSQL(ref sql);

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
                    Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
                    Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
                    Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                    Age = Calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
                });
            }
            dataGrid.ItemsSource = datas;

            reader.Close();

        }

    }
}
