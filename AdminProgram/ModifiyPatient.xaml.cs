using AdminProgram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AdminProgram
{
    /// <summary>
    /// ModifiyPatient.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifiyPatient : Window
    {
        //static으로 다른 클래스의 객체정보 받아올 변수
        private static PMModel Form2_value;
        public static PMModel Passvalue
        {
            get { return Form2_value; }
            set { Form2_value = value; }
        }


        //환자정보 시작과 동시에 초기화함수
        void InitPatient()
        {
            patientName.Text = Passvalue.Patient_Name;
            securityNum.Text = Passvalue.Resident_Regist_Num;
            datePicker.Text = Passvalue.Dob.ToString();
            address.Text = Passvalue.Address;
            phoneNum.Text = Passvalue.Phone_Num;
            if (Passvalue.Gender == "M")
                male.IsChecked = true;
            else
                female.IsChecked = true;
        }

        public ModifiyPatient()
        {
            InitializeComponent();
            InitPatient();
        }


        //달력선택기능
        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                MessageBox.Show("No Date");
            }
        }

        //수정버튼 이벤트
        private void Modifiy_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("위 내용으로 수정하시겠습니까?", "수정", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("수정되었습니다.");
                Close();
            }
        }

        //취소버튼 이벤트
        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("환자정보를 삭제하시겠습니까?", "삭제", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("해당 환자가 삭제되었습니다.");
                Close();
            }
        }
    }
}
