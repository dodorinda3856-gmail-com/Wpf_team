using AdminProgram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

/// <summary>
/// 예약 환자의 상세 정보를 볼 수 있는 화면
/// 환자의 예약 정보를 수정할 수 있고 예약 정보를 삭제할 수 있음
/// </summary>
namespace AdminProgram.ViewModels
{
    public partial class ADWViewModel : ObservableRecipient, IRecipient<ObservableCollection<ReservationListModel>>, IRecipient<ReservationListModel>
    {
        private readonly ILogger _logger;

        private ObservableCollection<ReservationListModel> details;
        public ObservableCollection<ReservationListModel> Details
        {
            get => details;
            set => SetProperty(ref details, value);
        }

        public ADWViewModel(ILogger<ADWViewModel> logger)
        {
            _logger = logger;
            _logger.LogWarning("{@ADWViewModel}", logger);
            this.IsActive = true; //이거 필수
        }

        public void Receive(ObservableCollection<ReservationListModel> message) //이거 필수
        {
            Details = new ObservableCollection<ReservationListModel>();
            _logger.LogInformation("{@rModels}", message);

            foreach (var item in message)
            {
                Details.Add(item);
            }
        }

        public void Receive(ReservationListModel message)
        {
            Console.WriteLine("");
        }
    }

}
