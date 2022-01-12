using AdminProgram.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AdminProgram
{
    /// <summary>
    /// MainUnit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainUnit : Window
    {
        OracleConnection connn;

        public MainUnit()
        {
            InitializeComponent();
            Global_Name.Content = Application.Current.Properties["globalName"] + "님 안녕하세요.";
            LogRecord.LogWrite("------------'" + Application.Current.Properties["globalName"] + "' 로그인------------");
        }

        private void MediAppointment_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/MediAppointmentPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Home_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("StartPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Medi_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/TreatmentPage.xaml", UriKind.RelativeOrAbsolute));
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
        private int Calculate_age(DateTime date)
        {
            int now = 2022;
            string str_tmp = date.ToString("yyyy");
            int age = Convert.ToInt32(str_tmp);
            age = now - age + 1;

            return age;
        }

        private void Patient_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/PatientPage/PatientPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Staff_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/StaffPage/StaffPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Disease_Management_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/DiseaseManagePage/DiseaseManagementPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("로그아웃 하시겠습니까?", "취소", MessageBoxButton.YesNo);
            LogRecord.LogWrite("로그아웃 클릭");
            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
            {
                MainWindow m = new();
                LogRecord.LogWrite("로그아웃 최종확인 클릭");
                m.Show();
                this.Close();
            }
                
        }
    }
}
