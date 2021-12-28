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

        private readonly ILogger _logger;
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

        // 3) 시간 table 사용 위함
        private ObservableCollection<TimeModel> timeModel;
        public ObservableCollection<TimeModel> TimeModels
        {
            get { return timeModel; }
            set { SetProperty(ref timeModel, value); }
        }

        public MediAppointmentVM(ILogger<MediAppointmentVM> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            RModels = new ObservableCollection<ReservationListModel>();
            RModels.CollectionChanged += ContentCollectionChanged;

            WModels = new ObservableCollection<WaitingListModel>();
            WModels.CollectionChanged += ContentCollectionChanged;
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
                    _logger.LogInformation("예약 리스트 삭제");
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                    _logger.LogInformation("예약 리스트 등록");
                }
            }
        }

        // 데이터 변화 감지
        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var rModels = sender as ReservationListModel;
            if (rModels != null)
            {
                _logger.LogInformation("{@rModels}", rModels);
                WeakReferenceMessenger.Default.Send(RModels); //이거 필수
                _logger.LogInformation("send 성공");
            }
        }
        //== Messenger 기초 end ==//

        //== 처음 화면에 보일 DataGrid의 모든 정보 SQL Query start ==//
        private void GetReservationPatientList()
        {
            // 1) 진료 예약을 한 환자의 리스트를 가져옴
            string sql =
                "SELECT p.PATIENT_NAME, r.RESERVATION_DATE, r.SYMPTOM, ms.STAFF_NAME, r.TREAT_TYPE  " +
                "FROM RESERVATION r " +
                "JOIN PATIENT p ON r.PATIENT_ID = p.PATIENT_ID " +
                "JOIN MEDI_STAFF ms ON r.MEDICAL_STAFF_ID = ms.STAFF_ID ";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    //데이터가 누적되던 문제 해결
                    RModels = new ObservableCollection<ReservationListModel>();
                    RModels.CollectionChanged += ContentCollectionChanged;

                    /*WModels = new ObservableCollection<WaitingListModel>();
                    WModels.CollectionChanged += ContentCollectionChanged;*/

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("select 실행");
                            _logger.LogInformation("[SQL QUERY] " + sql);
                            try
                            {
                                while (reader.Read())
                                {
                                    RModels.Add(new ReservationListModel() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        ReservationDT = reader.GetDateTime(reader.GetOrdinal("RESERVATION_DATE")),
                                        Symptom = reader.GetString(reader.GetOrdinal("SYMPTOM")),
                                        Doctor = reader.GetString(reader.GetOrdinal("STAFF_NAME")),
                                        TreatType = reader.GetString(reader.GetOrdinal("TREAT_TYPE"))
                                    });
                                }
                            }
                            catch(InvalidCastException e)
                            {//System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                _logger.LogInformation(e + "");
                            }
                            finally
                            {
                                _logger.LogInformation("데이터 읽어오기 성공");
                                reader.Close();
                            }
                        }
                        sql =
                            "SELECT w.PATIENT_ID, p.PATIENT_NAME, p.GENDER, p.PHONE_NUM, p.ADDRESS, w.REQUEST_TO_WAIT, w.REQUIREMENTS " +
                            "FROM WAITING w, PATIENT p " +
                            "WHERE w.PATIENT_ID = p.PATIENT_ID " +
                            "AND TO_CHAR(w.REQUEST_TO_WAIT, 'YYYYMMDD') = TO_CHAR(SYSDATE, 'YYYYMMDD') ORDER BY w.REQUEST_TO_WAIT";
                        comm.CommandText = sql;

                        // 2) 방문해서 대기 중인 환자 리스트를 가져옴
                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("select 실행");
                            _logger.LogInformation("[SQL QUERY] " + sql);

                            try
                            {
                                while (reader.Read())
                                {
                                    WModels.Add(new WaitingListModel()
                                    {
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
                            {//System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                _logger.LogInformation(e + "");
                            }
                            finally
                            {
                                _logger.LogInformation("병원에서 대기 중인 환자 데이터 읽어오기 성공");
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
        private RelayCommand reservationUpdateBtn;
        public ICommand ReservationUpdateBtn => reservationUpdateBtn ??= new RelayCommand(GetReservationPatientList);
        //== 처음 화면에 보일 DataGrid의 모든 정보 SQL Query end ==//

        //== 방문해서 대기중인 환자 삭제(대기하다가 탈주함, 또는 대기자 리스트에 올려두고 환자가 오지 않음) start ==//
        //진행중...
        private void DeleteWaitingData()
        {
            //후속 처리 쿼리 짜서 넣으면 됨
            _logger.LogInformation("이 환자를 대기자 리스트에서 삭제합니다.");
        }
        private RelayCommand deleteWaitingDataBtn;
        public ICommand DeleteWaitingDataBtn => deleteWaitingDataBtn ??= new RelayCommand(DeleteWaitingData);
        //== 방문해서 대기중인 환자 삭제(대기하다가 탈주함, 또는 대기자 리스트에 올려두고 환자가 오지 않음) end ==//

        //== 전화로 예약 취소하는 경우 start ==//
        //진행중...
        private void FinDiagnosis()
        {
            //후속 처리 쿼리 짜서 넣으면 됨
            _logger.LogInformation("수납을 완료하였습니다.");
        }
        private RelayCommand finDiagnosisBtn;
        public ICommand FinDiagnosisBtn => finDiagnosisBtn ??= new RelayCommand(FinDiagnosis);
        //== 전화로 예약 취소하는 경우 end ==//

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
            var selected = SelectedItem;
            _logger.LogInformation("선택된 행의 환자 이름은 " + selected.PatientName + ", 증상은 " + selected.Symptom,
                "담당 의사는 " + selected.Doctor);
        }

        //방문 대기 환자 리스트 정보 messneger에 활용
        public ICollectionView CollectionView2 { get; set; }

        private ActionCommand waitingListDoubleClickCommand;
        public ICommand WaitingListDoubleClickCommand => waitingListDoubleClickCommand ??= new ActionCommand(DoubleClick2);

        //병원 대기자 리스트에서 선택된 값 처리 - 진행중
        private WaitingListModel selectedItem2;
        public WaitingListModel SelectedItem2
        {
            get => selectedItem2;
            set => SetProperty(ref selectedItem2, value);
        }
        private void DoubleClick2()
        {
            var selected2 = SelectedItem2;
            _logger.LogInformation("선택된 행의 환자 이름은 " + selected2.PatientName + ", 증상은 " + selected2.Symptom);
        }
        //== 더블 클릭 후 상세 화면에서 클릭한 행의 정보를 보여주기 위한 코드 end ==//

        //== Time Table에서 시간 값 선택을 할 수 있도록 하기 위함 start ==//
        //진행중...
        private TimeModel selectedTime;
        public TimeModel SelectedTime
        {
            get => selectedTime;
            set => SetProperty(ref selectedTime, value);
        }

        private void GetTimeList()
        {

        }
        private RelayCommand getTime;
        public ICommand GetTime => getTime ??= new RelayCommand(GetTimeList);
        //== Time Table에서 시간 값 선택을 할 수 있도록 하기 위함 end ==//
    }
}
