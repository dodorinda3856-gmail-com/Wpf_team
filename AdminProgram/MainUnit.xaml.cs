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


        //대기/예약 관리
        private void MediAppointment_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Disease_Management_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Disease_Management.Foreground = Brushes.White;
            Home_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Home_Label.Foreground = Brushes.White;
            Medi_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Medi_Label.Foreground = Brushes.White;
            Patient_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Patient_Label.Foreground = Brushes.White;
            Staff_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Staff_Label.Foreground = Brushes.White;


            MediAppointment_Label_grid.Background = Brushes.White;
            MediAppointment_Label.Foreground = Brushes.Black;
            this.frame.Navigate(new Uri("Views/MediAppointmentPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //홈
        private void Home_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Disease_Management_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Disease_Management.Foreground = Brushes.White;
            MediAppointment_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            MediAppointment_Label.Foreground = Brushes.White;
            Medi_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Medi_Label.Foreground = Brushes.White;
            Patient_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Patient_Label.Foreground = Brushes.White;
            Staff_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Staff_Label.Foreground = Brushes.White;

            Home_Label_grid.Background = Brushes.White;
            Home_Label.Foreground = Brushes.Black;
            this.frame.Navigate(new Uri("StartPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //진료 내역 관리
        private void Medi_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Disease_Management_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Disease_Management.Foreground = Brushes.White;
            MediAppointment_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            MediAppointment_Label.Foreground = Brushes.White;
            Home_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Home_Label.Foreground = Brushes.White;
            Patient_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Patient_Label.Foreground = Brushes.White;
            Staff_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Staff_Label.Foreground = Brushes.White;

            Medi_Label_grid.Background = Brushes.White;
            Medi_Label.Foreground = Brushes.Black;
            this.frame.Navigate(new Uri("Views/TreatmentPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //환자정보 관리
        private void Patient_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Disease_Management_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Disease_Management.Foreground = Brushes.White;
            MediAppointment_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            MediAppointment_Label.Foreground = Brushes.White;
            Home_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Home_Label.Foreground = Brushes.White;
            Medi_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Medi_Label.Foreground = Brushes.White;
            Staff_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Staff_Label.Foreground = Brushes.White;

            Patient_Label_grid.Background = Brushes.White;
            Patient_Label.Foreground = Brushes.Black;
            this.frame.Navigate(new Uri("Views/PatientPage/PatientPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //의료진정보관리
        private void Staff_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Disease_Management_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Disease_Management.Foreground = Brushes.White;
            MediAppointment_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            MediAppointment_Label.Foreground = Brushes.White;
            Home_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Home_Label.Foreground = Brushes.White;
            Medi_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Medi_Label.Foreground = Brushes.White;
            Patient_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Patient_Label.Foreground = Brushes.White;

            Staff_Label_grid.Background = Brushes.White;
            Staff_Label.Foreground = Brushes.Black;
            this.frame.Navigate(new Uri("Views/StaffPage/StaffPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //상병 / 시술관리
        private void Disease_Management_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MediAppointment_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            MediAppointment_Label.Foreground = Brushes.White;
            Home_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Home_Label.Foreground = Brushes.White;
            Medi_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Medi_Label.Foreground = Brushes.White;
            Patient_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Patient_Label.Foreground = Brushes.White;
            Staff_Label_grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 48, 48, 48));
            Staff_Label.Foreground = Brushes.White;

            Disease_Management_grid.Background = Brushes.White;
            Disease_Management.Foreground = Brushes.Black;
            
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
