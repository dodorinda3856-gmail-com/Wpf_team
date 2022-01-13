using AdminProgram.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
            MediAppointment_Label_grid.Background = Brushes.White;
            MediAppointment_Label.Foreground = Brushes.Black;
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
