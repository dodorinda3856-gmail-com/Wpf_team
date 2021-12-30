using System;
using System.Windows;
using System.Windows.Controls;

namespace AdminProgram.Views
{
    /// <summary>
    /// MediAppointmentPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MediAppointmentPage : Page
    {
        public MediAppointmentPage()
        {
            InitializeComponent();
        }

        //== 예약 등록 윈도우로 이동 ==//
        private void Add_Appointment_Btn(object sender, RoutedEventArgs e)
        {
            AddReservationWindow aa = new AddReservationWindow();
            aa.ShowDialog();
        }

        //== 대기자 등록 윈도우로 이동 ==//
        private void Add_Waiting_Btn(object sender, RoutedEventArgs e)
        {
            AddWaitingWindow aw = new AddWaitingWindow();
            aw.ShowDialog();
        }

        //== 예약 상세정보 윈도우로 이동 ==//
        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                AppointmentDetailWindow ad = new AppointmentDetailWindow();
                ad.Title = "예약 환자 상세정보";
                ad.ShowDialog();
            }
        }

        //== 대기자 상세정보 윈도우로 이동 ==//
        private void Waiting_Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                WaitingListDetailWindow ad = new WaitingListDetailWindow();
                ad.Title = "대기자 상세정보";
                ad.ShowDialog();
            }
        }

        private void FinishedBtn(object sender, RoutedEventArgs e)
        {

        }
    }
}
