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

namespace AdminProgram
{
    /// <summary>
    /// ModifiyPatient.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifiyPatient : Window
    {
        public ModifiyPatient()
        {
            InitializeComponent();
        }

        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                MessageBox.Show("No Date");
            }
            else
            {
                //날짜 가져오는 부분
                MessageBox.Show(date.Value.ToShortDateString());
            }
        }

        private void Modifiy_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("위 내용으로 수정하시겠습니까?", "수정", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("수정되었습니다.");
                Close();
            }
        }

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
