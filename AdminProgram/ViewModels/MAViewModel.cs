using AdminProgram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

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
        public ObservableCollection<MAModel> MAModel
        {
            get { return maModel; }
            set { SetProperty(ref maModel, value); }
        }

        public MAViewModel(ILogger<MAViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {

        }


    }
}
