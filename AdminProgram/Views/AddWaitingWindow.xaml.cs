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
