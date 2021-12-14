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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Login_Click_Btn(object sender, RoutedEventArgs e)
        {
            if (txtBoxUserName.Text == "")
            {
                userNameTip.Visibility = Visibility.Visible;
                userNameTip.Content = "아이디를 입력하세요!";
                return;
            }
            else if (txtBoxPwd.Password == "")
            {
                pwdTip.Visibility = Visibility.Visible;
                pwdTip.Content = "비밀번호를 입력하세요!";
                return;
            }
            else if (txtBoxPwd.Password != "admin")
            {

                pwdTip.Visibility = Visibility.Visible;
                pwdTip.Content = "비밀번호가 틀렸습니다!";
            }
            else if (txtBoxUserName.Text != "admin")
            {
                userNameTip.Visibility = Visibility.Visible;
                userNameTip.Content = "해당 아이디가 없습니다!";
            }
            else
            {
                MainUnit m = new();
                m.Show();
                this.Close();
            }
        }
        
        private void Cancel_Click_Btn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
