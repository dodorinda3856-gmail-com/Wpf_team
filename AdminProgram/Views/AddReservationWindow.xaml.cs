using AdminProgram.Models;
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

            //== 화면 초기화 ==//
            //직접 입력 부분
            searchText.Text = "";
            explainSymtom.Text = "";
            datePicker.SelectedDate = DateTime.Now;
            //세부 데이터 부분
            patientName.Text = "";
            patientResidentRegistNum.Text = "";
            patientGender.Text = "";
            patientAddress.Text = "";
        }
        
        private void Close_Window(object sender, RoutedEventArgs e)
        {
            doctorCombo.SelectedIndex = 0;
            timeCombo.SelectedIndex = 0;
            Window.GetWindow(this).Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("진료 예약을 완료하였습니다.");
            Window.GetWindow(this).Close();
        }
    }
}
