using AdminProgram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Xaml.Behaviors.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

/**
 * MediAppointment.xaml(진료 예약 관리 페이지)과 관련된 ViewModel
 * 1) 예약한 환자 리스트
 * 2) 방문 대기중인 환자 리스트
 * 3) 진료 완료된 환자 리스트
 */
namespace AdminProgram.ViewModels
{
    public class MediAppointmentVM : ObservableRecipient
    {
        // viewmodel에서는 model의 값/을 가져와서 view에 뿌리기 전에 전처리 하는 곳
        // messenger의 send와 receive도 하는 곳
        // viewmodel은 messenger를 사용하는 통신에 직접적인 영향을 미침

        private readonly ILogger _loger;
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        // rModels을 observable collection 형식(사실상 list)으로 받아옴
        // 1) 예약한 환자 리스트 관련된 Model 사용을 위함
        private ObservableCollection<ReservationListModel> rModels;
        public ObservableCollection<ReservationListModel> RModels
        {
            get { return rModels; }
            set { SetProperty(ref rModels, value); }
        }

        // 2) 방문 대기중인 환자 리스트 Model 사용을 위함
        private ObservableCollection<WaitingListModel> wModel;
        public ObservableCollection<WaitingListModel> WModels
        {
            get { return wModel; }
            set { SetProperty(ref wModel, value); }
        }

        // 4) 진료 완료 리스트 Model 사용을 위함
        private ObservableCollection<TreatmentCompleteListModel> treatmentCompleteModels;
        public ObservableCollection<TreatmentCompleteListModel > TreatmentCompleteModels
        {
            get { return treatmentCompleteModels; }
            set { SetProperty(ref treatmentCompleteModels, value); }
        }

        public MediAppointmentVM(ILogger<MediAppointmentVM> logger)
        {

            _loger = logger;
            _loger.LogInformation("{@ILogger}", logger);

            RModels = new ObservableCollection<ReservationListModel>();
            RModels.CollectionChanged += ContentCollectionChanged;

            WModels = new ObservableCollection<WaitingListModel>();
            WModels.CollectionChanged += ContentCollectionChanged;

            TreatmentCompleteModels = new ObservableCollection<TreatmentCompleteListModel>();
            TreatmentCompleteModels.CollectionChanged += ContentCollectionChanged;

            //페이지에 처음 들어가면 오늘 데이터 바로 보임
            GetReservationPatientList();
        }

        
        //== Messenger 기초 start ==//
        //데이터의 변경을 감지하기 위함
        //데이터 변경이나 데이터 가져오기가 발생하는 Model을 작성해서 처리해야 함...
        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= ProductOnPropertyChanged;
                    ////_loger.LogInformation("리스트 삭제");
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                    ////_loger.LogInformation("리스트 불러옴");
                }
            }
        }

        // 데이터 변화 감지
        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var rModels = sender as ReservationListModel;
            if (rModels != null)
            {
                //_loger.LogInformation("{@rModels}", rModels);
                WeakReferenceMessenger.Default.Send(RModels); //이거 필수
                //_loger.LogInformation("ReservationList send 성공");
            }
            var wModels = sender as WaitingListModel;
            if (wModels != null)
            {
                //_loger.LogInformation("{@wModels}", wModels);
                WeakReferenceMessenger.Default.Send(WModels); //이거 필수
                //_loger.LogInformation("WaitingList send 성공");
            }
            var tcModels = sender as WaitingListModel;
            if (tcModels != null)
            {
                //_loger.LogInformation("{@tcModels}", tcModels);
                WeakReferenceMessenger.Default.Send(TreatmentCompleteModels); //이거 필수
                //_loger.LogInformation("WaitingList send 성공");
            }
        }
        //== Messenger 기초 end ==//


        //== 처음 화면에 보일 DataGrid의 모든 정보 SQL Query start ==//
        public void GetReservationPatientList()
        {
            LogRecord.LogWrite("[MediAppointmentVM - GetReservationPatientList()] [실행]");
            
            //Month, Day가 1~12까지 가져와서 01, 02 ... 이런식으로 만들기 위함
            string month = "";
            string day = "";
            if (SelectedDateTimeM.Month.ToString().Length == 1)
            {
                month = "0" + SelectedDateTimeM.Month.ToString();
            }
            else
            {
                month = SelectedDateTimeM.Month.ToString();
            }
            if (SelectedDateTimeM.Day.ToString().Length == 1)
            {
                day = "0" + SelectedDateTimeM.Day.ToString();
            }
            else
            {
                day = SelectedDateTimeM.Day.ToString();
            }
            string date = SelectedDateTimeM.Year + "" + month + day;

            // 1) 오늘 날짜에 진료 예약을 한 환자의 리스트를 가져옴
            string sql =
                "SELECT p.PATIENT_NAME, r.RESERVATION_DATE, r.SYMPTOM, ms.STAFF_NAME, r.RESERVATION_ID " +
                "FROM RESERVATION r " +
                "JOIN PATIENT p ON r.PATIENT_ID = p.PATIENT_ID " +
                "JOIN MEDI_STAFF ms ON r.MEDICAL_STAFF_ID = ms.STAFF_ID " +
                "WHERE TO_CHAR(r.RESERVATION_DATE, 'YYYYMMDD') = " + date +
                " AND r.RESERVE_STATUS_VAL = 'T'" + 
                " ORDER BY r.RESERVATION_DATE ";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    LogRecord.LogWrite("[MediAppointmentVM] [예약 환자 데이터 읽어오기 SQL QUERY] " + sql);

                    //데이터가 누적되던 문제 해결
                    RModels = new ObservableCollection<ReservationListModel>();
                    RModels.CollectionChanged += ContentCollectionChanged;

                    WModels = new ObservableCollection<WaitingListModel>();
                    WModels.CollectionChanged += ContentCollectionChanged;

                    TreatmentCompleteModels = new ObservableCollection<TreatmentCompleteListModel>();
                    TreatmentCompleteModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    RModels.Add(new ReservationListModel() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        ReservationId = reader.GetInt32(reader.GetOrdinal("RESERVATION_ID")),
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        ReservationDT = reader.GetDateTime(reader.GetOrdinal("RESERVATION_DATE")),
                                        Symptom = reader.GetString(reader.GetOrdinal("SYMPTOM")),
                                        Doctor = reader.GetString(reader.GetOrdinal("STAFF_NAME"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                LogRecord.LogWrite("[MediAppointmentVM] [InvalidCastException] " + e);
                            }
                            finally
                            {
                                LogRecord.LogWrite("[MediAppointmentVM] 예약 환자 데이터 읽어오기 성공");
                                reader.Close();
                            }
                        }

                        // 2) 방문해서 대기 중인 환자 리스트를 가져옴
                        sql =
                            "SELECT w.WATING_ID, w.PATIENT_ID, p.PATIENT_NAME, p.GENDER, p.PHONE_NUM, p.ADDRESS, w.REQUEST_TO_WAIT, w.REQUIREMENTS " +
                            "FROM WAITING w, PATIENT p " +
                            "WHERE w.PATIENT_ID = p.PATIENT_ID " +
                            "AND TO_CHAR(w.REQUEST_TO_WAIT, 'YYYYMMDD') = " + date +
                            " AND w.WAIT_STATUS_VAL = 'T'" +
                            " ORDER BY w.REQUEST_TO_WAIT";
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            LogRecord.LogWrite("[MediAppointmentVM] [방문 대기 리스트 읽어오기 SQL Query] " + sql);

                            try
                            {
                                while (reader.Read())
                                {
                                    WModels.Add(new WaitingListModel()
                                    {
                                        WaitingId = reader.GetInt32(reader.GetOrdinal("WATING_ID")),
                                        PatientId = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        PatientGender = reader.GetString(reader.GetOrdinal("GENDER")),
                                        PatientPhoneNum = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                                        PatientAddress = reader.GetString(reader.GetOrdinal("ADDRESS")),
                                        RequestToWait = reader.GetDateTime(reader.GetOrdinal("REQUEST_TO_WAIT")),
                                        Symptom = reader.GetString(reader.GetOrdinal("REQUIREMENTS"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                LogRecord.LogWrite("[MediAppointmentVM] [InvalidCastException] " + e);
                            }
                            finally
                            {
                                LogRecord.LogWrite("[MediAppointmentVM] 방문 대기 리스트 읽어오기 성공");
                                reader.Close();
                            }
                        }

                        // 3) 진료 완료된 환자 리스트를 가져옴
                        sql =
                            "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.PHONE_NUM, '방문' AS VISIT_TYPE, w.REQUEST_TO_WAIT AS TR_DATETIME " +
                            "FROM WAITING w " +
                            "JOIN PATIENT p ON p.PATIENT_ID = w.PATIENT_ID " +
                                "WHERE TO_CHAR(w.REQUEST_TO_WAIT , 'YYYYMMDD') = " + date + 
                                "AND w.WAIT_STATUS_VAL = 'F' " +
                            "UNION ALL " +
                            "SELECT p.PATIENT_ID, p.PATIENT_NAME, p.PHONE_NUM, '예약' AS VISIT_TYPE, r.RESERVATION_DATE " +
                            "FROM RESERVATION r " +
                            "JOIN PATIENT p ON p.PATIENT_ID = r.PATIENT_ID " +
                                "WHERE TO_CHAR(r.RESERVATION_DATE , 'YYYYMMDD') = " + date + 
                                "AND r.RESERVE_STATUS_VAL = 'F' ";
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            LogRecord.LogWrite("[MediAppointmentVM] [진료 완료 리스트 읽어오기 SQL Query] " + sql);

                            try
                            {
                                while (reader.Read())
                                {
                                    TreatmentCompleteModels.Add(new TreatmentCompleteListModel()
                                    {
                                        PatientId = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        PatientPhoneNum = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                                        VisitType = reader.GetString(reader.GetOrdinal("VISIT_TYPE")),
                                        Time = reader.GetDateTime(reader.GetOrdinal("TR_DATETIME"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                LogRecord.LogWrite("[MediAppointmentVM] [InvalidCastException] " + e);
                            }
                            finally
                            {
                                LogRecord.LogWrite("[MediAppointmentVM] 진료 완료 리스트 읽어오기 성공");
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    LogRecord.LogWrite("[MediAppointmentVM - GetReservationPatientList()] [exception 발생] " + err);
                }
            }
        }
        private RelayCommand reservationUpdateBtn;
        public ICommand ReservationUpdateBtn => reservationUpdateBtn ??= new RelayCommand(GetReservationPatientList);
        //== 처음 화면에 보일 DataGrid의 모든 정보 SQL Query end ==//


        //== 방문해서 대기중인 환자 삭제(대기하다가 탈주함, 또는 대기자 리스트에 올려두고 환자가 오지 않음) start ==//
        private void DeleteWaitingData()
        {
            string sql = "DELETE FROM WAITING w WHERE w.WATING_ID = " + SelectedItem2.WaitingId;

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    //_loger.LogInformation("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        //_loger.LogInformation("[SQL Query] : " + sql);
                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                    }
                }
                catch(Exception err)
                {
                    //_loger.LogCritical(err + "");
                }
                finally
                {
                    //_loger.LogInformation("이 환자를 대기자 리스트에서 삭제했습니다.");
                }
            }
            //동기화
            GetReservationPatientList(); 
        }
        private RelayCommand deleteWaitingDataBtn;
        public ICommand DeleteWaitingDataBtn => deleteWaitingDataBtn ??= new RelayCommand(DeleteWaitingData);
        //== 방문해서 대기중인 환자 삭제(대기하다가 탈주함, 또는 대기자 리스트에 올려두고 환자가 오지 않음) end ==//

        /// <summary>
        /// ////////////////////////////////////////////////////////
        /// </summary>
        //== 예약 환자가 수납을 완료하는 경우 start ==//
        private void FinDiagnosis()
        {
            //후속 처리 쿼리 짜서 넣으면 됨
            //_loger.LogInformation("예약 환자 수납 진행");

            string sql = "UPDATE RESERVATION r SET r.RESERVE_STATUS_VAL = 'F' WHERE r.RESERVATION_ID = " + SelectedItem.ReservationId;

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    //_loger.LogInformation("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        //_loger.LogInformation("[SQL Query] : " + sql);
                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception err)
                {
                    //_loger.LogCritical(err + "");
                }
                finally
                {
                    //_loger.LogInformation("이 환자는 수납을 완료하였습니다.");
                }
            }

            //동기화
            GetReservationPatientList();

        }
        private RelayCommand finDiagnosisBtn;
        public ICommand FinDiagnosisBtn => finDiagnosisBtn ??= new RelayCommand(FinDiagnosis);
        //== 예약 환자가 수납을 완료하는 경우 start ==//


        //== 대기 중이던 환자의 수납이 완료되는(진료가 완료되는) 경우 start ==//
        private void FinDiagnosis2()
        {
            //후속 처리 쿼리 짜서 넣으면 됨
            //_loger.LogInformation("방문 대기 환자 수납 진행");

            string sql = "UPDATE WAITING w SET w.WAIT_STATUS_VAL = 'F' WHERE w.WATING_ID = " + SelectedItem2.WaitingId;

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    //_loger.LogInformation("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        //_loger.LogInformation("[SQL Query] : " + sql);
                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception err)
                {
                    //_loger.LogCritical(err + "");
                }
                finally
                {
                    //_loger.LogInformation("이 환자는 수납을 완료하였습니다.");
                }
            }
            //동기화
            GetReservationPatientList();
        }
        private RelayCommand finDiagnosisBtn2;
        public ICommand FinDiagnosisBtn2 => finDiagnosisBtn2 ??= new RelayCommand(FinDiagnosis2);
        //== 대기 중이던 환자의 수납이 완료되는(진료가 완료되는) 경우 end ==//
        /// <summary>
        /// ////////////////////////////////////////////////////////
        /// </summary>

        //== 예약 정보를 수정하는 경우 start ==//
        private void UpdateReservationData()
        {
            
        }
        private RelayCommand updateReservationDataBtn;
        public ICommand UpdateReservationDataBtn => updateReservationDataBtn ??= new RelayCommand(UpdateReservationData);
        //== 예약 정보를 수정하는 경우 end ==//


        //== 예약 정보를 삭제하는 경우 start ==//
        private void DeleteReservationData()
        {
            //_loger.LogInformation("예약 정보를 리스트에서 삭제합니다.");
            //_loger.LogInformation("SelectedItem(예약 번호) = " + SelectedItem.ReservationId);

            string sql = "DELETE FROM RESERVATION r WHERE r.RESERVATION_ID = " + SelectedItem.ReservationId;

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    //_loger.LogInformation("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        //_loger.LogInformation("[SQL Query] : " + sql);
                        //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception err)
                {
                    //_loger.LogCritical(err + "");
                }
                finally
                {
                    //_loger.LogInformation("이 예약 정보를 예약자 리스트에서 삭제했습니다.");
                }
            }

            //동기화
            GetReservationPatientList();
        }
        private RelayCommand deleteReservationDataBtn;
        public ICommand DeleteReservationDataBtn => deleteReservationDataBtn ??= new RelayCommand(DeleteReservationData);
        //== 예약 정보를 삭제하는 경우 end ==//


        //== 더블 클릭 후 상세 화면에서 클릭한 행의 정보를 보여주기 위한 코드 start ==//
        //예약 환자 리스트 정보 messenger에 활용
        public ICollectionView CollectionView { get; set; }

        private ActionCommand reservationListDoubleClickCommand;
        public ICommand ReservationListDoubleClickCommand => reservationListDoubleClickCommand ??= new ActionCommand(DoubleClick);

        //예약 리스트에서 선택된 값 처리
        private ReservationListModel selectedItem;
        public ReservationListModel SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        private void DoubleClick()
        {
            try
            {//데이터가 없는 부분에 더블 클릭하면 프로그램 중단되는 문제 해결을 위해 try-catch문 사용
                var selected = SelectedItem;
                //_loger.LogInformation("선택된 행의 환자 이름은 " + selected.PatientName + ", 증상은 " + selected.Symptom, "담당 의사는 " + selected.Doctor);
            } 
            catch (NullReferenceException e)
            {
                //_loger.LogCritical(e + "");
            }
        }

        //방문 대기 환자 리스트 정보 messneger에 활용
        public ICollectionView CollectionView2 { get; set; }

        private ActionCommand waitingListDoubleClickCommand;
        public ICommand WaitingListDoubleClickCommand => waitingListDoubleClickCommand ??= new ActionCommand(DoubleClick2);

        //병원 대기자 리스트에서 선택된 값 처리
        private WaitingListModel selectedItem2;
        public WaitingListModel SelectedItem2
        {
            get => selectedItem2;
            set => SetProperty(ref selectedItem2, value);
        }
        private void DoubleClick2()
        {
            try
            {//데이터가 없는 부분에 더블 클릭하면 프로그램 중단되는 문제 해결을 위해 try-catch문 사용
                var selected2 = SelectedItem2;
                //_loger.LogInformation("선택된 행의 환자 이름은 " + selected2.PatientName + ", 증상은 " + selected2.Symptom);
            }
            catch(NullReferenceException e)
            {
                //_loger.LogCritical(e + "");
            }
        }
        //== 더블 클릭 후 상세 화면에서 클릭한 행의 정보를 보여주기 위한 코드 end ==//


        //== Time Table에서 시간 값 선택을 할 수 있도록 하기 위함 start ==//
        //진행중...
        private TimeModel selectedTimeM;
        public TimeModel SelectedTimeM
        {
            get => selectedTimeM;
            set => SetProperty(ref selectedTimeM, value);
        }

        private void GetTimeList()
        {

        }
        private RelayCommand getTime;
        public ICommand GetTime => getTime ??= new RelayCommand(GetTimeList);
        //== Time Table에서 시간 값 선택을 할 수 있도록 하기 위함 end ==//


        //== 날짜 start ==//
        private DateTime selectedDateTimeM = DateTime.Now;
        public DateTime SelectedDateTimeM
        {
            get => selectedDateTimeM;
            set => SetProperty(ref selectedDateTimeM, value);
        }
        //== 날짜 end ==//


        //////////
        ///대기, 예약 등록
        //////////

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

        //== (진료 예약 등록) 환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 start ==//
        private void SearchPatientR()
        {
            string getDate = MakeDate(SelectedDateTime);
            string nowDate = MakeDate(DateTime.Now);

            if (getDate == nowDate)
            { //당일 예약 불가 + 날짜별 시간 테이블 값 가져오기 위해 초기 작업
                MessageBox.Show("날짜를 선택해서 검색해주세요");
            }
            else
            {
                string sql;
                string patientName = searchText; //환자 이름
                
                if (patientName == "'")
                {
                    MessageBox.Show("다시 입력해주세요");

                }
                else if (patientName != "") //이름 입력하면
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
                                    LogRecord.LogWrite("[검색한 환자 리스트 가져오기 SQL Query] " + sql);
                                    //Regex.Replace 
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
                                    { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                        LogRecord.LogWrite("[검색한 환자 리스트 가져오기 ERROR] " + e);
                                    }
                                    finally
                                    {
                                        LogRecord.LogWrite("[검색한 환자 리스트 가져오기 OK]");
                                        reader.Close();
                                    }
                                }

                                // 2) 요일에 해당하는 <시간 테이블 값> 가져오기
                                string date = MakeDate(SelectedDateTime);
                                sql =
                                    "SELECT TIME_ID, \"HOUR\", \"DAY\" " +
                                    "FROM \"TIME\" t " +
                                    "WHERE \"DAY\" = TO_NUMBER(TO_CHAR(TO_DATE('" + date + "', 'YYYY/MM/DD'), 'd'))-1 " +
                                    "AND \"HOUR\" NOT IN (SELECT t.\"HOUR\" FROM RESERVATION r, \"TIME\" t WHERE r.TIME_ID = t.TIME_ID AND TO_CHAR(r.RESERVATION_DATE , 'YYYYMMDD') = " + date + ") " +
                                    "ORDER BY TIME_ID ";
                                comm.CommandText = sql;

                                using (OracleDataReader reader = comm.ExecuteReader())
                                {
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
                                    { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                        LogRecord.LogWrite("[TIME TABLE 가져오기 ERROR] " + e);
                                    }
                                    finally
                                    {
                                        LogRecord.LogWrite("[TIME TABLE 가져오기 OK]");
                                        reader.Close();
                                    }
                                }

                                // 3) 의료진 정보 가져오기
                                sql =
                                    "SELECT STAFF_ID, STAFF_NAME, MEDI_SUBJECT " +
                                    "FROM MEDI_STAFF ms " +
                                    "WHERE \"POSITION\" = 'D' " +
                                    "ORDER BY STAFF_ID DESC";
                                comm.CommandText = sql;


                                using (OracleDataReader reader = comm.ExecuteReader())
                                {
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
                                    { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                        LogRecord.LogWrite("[의료진 정보 가져오기 ERROR] " + e);
                                    }
                                    finally
                                    {
                                        LogRecord.LogWrite("[의료진 정보 가져오기 OK]");
                                        reader.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception err)
                        {
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
        //== (진료 예약 등록) 환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 end ==//


        //== (대기자 등록) 환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 start ==//
        private void SearchPatientW()
        {
            string sql;
            string patientName = searchText; //환자 이름

            if (patientName == "'") //작은 따옴표 입력하면(에러 처리)
            {
                MessageBox.Show("다시 입력해주세요");
            }
            else if (patientName != "") //이름 입력하면
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
                        LogRecord.LogWrite("DB Connection OK...");

                        //검색 할 때 마다 데이터가 누적되는 문제 해결을 위함
                        PModels = new ObservableCollection<PatientModelTemp>();
                        PModels.CollectionChanged += ContentCollectionChanged;

                        using (OracleCommand comm = new OracleCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = sql;

                            // 1) 환자 검색
                            using (OracleDataReader reader = comm.ExecuteReader())
                            {
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
                                { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                    LogRecord.LogWrite("[환자 검색 ERROR] " + e);
                                }
                                finally
                                {
                                    LogRecord.LogWrite("[환자 검색 OK]");
                                    reader.Close();
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        LogRecord.LogWrite("[대기자 등록 페이지에 정보 가져오기 ERROR] " + err);
                    }
                    finally
                    {
                        LogRecord.LogWrite("[대기자 등록 페이지에 정보 가져오기 OK]");
                    }
                }
            }
            else
            {
                MessageBox.Show("환자 이름을 검색해주세요");
            }
        }
        private RelayCommand searchPatientActW;
        public ICommand SearchPatientActW => searchPatientActW ??= new RelayCommand(SearchPatientW);
        //== (대기자 등록)환자 정보 주민등록번호로 검색, TIME TABLE, 진료진 정보 가져오기 end ==//


        //==  방문 대기자 등록 start ==//
        private void RegisterWaiting()
        {
            if(explainSymtom == null)
            {
                _loger.LogInformation("explainSysmtom = " + explainSymtom);
                explainSymtom = "딱히 없음";
            }
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
                        LogRecord.LogWrite("DB Connection OK...");

                        PModels = new ObservableCollection<PatientModelTemp>();
                        PModels.CollectionChanged += ContentCollectionChanged;

                        using (OracleCommand comm = new OracleCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = sql;
                            LogRecord.LogWrite("[방문 대기자 등록 SQL Query] " + sql);
                            //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                            comm.ExecuteNonQuery();
                        }
                    }
                    catch (Exception err)
                    {
                        _loger.LogCritical("[error] " + err);
                        LogRecord.LogWrite("[방문 대기자 등록 ERROR] " + err);
                    }
                    finally
                    {
                        LogRecord.LogWrite("[방문 대기자 등록 OK]");
                    }
                }
                MessageBox.Show("환자를 대기 명단에 등록하였습니다");

                //동기화
                GetReservationPatientList();
            }

        }
        private RelayCommand registerWaitingData;
        public ICommand RegisterWaitingData => registerWaitingData ??= new RelayCommand(RegisterWaiting);
        //== 방문 대기자 등록 end ==//


        //== 진료 예약 등록 start ==//
        private void RegisterReservation()
        {
            if(explainSymtom == null)
            {
                _loger.LogInformation("explainSysmtom = " + explainSymtom);
                explainSymtom = "딱히 없음";
            }
            if(ExplainSymtom == null)
            {
                MessageBox.Show("증상을 입력해주세요");
            }
            else
            {
                //환자 번호, 진료예약 시간, 
                string date = MakeDate(SelectedDateTime);
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
                        LogRecord.LogWrite("DB Connection OK...");

                        PModels = new ObservableCollection<PatientModelTemp>();
                        PModels.CollectionChanged += ContentCollectionChanged;

                        using (OracleCommand comm = new OracleCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = sql;

                            LogRecord.LogWrite("[진료 예약 등록 SQL Query] " + sql);
                            //ExecuteNonQuery() : INSERT, UPDATE, DELETE 문장 실행시 사용
                            comm.ExecuteNonQuery();
                        }
                    }
                    catch (Exception err)
                    {
                        LogRecord.LogWrite("[진료 예약 등록 ERROR] " + err);
                    }
                    finally
                    {
                        LogRecord.LogWrite("[진료 예약 등록 OK]");
                    }
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
                    //_logger.LogInformation("DB Connection OK...");

                    TimeModels = new ObservableCollection<TimeModel>();
                    TimeModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            //_logger.LogInformation("DatePicker의 값은 " + SelectedDateTime);
                            //_logger.LogInformation("TIME TABLE값 가져오기 select 실행");
                            //_logger.LogInformation("[SQL QUERY] " + sql);

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
                                //_logger.LogCritical(e + "");
                            }
                            finally
                            {
                                //_logger.LogInformation("TIME TABLE 가져오기 성공");
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    //_logger.LogInformation(err + "");
                }
            }
        }
        private RelayCommand changeDateTime;
        public ICommand ChangeDateTime => changeDateTime ??= new RelayCommand(GetTimeTable);
        //== 요일에 해당하는 <시간 테이블 값> 가져오기 end ==//


        //== 공통 ==//
        //검색어(환자 이름)
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
