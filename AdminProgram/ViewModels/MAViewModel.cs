using AdminProgram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace AdminProgram.ViewModels
{
    public class MAViewModel : ObservableRecipient
    {
        // viewmodel에서는 model의 값을 가져와서 view에 뿌리기 전에 전처리 하는 곳
        // messenger의 send와 receive도 하는 곳
        // viewmodel은 messenger를 사용하는 통신에 직접적인 영향을 미침

        private readonly ILogger _logger;

        // MAModel을 observable collection 형식(사실상 list)으로 받아옴
        private ObservableCollection<MAModel> maModel;
        public ObservableCollection<MAModel> MAModels
        {
            get { return maModel; }
            set { SetProperty(ref maModel, value); }
        }

        public MAViewModel(ILogger<MAViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            MAModels = new ObservableCollection<MAModel>();
            MAModels.CollectionChanged += ContentCollectionChanged;
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= ProductOnPropertyChanged;
                    _logger.LogInformation("진료 예약 삭제");
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                    _logger.LogInformation("진료 예약 등록");
                }
            }
        }

        //데이터 변화 감지
        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var maModel = sender as MAModel;
            if (maModel != null)
            {
                _logger.LogInformation("{@MAModel}", maModel);
                WeakReferenceMessenger.Default.Send(MAModels);
                _logger.LogInformation("send 성공");
            }
        }
    }
}
