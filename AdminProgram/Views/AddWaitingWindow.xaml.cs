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
    }
}
