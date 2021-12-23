using System;
using System.Windows;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using AdminProgram.ViewModels;
using System.Diagnostics;

namespace AdminProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary
    public partial class MainWindow : Window
    {
        OracleConnection conn;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectDB()
        {
            try
            {
                string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";
                conn = new OracleConnection(strCon);
                conn.Open();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }


        private void Login_Click_Btn(object sender, RoutedEventArgs e)
        {
            string sql = "";
            if (string.IsNullOrEmpty(txtBoxUserName.Text))
            {
                userNameTip.Visibility = Visibility.Visible;
                userNameTip.Content = "아이디를 입력하세요!";
                return;

            }else
            {
                if (string.IsNullOrEmpty(txtBoxPwd.Password))
                {
                    pwdTip.Visibility = Visibility.Visible;
                    pwdTip.Content = "비밀번호를 입력하세요!";
                    return;
                }
                else
                {
                    ConnectDB();
                    OracleCommand cmd = new();
                    MSModel admin = new MSModel();
                    
                    sql = "SELECT STAFF_LOGIN_ID , STAFF_ID , STAFF_LOGIN_PW " +
                          "FROM MEDI_STAFF_LOGIN msl " +
                          "WHERE msl.STAFF_LOGIN_ID ='" + txtBoxUserName.Text +"'";
                    
                    cmd.Connection = conn;
                    cmd.CommandText = sql;

                    OracleDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (read.Read() && !string.IsNullOrEmpty(read.GetString(read.GetOrdinal("STAFF_LOGIN_ID"))))
                    {
                        if ((txtBoxPwd.Password).Equals(read.GetString(read.GetOrdinal("STAFF_LOGIN_PW"))))
                        {
                            MainUnit m = new();
                            m.Show();
                            this.Close();
                        }
                        else {
                            userNameTip.Visibility =  Visibility.Hidden;
                            pwdTip.Visibility = Visibility.Visible;
                            pwdTip.Content = "비밀번호가 틀렸습니다!";
                        }
                    }
                    else {
                        userNameTip.Visibility = Visibility.Visible;
                        userNameTip.Content = "해당 아이디가 없습니다!";
                    }
                }
            }
        }
        
        private void Cancel_Click_Btn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
