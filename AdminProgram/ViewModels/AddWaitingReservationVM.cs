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
    public class AddWaitingReservationVM : ObservableRecipient
    {
        private readonly ILogger _logger;
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        //== Messenger ==//
        //환자 정보
        private ObservableCollection<PatientModelTemp> pModel;
        public ObservableCollection<PatientModelTemp> PModels
        {
            get { return pModel; }
            set { SetProperty(ref pModel, value); }
        }

        //예약 시간 정보
        private ObservableCollection<TimeModel> tModel;
        public ObservableCollection<TimeModel> TModels
        {
            get { return tModel; }
            set { SetProperty(ref tModel, value); }
        }

        public AddWaitingReservationVM(ILogger<AddWaitingReservationVM> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            PModels = new ObservableCollection<PatientModelTemp>();
            PModels.CollectionChanged += ContentCollectionChanged;

            TModels = new ObservableCollection<TimeModel>();
            TModels.CollectionChanged += ContentCollectionChanged;
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= ProductOnPropertyChanged;
                    _logger.LogInformation("리스트 삭제");
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                    _logger.LogInformation("리스트 불러옴");
                }
            }
        }

        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var pModels = sender as PatientModelTemp;
            if (pModels != null)
            {
                _logger.LogInformation("{@pModels}", pModels);
                WeakReferenceMessenger.Default.Send(PModels); //이거 필수
                _logger.LogInformation("send 성공");
            }

            var tModels = sender as PatientModelTemp;
            if (tModels != null)
            {
                _logger.LogInformation("{@tModels}", tModels);
                WeakReferenceMessenger.Default.Send(TModels);
                _logger.LogInformation("send 성공");
            }
        }
        //== Messenger ==//

        //== 환자 정보 주민등록번호로 검색 start ==//
        private void SearchPatient()
        {
            string sql;
            string residentRegistNum = searchText; //주민등록번호
            if (residentRegistNum != null)
            {
                sql =
                    "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.ADDRESS, p.RESIDENT_REGIST_NUM,p.GENDER " +
                    "FROM PATIENT p " +
                    "WHERE p.RESIDENT_REGIST_NUM LIKE '" + residentRegistNum + "%' ";
            }
            else
            {
                sql =
                    "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.ADDRESS, p.RESIDENT_REGIST_NUM,p.GENDER " +
                    "FROM PATIENT p ";
            }

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    PModels = new ObservableCollection<PatientModelTemp>();
                    PModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("환자 검색 select 실행");
                            _logger.LogInformation("[SQL QUERY] " + sql);
                            try
                            {
                                while (reader.Read())
                                {
                                    PModels.Add(new PatientModelTemp() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        PatientId = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        Name = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        ResidentRegistNum = reader.GetString(reader.GetOrdinal("RESIDENT_REGIST_NUM")),
                                        Gender = reader.GetString(reader.GetOrdinal("GENDER")),
                                        Address = reader.GetString(reader.GetOrdinal("ADDRESS"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            {//System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                //화면에서 보여야 하는 값이 null인 경우에도 발생함
                                //처음에 데이터를 넣을 때 관련 값들은 null이 없게 하는것도 중요할듯
                                _logger.LogCritical(e + "");
                            }
                            finally
                            {
                                _logger.LogInformation("검색한 환자 리스트 가져오기 성공");
                                reader.Close();
                            }
                        }

                        //시간 테이블 값 가져오기
                        sql = "SELECT TIME_ID, \"HOUR\", \"DAY\" FROM \"TIME\" t ORDER BY TIME_ID ";
                        comm.CommandText = sql;
                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("TIME TABLE값 가져오기 select 실행");
                            _logger.LogInformation("[SQL QUERY] " + sql);

                            try
                            {
                                while (reader.Read())
                                {
                                    TModels.Add(new TimeModel()
                                    {
                                        TimeId = reader.GetInt32(reader.GetOrdinal("TIME_ID")),
                                        Hour = reader.GetString(reader.GetOrdinal("HOUR")),
                                        Day = reader.GetString(reader.GetOrdinal("Day"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            {//System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                _logger.LogCritical(e + "");
                            }
                            finally
                            {
                                _logger.LogInformation("TIME TABLE 가져오기 성공");
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    _logger.LogInformation(err + "");
                }
            }
        }
        private RelayCommand searchPatientBtn;
        public ICommand SearchPatientBtn => searchPatientBtn ??= new RelayCommand(SearchPatient);
        //== 환자 정보 주민등록번호로 검색 end ==//

        //== 대기자 등록 start ==//
        private void RegisterWaiting()
        {
            //시간값, 환자 번호, 간단한 요구사항이 필요하구려
            _logger.LogInformation("방문 대기자 등록 함수에 들어왔습니다... 개발 진행중입니다...");
            string sql = 
                "INSERT INTO WAITING (WATING_ID, PATIENT_ID, REQUEST_TO_WAIT, REQUIREMENTS, WAIT_STATUS_VAL) " +
                "VALUES(WAITING_SEQ.NEXTVAL, " + SelectedPatient.PatientId + ", sysdate + (interval '9' hour), '" + explainSymtom + "', 'F') ";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    PModels = new ObservableCollection<PatientModelTemp>();
                    PModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;
                        _logger.LogInformation("Insert 시작");
                        _logger.LogInformation("[SQL Query] : " + sql);
                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                        _logger.LogInformation("Insert 완료");
                    }
                }
                catch (Exception err)
                {
                    _logger.LogInformation(err + "");
                }
                finally
                {
                    _logger.LogInformation("이 환자를 대기자 명단에 등록하였습니다...");
                }
            }

        }
        private RelayCommand registerWaitingData;
        public ICommand RegisterWaitingData => registerWaitingData ??= new RelayCommand(RegisterWaiting);
        //== 대기자 등록 end ==//

        //== 진료 예약 등록 start ==//
        private void RegisterReservation()
        {
            //시간값이 필요하구려
            _logger.LogInformation("진료 예약 등록 함수에 들어왔습니다... 개발 진행중입니다...");
            string sql = ""; //insert

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    PModels = new ObservableCollection<PatientModelTemp>();
                    PModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception err)
                {
                    _logger.LogInformation(err + "");
                }
                finally
                {
                    _logger.LogInformation("이 예약 정보를 예약자 리스트에서 삭제했습니다.");
                }
            }
        }
        private RelayCommand registerReservationData;
        public ICommand RegisterReservationData => registerReservationData ??= new RelayCommand(RegisterReservation);
        //== 진료 예약 등록 end ==//

        //== 공통 ==//
        private string searchText; //검색어(주민등록번호)
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        private PatientModelTemp selectedPatient; //datagrid에서 선택된 행의 값들을 가짐
        public PatientModelTemp SelectedPatient
        {
            get => selectedPatient;
            set => SetProperty(ref selectedPatient, value);
        }

        private string explainSymtom; //간단한 증상 설명
        public string ExplainSymtom
        {
            get => explainSymtom;
            set => SetProperty(ref explainSymtom, value);
        }

        private PatientModelTemp selectedTime; //combobox에서 선택된 시간값
        public PatientModelTemp SelectedTime
        {
            get => selectedTime;
            set => SetProperty(ref selectedTime, value);
        }
    }
}
