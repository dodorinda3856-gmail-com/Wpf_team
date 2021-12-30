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

        //==예약 등록 윈도우로 이동==//
        private void AddAppointmentBtn(object sender, RoutedEventArgs e)
        {
            AddAppointment aa = new AddAppointment();
            aa.Title = "진료 예약 등록";
            aa.ShowDialog();
        }

        //==예약 수정, 삭제 윈도우로 이동==//
        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                AppointmentDetailWindow ad = new AppointmentDetailWindow();
                ad.Title = "진료 예약 환자 상세정보";
                ad.ShowDialog();
            }
        }

        private void Waiting_Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                WaitingListDetailWindow ad = new WaitingListDetailWindow();
                ad.Title = "방문 대기 환자 상세정보";
                ad.ShowDialog();
            }
        }

        private void FinishedBtn(object sender, RoutedEventArgs e)
        {

        }
    }
}
