using AdminProgram.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace AdminProgram
{
    // app.xaml.cs 에 등록을 해야 사용할 수 있다.
    //사용을 원하는 viewmodel을 선언해서 사용 가능
    //MainWindow.xaml과 Page1.xaml이 똑같이 PMV를 보고 있음
    public class ViewModelLocator
    {
        //MediAppointment.xaml의 ViewModel
        public MAViewModel MAVM => Ioc.Default.GetService<MAViewModel>();
        public ADWViewModel ADWVM => Ioc.Default.GetService<ADWViewModel>();

    }
}
