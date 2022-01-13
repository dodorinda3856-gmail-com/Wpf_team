using System.Windows;

namespace AdminProgram.Views
{
    /// <summary>
    /// AppointmentDetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AppointmentDetailWindow : Window
    {
        public AppointmentDetailWindow()
        {
            InitializeComponent();
        }

        private void Delete_Reservation_Date(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("예약 정보를 삭제했습니다.");
            Window.GetWindow(this).Close();
        }

        private void Fin_Treatment(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("진료을 완료하였습니다.");
            Window.GetWindow(this).Close();
        }

        private void Fin_Payment(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("수납를 완료하였습니다.");
            Window.GetWindow(this).Close();
        }
    }
}
