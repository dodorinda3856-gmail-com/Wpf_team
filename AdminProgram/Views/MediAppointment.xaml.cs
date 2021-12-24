using AdminProgram.ViewModels;
using AdminProgram.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AdminProgram
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

        //혹시나 날짜 값이 필요할 경우 이거를 활용해서 할 것
        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if(date == null)
            {
                MessageBox.Show("No Date");
            }
            else
            {//날짜 가져오는 부분
                MessageBox.Show(date.Value.ToShortDateString());
            }
        }
        
        //==예약 환자 리스트 업데이트(임시)==//
        /*private void ReservationUpdateBtn(object sender, RoutedEventArgs e)
        {
 
        }*/

        //==예약 등록 윈도우로 이동==//
        private void addAppointmentBtn(object sender, RoutedEventArgs e)
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
        
    }
}
