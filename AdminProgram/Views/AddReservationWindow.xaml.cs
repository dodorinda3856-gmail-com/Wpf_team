using AdminProgram.ViewModels;
using System;
using System.Windows;

namespace AdminProgram.Views
{
    /// <summary>
    /// AddReservationWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddReservationWindow : Window
    {
        public AddReservationWindow()
        {
            InitializeComponent();
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("예약 정보를 삭제했습니다");
            Window.GetWindow(this).Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("진료 예약을 완료하였습니다.");
            Window.GetWindow(this).Close();
        }

    }
}
