using AdminProgram.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Serilog;
using System;
using System.Windows;

namespace AdminProgram
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Ioc.Default.ConfigureServices
                (new ServiceCollection()
                    .AddSingleton<MediAppointmentVM>()
                    .AddSingleton<TreatmentVM>()
                    .AddSingleton<AddWaitingReservationVM>()
                    .AddSingleton<SPViewModel>()
                    .AddLogging(builder =>
                    {
                        var logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();

                        builder.AddSerilog(logger);
                    })
                    .BuildServiceProvider()
                );
            this.InitializeComponent();
        }

        //== 요 아래의 사용성은 잘 모르겠으나 참고한 자료에는 있었음... ==//
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // ViewModel, View 등록 {
            services.AddSingleton<MediAppointmentVM>();
            //services.AddSingleton<MediAppointmentPage>();
            /*services.AddTransient<Page1>();//page1을 여러 viewmodel이 봐도 하나의 view만 보는 기능이 됨
            services.AddTransient<Page2>();*/

            // 어플리케이션 Context 등록
            //services.AddSingleton<IAppContext>(new AppContext());

            // 자원 서비스 등록
            //services.AddSingleton<IResourceService>(new ResourceService());

            return services.BuildServiceProvider();
        }
    }
}
