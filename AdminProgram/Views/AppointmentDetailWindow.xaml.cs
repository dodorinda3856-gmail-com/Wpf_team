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

        private void Delete_Reservation(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("예약 정보를 삭제했습니다");
            Window.GetWindow(this).Close();
        }
    }
}
