using System.Windows;

namespace AdminProgram.Views
{
    /// <summary>
    /// AddWaitingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddWaitingWindow : Window
    {
        public AddWaitingWindow()
        {
            InitializeComponent();
            //== 화면 초기화 ==//
            //직접 입력 부분
            searchText.Text = "";
            explainSymtom.Text = "";
            //세부 사항 부분
            patientAddress.Text = "";
            patientGender.Text = "";
            patientName.Text = "";
            patientResidentRegistNum.Text = "";
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Add_Waiting_Btn(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("환자를 대기 명단에 등록하였습니다");
            Window.GetWindow(this).Close();
            
        }
    }
}
