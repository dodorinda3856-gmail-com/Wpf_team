using System.Windows;

namespace AdminProgram.Views
{
    /// <summary>
    /// WaitingListDetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaitingListDetailWindow : Window
    {
        public WaitingListDetailWindow()
        {
            InitializeComponent();
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("대기 환자를 삭제했습니다");
            Window.GetWindow(this).Close();
        }
    }
}
