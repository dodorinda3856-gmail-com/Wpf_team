using AdminProgram.Views;
using System;
using System.Windows;
using System.Windows.Controls;

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

        //==Data Grid Row 더블 클릭 시 이벤트 처리==//
        private void Row_DoubleClick(object sender, EventArgs args)
        {
            //선택된 환자의 진료 상세정보 가져오기 - 진행 중
            var row = sender as DataGridRow;
            if (row != null && row.IsSelected)
            {
                TreatmentDetailWindow tw = new TreatmentDetailWindow();
                tw.ShowDialog();
            }
        }

 

        private void AddDiseaseFilterBtn(object sender, RoutedEventArgs e)
        {
           
            AddDiseaseFilter ad = new AddDiseaseFilter();

            ad.ShowDialog();
        }
        private void AddPatientFilterBtn(object sender, RoutedEventArgs e)
        {
            AddPatientFilter pf = new AddPatientFilter();
            pf.ShowDialog();
        }

        private void AddProcedureFilterBtn(object sender, RoutedEventArgs e)
        {
         
            AddProcedureFilter ap = new AddProcedureFilter();

            ap.ShowDialog();
        }
    }
}
