using AdminProgram.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace AdminProgram
{
    // app.xaml.cs 에 등록을 해야 사용할 수 있다.
    // 사용을 원하는 viewmodel을 선언해서 사용 가능
    public class ViewModelLocator
    {
        public MAViewModel MAVM => Ioc.Default.GetService<MAViewModel>();
        public ADWViewModel ADWVM => Ioc.Default.GetService<ADWViewModel>();
        public PMViewModel PMVM => Ioc.Default.GetService<PMViewModel>(); // 아직 App.xaml.cs에 등록 안함
        public TMViewModel TMVM => Ioc.Default.GetService<TMViewModel>();
    }
}
