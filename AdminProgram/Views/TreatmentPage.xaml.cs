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

        private void AddDiseaseFilterBtn(object sender, RoutedEventArgs e)
        {
            AddDiseaseFilter ad = new AddDiseaseFilter();
            ad.ShowDialog();
        }
        //== 환자 찾기 페이지로 이동 ==//
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
