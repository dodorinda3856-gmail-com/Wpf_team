using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// TreatmentPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TreatmentPage : Page
    {
        
        public TreatmentPage()
        {
            InitializeComponent();
        }

        /*row 더블 클릭 시 이벤트 처리*/
        private void Row_DoubleClick(object sender, EventArgs args)
        {
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                NewWindow NewWindow = new NewWindow();
                NewWindow.ShowDialog(); //해당 창을 닫기 전까지는 뒤에 있는 창으로 이동 못함
            }
        }
    }
}
