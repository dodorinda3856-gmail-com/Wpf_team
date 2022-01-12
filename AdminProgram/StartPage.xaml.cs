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
using LiveCharts;
using LiveCharts.Wpf;
using AdminProgram.ViewModels;

namespace AdminProgram
{
    /// <summary>
    /// StartPage.xaml에 대한 상호 작용 논리, choi -> devline
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            SPViewModel sp = new SPViewModel();
            WaitTitle.Content = dt.ToString("yyyy/MMM/dd") + "일 대기자 수 ";
            WaitContent.Text = "" + sp.WaitCount() + "명";

        }
    }
}
