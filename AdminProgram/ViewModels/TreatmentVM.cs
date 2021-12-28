using AdminProgram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace AdminProgram.ViewModels
{
    public class TreatmentVM : ObservableRecipient
    {
        private readonly ILogger _logger; // 로그   
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        //진료 정보 관련 Model 사용을 위함
        private ObservableCollection<TreatmentModel> tmModel;
        public ObservableCollection<TreatmentModel> TMModels
        {
            get { return tmModel; }
            set { SetProperty(ref tmModel, value); }
        }

        public TreatmentVM(ILogger<TreatmentVM> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            TMModels = new ObservableCollection<TreatmentModel>();
            TMModels.CollectionChanged += ContentCollectionChanged;
        }

        //== Messenger 사용 start ==//
        //== 데이터의 변경을 감지함 ==//
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
            //Model 값의 변경을 감지
            var rModels = sender as TreatmentModel;
            if (rModels != null)
            {
                _logger.LogInformation("{@rModels}", rModels);
                WeakReferenceMessenger.Default.Send(TMModels); //이거 필수
                _logger.LogInformation("send 성공");
            }
        }
        //== Messenger 사용 end ==//

        //== SQL : 진료 정보 가져오기 ==//
        //검색어를 기준으로 진료 정보 가져옴
        //검색어가 없으면 그냥 전체 진료 정보를 가져옴
        private void GetTreatmentData()
        {
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

                    //DataGrid 사용 시 이전에 검색했던(조회했던) 내용이 없어지지 않고
                    //계속 남아있는 문제점 해결을 위해 추가
                    TMModels = new ObservableCollection<TreatmentModel>();
                    TMModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("GetTreatmentData() : select 실행");
                            try
                            {
                                while (reader.Read())
                                {
                                    TMModels.Add(new TreatmentModel()
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
                                _logger.LogInformation("진료 데이터 읽어오기 성공");
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    _logger.LogInformation(err+"");
                }
            }
        }
        private RelayCommand getTreatmentBtn;
        public ICommand GetTreatmentBtn => getTreatmentBtn ??= new RelayCommand(GetTreatmentData);
        
        //검색어
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        private TreatmentModel selectedPatient;
        public TreatmentModel SelectedPatient
        {
            get => selectedPatient;
            set => SetProperty(ref selectedPatient, value);
        }

        //== 진료 내용 저장 start ==//
        private void SaveTreatmentData()
        {
            _logger.LogInformation("진료 내용을 저장합니다.");
        }
        private RelayCommand saveTreatmentDataBtn;
        public ICommand SaveTreatmentDataBtn => saveTreatmentDataBtn ??= new RelayCommand(SaveTreatmentData);
        //== 진료 내용 저장 end ==//
    }
}
