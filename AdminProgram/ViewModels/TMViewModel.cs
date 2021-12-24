using AdminProgram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdminProgram.ViewModels
{
    public class TMViewModel : ObservableRecipient
    {
        //로그
        private readonly ILogger _logger; 
        //sql 연결
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        private ObservableCollection<TMModel> tmModel;
        public ObservableCollection<TMModel> TMModels
        {
            get { return tmModel; }
            set { SetProperty(ref tmModel, value); }
        }

        public TMViewModel(ILogger<TMViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            TMModels = new ObservableCollection<TMModel>();
            TMModels.CollectionChanged += ContentCollectionChanged;
        }

        private void ContentCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= ProductOnPropertyChanged;
                    _logger.LogInformation("진료 리스트 삭제됨");
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                    _logger.LogInformation("진료 리스트 불러옴");
                }
            }
        }

        private void ProductOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var maModel = sender as TMModel;
            if (maModel != null)
            {
                _logger.LogInformation("{@MAModel}", maModel);
                WeakReferenceMessenger.Default.Send(TMModels); //이거 필수
                _logger.LogInformation("send 성공");
            }
        }

        //== SQL : 진료 정보 가져오기 ==//
        private void GetTreatmentData()
        {
            //_logger.LogInformation(""+SearchText.ToString());
            string sql;
            string name = searchText;
            if (name != null)
            {
                sql = "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.PHONE_NUM, t.TREAT_DETAILS " +
                "FROM PATIENT p, TREATMENT t " +
                "WHERE p.PATIENT_ID = t.PATIENT_ID AND p.PATIENT_NAME LIKE '" + searchText + "%'";
            }
            else
            {
                sql = "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.PHONE_NUM, t.TREAT_DETAILS " +
                "FROM PATIENT p, TREATMENT t " +
                "WHERE p.PATIENT_ID = t.PATIENT_ID";
            }

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("select 실행");
                            try
                            {
                                while (reader.Read())
                                {
                                    TMModels.Add(new TMModel() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        PatientNumber = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        PatientPhoneNum = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                                        TreatDetail = reader.GetString(reader.GetOrdinal("TREAT_DETAILS"))
                                    });
                                }
                            }
                            finally
                            {
                                _logger.LogInformation("데이터 읽어오기 성공");
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    _logger.LogInformation(err.ToString());
                }
            }
        }

        private RelayCommand getTreatmentBtn;
        public ICommand GetTreatmentBtn => getTreatmentBtn ??= new RelayCommand(GetTreatmentData);

        private string searchText; //검색어
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }
    }
}
