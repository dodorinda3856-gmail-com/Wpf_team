using AdminProgram.Models;
using AdminProgram.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

/**
 * AddReservationWindow.xaml과 AddWaitingWindow.xaml과 관련된 ViewModel
 * 
 * 1) 환자를 주민등록번호로 검색
 * 2) 요일에 맞는 시간 테이블 가져오기
 * 3) 의료진 정보 가져오기
 * 4) 진료 예약 등록
 * 5) 방문 대기자 등록
 */
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
        private ObservableCollection<TimeModel> timeModel;
        public ObservableCollection<TimeModel> TimeModels
        {
            get { return timeModel; }
            set { SetProperty(ref timeModel, value); }
        }

        //진료진 정보
        private ObservableCollection<MediStaffModel> staffModel;
        public ObservableCollection<MediStaffModel> StaffModels
        {
            get { return staffModel; }
            set { SetProperty(ref staffModel, value); }
        }

        public AddWaitingReservationVM(ILogger<AddWaitingReservationVM> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            PModels = new ObservableCollection<PatientModelTemp>();
            PModels.CollectionChanged += ContentCollectionChanged;

            TimeModels = new ObservableCollection<TimeModel>();
            TimeModels.CollectionChanged += ContentCollectionChanged;

            StaffModels = new ObservableCollection<MediStaffModel>();
            StaffModels.CollectionChanged += ContentCollectionChanged;
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
                WeakReferenceMessenger.Default.Send(TimeModels);
                _logger.LogInformation("send 성공");
            }

            var mediStaffModels = sender as PatientModelTemp;
            if (mediStaffModels != null)
            {
                _logger.LogInformation("{@mediStaffModels}", mediStaffModels);
                WeakReferenceMessenger.Default.Send(StaffModels);
                _logger.LogInformation("send 성공");
            }
        }
        //== Messenger ==//


        //== (진료 예약 등록)환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 start ==//
        private void SearchPatientR()
        {
            string getDate = MakeDate(SelectedDateTime);
            string nowDate = MakeDate(DateTime.Now);

            if (getDate == nowDate)
            {
                MessageBox.Show("날짜를 선택해서 검색해주세요");
            }
            else
            {
                string sql;
                string patientName = searchText; //환자 이름
                if (patientName != "") //이름 입력하면
                {
                    sql =
                        "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.ADDRESS, p.RESIDENT_REGIST_NUM, p.GENDER " +
                        "FROM PATIENT p " +
                        "WHERE p.PATIENT_NAME LIKE '%" + patientName + "%' ";

                    using (OracleConnection conn = new OracleConnection(strCon))
                    {
                        try
                        {
                            conn.Open();
                            _logger.LogInformation("DB Connection OK...");
                            LogRecord.LogWrite("DB Connection OK...");

                            //검색 할 때 마다 데이터가 누적되는 문제 해결을 위함
                            PModels = new ObservableCollection<PatientModelTemp>();
                            PModels.CollectionChanged += ContentCollectionChanged;

                            TimeModels = new ObservableCollection<TimeModel>();
                            TimeModels.CollectionChanged += ContentCollectionChanged;

                            StaffModels = new ObservableCollection<MediStaffModel>();
                            StaffModels.CollectionChanged += ContentCollectionChanged;


                            using (OracleCommand comm = new OracleCommand())
                            {
                                comm.Connection = conn;
                                comm.CommandText = sql;

                                // 1) 환자 검색
                                using (OracleDataReader reader = comm.ExecuteReader())
                                {
                                    _logger.LogInformation("환자 검색 select 실행");
                                    _logger.LogInformation("[SQL QUERY] " + sql);
                                    LogRecord.LogWrite("[검색한 환자 리스트 가져오기 SQL Query] " + sql);
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
                                    {
                                        //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                        //화면에서 보여야 하는 값이 null인 경우에도 발생함
                                        //처음에 데이터를 넣을 때 관련 값들은 null이 없게 하는것도 중요할듯
                                        _logger.LogCritical(e + "");
                                        LogRecord.LogWrite("[검색한 환자 리스트 가져오기 ERROR] " + e);
                                    }
                                    finally
                                    {
                                        _logger.LogInformation("검색한 환자 리스트 가져오기 성공");
                                        LogRecord.LogWrite("[검색한 환자 리스트 가져오기 OK]");
                                        reader.Close();
                                    }
                                }

                                // 2) 요일에 해당하는 <시간 테이블 값> 가져오기
                                //진행중...
                                //예약이 되어있는 값은 보여주면 안됨ㅜㅜ
                                string date = MakeDate(SelectedDateTime);
                                sql =
                                    "SELECT TIME_ID, \"HOUR\", \"DAY\" " +
                                    "FROM \"TIME\" t " +
                                    "WHERE \"DAY\" = TO_NUMBER(TO_CHAR(TO_DATE('" + date + "', 'YYYY/MM/DD'), 'd'))-1 " +
                                    "ORDER BY TIME_ID ";
                                comm.CommandText = sql;

                                using (OracleDataReader reader = comm.ExecuteReader())
                                {
                                    _logger.LogInformation("DatePicker의 값은 " + SelectedDateTime);
                                    _logger.LogInformation("TIME TABLE값 가져오기 select 실행");
                                    _logger.LogInformation("[SQL QUERY] " + sql);
                                    LogRecord.LogWrite("[TIME TABLE 값 가져오기 SQL Query] " + sql);
                                    try
                                    {
                                        while (reader.Read())
                                        {
                                            TimeModels.Add(new TimeModel()
                                            {
                                                TimeId = reader.GetInt32(reader.GetOrdinal("TIME_ID")),
                                                Hour = reader.GetString(reader.GetOrdinal("HOUR")),
                                                Day = reader.GetString(reader.GetOrdinal("Day"))
                                            });
                                        }
                                    }
                                    catch (InvalidCastException e)
                                    {
                                        //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                        _logger.LogCritical(e + "");
                                        LogRecord.LogWrite("[TIME TABLE 가져오기 ERROR] " + e);
                                    }
                                    finally
                                    {
                                        _logger.LogInformation("TIME TABLE 가져오기 성공");
                                        LogRecord.LogWrite("[TIME TABLE 가져오기 OK]");
                                        reader.Close();
                                    }
                                }

                                // 3) 의료진 정보 가져오기
                                sql =
                                    "SELECT STAFF_ID, STAFF_NAME, MEDI_SUBJECT " +
                                    "FROM MEDI_STAFF ms " +
                                    "WHERE \"POSITION\" = 'D' ";
                                comm.CommandText = sql;
                                _logger.LogInformation("[SQL QUERY] " + sql);
                                

                                using (OracleDataReader reader = comm.ExecuteReader())
                                {
                                    _logger.LogInformation("의료진 정보 Table 값 가져오기 select 실행");
                                    LogRecord.LogWrite("[의료진 정보 가져오기 SQL Query] " + sql);

                                    try
                                    {
                                        while (reader.Read())
                                        {
                                            StaffModels.Add(new MediStaffModel()
                                            {
                                                StaffId = reader.GetInt32(reader.GetOrdinal("STAFF_ID")),
                                                StaffName = reader.GetString(reader.GetOrdinal("STAFF_NAME")),
                                                MediSubject = reader.GetString(reader.GetOrdinal("MEDI_SUBJECT"))
                                            });
                                        }
                                    }
                                    catch (InvalidCastException e)
                                    {
                                        //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                        _logger.LogCritical(e + "");
                                        LogRecord.LogWrite("[의료진 정보 가져오기 ERROR] " + e);
                                    }
                                    finally
                                    {
                                        _logger.LogInformation("의료진 정보 Table 가져오기 성공");
                                        LogRecord.LogWrite("[의료진 정보 가져오기 OK]");
                                        reader.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            _logger.LogInformation(err + "");
                            LogRecord.LogWrite("[진료 예약 등록 페이지에 DATA 가져오기 ERROR] " + err);
                        }
                        finally
                        {
                            LogRecord.LogWrite("[진료 예약 등록 페이지에 DATA 가져오기 OK]");
                        }
                    }
                }
                else //이름 입력 안하면
                {
                    MessageBox.Show("환자 이름을 검색해주세요");
                }
            }
        }
        private RelayCommand searchPatientActR;
        public ICommand SearchPatientActR => searchPatientActR ??= new RelayCommand(SearchPatientR);
        //== (진료 예약 등록)환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 end ==//


        //== (대기자 등록)환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 start ==//
        private void SearchPatientW()
        {
            string sql;
            string patientName = searchText; //환자 이름

            if(patientName == "") //null 처리
            {
                MessageBox.Show("환자 이름을 검색해주세요");
            }
            else
            {
                sql =
                    "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.ADDRESS, p.RESIDENT_REGIST_NUM, p.GENDER " +
                    "FROM PATIENT p " +
                    "WHERE p.PATIENT_NAME LIKE '%" + patientName + "%' ";
                //_logger.LogInformation("[의사 진료 코드(?)] " + SelectedDiseaseType);

                using (OracleConnection conn = new OracleConnection(strCon))
                {
                    try
                    {
                        conn.Open();
                        _logger.LogInformation("DB Connection OK...");
                        LogRecord.LogWrite("DB Connection OK...");
                        //LogRecord.LogWrite("[AddWaitingReservationVM] DB Connection OK...");

                        //검색 할 때 마다 데이터가 누적되는 문제 해결을 위함
                        PModels = new ObservableCollection<PatientModelTemp>();
                        PModels.CollectionChanged += ContentCollectionChanged;

                        StaffModels = new ObservableCollection<MediStaffModel>();
                        StaffModels.CollectionChanged += ContentCollectionChanged;

                        using (OracleCommand comm = new OracleCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = sql;

                            // 1) 환자 검색
                            using (OracleDataReader reader = comm.ExecuteReader())
                            {
                                _logger.LogInformation("[환자 검색 SQL QUERY] " + sql);
                                LogRecord.LogWrite("[AddWaitingReservationVM][대기자 등록 페이지 환자 검색 SQL Query] " + sql);
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
                                {
                                    //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                    //화면에서 보여야 하는 값이 null인 경우에도 발생함
                                    //처음에 데이터를 넣을 때 관련 값들은 null이 없게 하는것도 중요할듯
                                    _logger.LogCritical(e + "");
                                    LogRecord.LogWrite("[환자 검색 ERROR] " + e);
                                }
                                finally
                                {
                                    _logger.LogInformation("검색한 환자 리스트 가져오기 성공");
                                    LogRecord.LogWrite("[환자 검색 OK]");
                                    reader.Close();
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        _logger.LogInformation(err + "");
                        LogRecord.LogWrite("[대기자 등록 페이지에 정보 가져오기 ERROR] " + err);
                    }
                    finally
                    {
                        LogRecord.LogWrite("[대기자 등록 페이지에 정보 가져오기 OK]");
                    }
                }
            }

        }
        private RelayCommand searchPatientActW;
        public ICommand SearchPatientActW => searchPatientActW ??= new RelayCommand(SearchPatientW);
        //== (대기자 등록)환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 end ==//


        //==  방문 대기자 등록 start ==//
        private void RegisterWaiting()
        {
            _logger.LogInformation("방문 대기자 등록을 시작합니다.");

            if (ExplainSymtom == null)
            {
                MessageBox.Show("증상을 입력해주세요");
            }
            else
            {
                string sql =
                    "INSERT INTO WAITING (WATING_ID, PATIENT_ID, REQUEST_TO_WAIT, REQUIREMENTS, WAIT_STATUS_VAL) " +
                    "VALUES(WAITING_SEQ.NEXTVAL, " + SelectedPatient.PatientId + ", sysdate + (interval '9' hour), '" + explainSymtom + "', 'T') ";

                using (OracleConnection conn = new OracleConnection(strCon))
                {
                    try
                    {
                        conn.Open();
                        _logger.LogInformation("DB Connection OK...");
                        LogRecord.LogWrite("DB Connection OK...");

                        PModels = new ObservableCollection<PatientModelTemp>();
                        PModels.CollectionChanged += ContentCollectionChanged;

                        using (OracleCommand comm = new OracleCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = sql;
                            _logger.LogInformation("Insert 시작");
                            _logger.LogInformation("[SQL Query] : " + sql);
                            LogRecord.LogWrite("[방문 대기자 등록 SQL Query] " + sql);
                            //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                            comm.ExecuteNonQuery();
                            _logger.LogInformation("Insert 완료");
                        }
                    }
                    catch (Exception err)
                    {
                        _logger.LogInformation(err + "");
                        LogRecord.LogWrite("[방문 대기자 등록 ERROR] " + err);
                    }
                    finally
                    {
                        _logger.LogInformation("이 환자를 대기자 명단에 등록하였습니다...");
                        LogRecord.LogWrite("[방문 대기자 등록 OK]");
                    }
                }

                MessageBox.Show("환자를 대기 명단에 등록하였습니다");
            }

        }
        private RelayCommand registerWaitingData;
        public ICommand RegisterWaitingData => registerWaitingData ??= new RelayCommand(RegisterWaiting);
        //== 방문 대기자 등록 end ==//


        //== 진료 예약 등록 start ==//
        private void RegisterReservation()
        {
            //환자 번호, 진료예약 시간, 
            _logger.LogInformation("진료 예약 등록을 시작합니다...");
            string date = MakeDate(SelectedDateTime);
            //insert
            string sql =
                "INSERT INTO RESERVATION(RESERVATION_ID, PATIENT_ID, TIME_ID, MEDICAL_STAFF_ID, RESERVE_STATUS_VAL, RESERVATION_DATE, SYMPTOM) " +
                "VALUES(RESERVATION_SEQ1.NEXTVAL, " +
                    SelectedPatient.PatientId + ", " +
                    SelectedTime.TimeId + ", " +
                    SelectedStaff.StaffId + ", " +
                    "'T', to_date('" + date + " " + SelectedTime.Hour + "', 'YYYY/MM/DD HH24:MI:SS'), '" + explainSymtom + "') ";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");
                    LogRecord.LogWrite("DB Connection OK...");

                    PModels = new ObservableCollection<PatientModelTemp>();
                    PModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        _logger.LogInformation("[SQL Query] " + sql);
                        LogRecord.LogWrite("[진료 예약 등록 SQL Query] " + sql);
                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception err) 
                { 
                    _logger.LogInformation(err + "");
                    LogRecord.LogWrite("[진료 예약 등록 ERROR] " + err);
                }
                finally
                {
                    _logger.LogInformation("이 예약 정보를 예약자 리스트에 등록했습니다.");
                    LogRecord.LogWrite("[진료 예약 등록 OK]");
                }
            }
        }
        private RelayCommand registerReservationData;
        public ICommand RegisterReservationData => registerReservationData ??= new RelayCommand(RegisterReservation);
        //== 진료 예약 등록 end ==//


        //== 요일에 해당하는 <시간 테이블 값> 가져오기 start ==//
        private void GetTimeTable()
        {
            string date = MakeDate(SelectedDateTime);
            string sql =
                "SELECT TIME_ID, \"HOUR\", \"DAY\" " +
                            "FROM \"TIME\" t " +
                            "WHERE \"DAY\" = TO_NUMBER(TO_CHAR(TO_DATE('" + date + "', 'YYYY/MM/DD'), 'd'))-1 " +
                            "ORDER BY TIME_ID ";
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    TimeModels = new ObservableCollection<TimeModel>();
                    TimeModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("DatePicker의 값은 " + SelectedDateTime);
                            _logger.LogInformation("TIME TABLE값 가져오기 select 실행");
                            _logger.LogInformation("[SQL QUERY] " + sql);

                            try
                            {
                                while (reader.Read())
                                {
                                    TimeModels.Add(new TimeModel()
                                    {
                                        TimeId = reader.GetInt32(reader.GetOrdinal("TIME_ID")),
                                        Hour = reader.GetString(reader.GetOrdinal("HOUR")),
                                        Day = reader.GetString(reader.GetOrdinal("Day"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            {
                                //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
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
        private RelayCommand changeDateTime;
        public ICommand ChangeDateTime => changeDateTime ??= new RelayCommand(GetTimeTable);
        //== 요일에 해당하는 <시간 테이블 값> 가져오기 end ==//


        //== 공통 ==//
        //검색어(주민등록번호)
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        //datagrid에서 선택된 행의 값들을 가짐
        private PatientModelTemp selectedPatient;
        public PatientModelTemp SelectedPatient
        {
            get => selectedPatient;
            set => SetProperty(ref selectedPatient, value);
        }

        //간단한 증상 설명
        private string explainSymtom;
        public string ExplainSymtom
        {
            get => explainSymtom;
            set => SetProperty(ref explainSymtom, value);
        }

        //combobox에서 선택된 시간값
        private TimeModel selectedTime;
        public TimeModel SelectedTime
        {
            get => selectedTime;
            set => SetProperty(ref selectedTime, value);
        }

        //combobox에서 선택된 진료진값
        private MediStaffModel selectedStaff;
        public MediStaffModel SelectedStaff
        {
            get => selectedStaff;
            set => SetProperty(ref selectedStaff, value);
        }

        //combobox에서 선택된 질병 타입
        private string selectedDiseaseType;
        public string SelectedDiseaseType
        {
            get => selectedDiseaseType;
            set => SetProperty(ref selectedDiseaseType, value);
        }

        //== 날짜 start ==//
        private DateTime selectedDateTime = DateTime.Now;
        public DateTime SelectedDateTime
        {
            get => selectedDateTime;
            set => SetProperty(ref selectedDateTime, value);
        }

        //Month, Day가 1~12까지 가져와서 01, 02 ... 이런식으로 만들기 위함
        private string MakeDate(DateTime date)
        {
            string month;
            string day;

            if (date.Month.ToString().Length == 1)
            {
                month = "0" + date.Month.ToString();
            }
            else
            {
                month = date.Month.ToString();
            }
            if (date.Day.ToString().Length == 1)
            {
                day = "0" + date.Day.ToString();
            }
            else
            {
                day = date.Day.ToString();
            }

            string returnDate = date.Year + "" + month + day;

            return returnDate;
        }
        //== 날짜 end ==//
        //== 공통 ==//
    }
}
