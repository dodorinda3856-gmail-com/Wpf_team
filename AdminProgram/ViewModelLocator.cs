using AdminProgram.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace AdminProgram
{
    // app.xaml.cs 에 등록을 해야 사용할 수 있다.
    // 사용을 원하는 viewmodel을 선언해서 사용 가능
    public class ViewModelLocator
    {
        public MediAppointmentVM MAVM => Ioc.Default.GetService<MediAppointmentVM>();
        public ADWViewModel ADWVM => Ioc.Default.GetService<ADWViewModel>();
        //public PatientVM PMVM => Ioc.Default.GetService<PatientVM>(); // 아직 App.xaml.cs에 등록 안함
        public TreatmentVM TMVM => Ioc.Default.GetService<TreatmentVM>();
        //public AddWaitingReservationVM AWRVM => Ioc.Default.GetService<AddWaitingReservationVM>();
        public SPViewModel SPVM => Ioc.Default.GetService<SPViewModel>();
        public LogVM LVM => Ioc.Default.GetService<LogVM>();
    }
}
