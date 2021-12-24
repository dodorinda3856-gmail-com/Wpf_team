using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.ViewModels
{
    public class PMViewModel : ObservableRecipient
    {
        private readonly ILogger _logger;
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";


        private ObservableCollection<PMModel> pMModel;
        private ObservableCollection<PMModel> pMModels
        {
            get { return pMModel; }
            set { SetProperty(ref pMModel, value); }
        }

        public PMViewModel(ILogger<PMViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            pMModels = new ObservableCollection<PMModel>();
            pMModels.CollectionChanged += ContentCollectionChanged;
        }


    }
}
