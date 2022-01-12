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
using System.Diagnostics;
using System.Windows.Data;
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
        private ObservableCollection<DiseaseModel> dmModel;
        public ObservableCollection<DiseaseModel> DmModels
        {
            get { return dmModel; }
            set { SetProperty(ref dmModel, value); }
        }

        private ObservableCollection<MediStaffModel> staffModel;
        public ObservableCollection<MediStaffModel> StaffModels
        {
            get { return staffModel; }
            set { SetProperty(ref staffModel, value); }
        }

        private ICollectionView filteredTreatment;
        public ICollectionView FilteredTreatment
        {
            get { return filteredTreatment; }
            set { filteredTreatment = value; }
        }

        private ObservableCollection<ProcedureFilterModel> pModel;
        public ObservableCollection<ProcedureFilterModel> PModels
        {
            get { return pModel; }
            set { SetProperty(ref pModel, value); }
        }
        private ObservableCollection<PatientModel> ptModel;
        public ObservableCollection<PatientModel> PtModels
        {
            get { return ptModel; }
            set { SetProperty(ref ptModel, value); }
        }

        private CollectionViewSource _itemSourceList;

        public TreatmentVM(ILogger<TreatmentVM> logger)
        {
            _logger = logger;
            _logger.LogInformation("{@ILogger}", logger);

            TMModels = new ObservableCollection<TreatmentModel>();
            TMModels.CollectionChanged += ContentCollectionChanged;

            DmModels = new ObservableCollection<DiseaseModel>();
            DmModels.CollectionChanged += ContentCollectionChanged;

            PModels = new ObservableCollection<ProcedureFilterModel>();
            PModels.CollectionChanged += ContentCollectionChanged;

            PModels = new ObservableCollection<ProcedureFilterModel>();
            PModels.CollectionChanged += ContentCollectionChanged;

            StaffModels = new ObservableCollection<MediStaffModel>();
            StaffModels.CollectionChanged += ContentCollectionChanged;

            GetTreatmentData();

            GetStafftData();

            //Filter Treatment list model and.
            _itemSourceList = new CollectionViewSource() { Source = TMModels };
            _itemSourceList.Filter += new FilterEventHandler(yourFilter);
            FilteredTreatment = _itemSourceList.View;
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
            var tModels = sender as TreatmentModel;
            if (tModels != null)
            {
                _logger.LogInformation("{@rModels}", tModels);
                WeakReferenceMessenger.Default.Send(TMModels);
                _logger.LogInformation("send 성공");
            }

            var dModels = sender as DiseaseModel;
            if (dModels != null)
            {
                _logger.LogInformation("{@rModels}", dModels);
                WeakReferenceMessenger.Default.Send(DmModels);
                _logger.LogInformation("send 성공");
            }

            var sModels = sender as MediStaffModel;
            if (sModels != null)
            {
                _logger.LogInformation("{@rModels}", sModels);
                WeakReferenceMessenger.Default.Send(StaffModels);
                _logger.LogInformation("send 성공");
            }

            var procModels = sender as ProcedureFilterModel;
            if (procModels != null)
            {
                _logger.LogInformation("{@rModels}", procModels);
                WeakReferenceMessenger.Default.Send(PModels);
                _logger.LogInformation("send 성공");
            }

            var patientModels = sender as PatientModel;
            if (patientModels != null)
            {
                _logger.LogInformation("{@rModels}", patientModels);
                WeakReferenceMessenger.Default.Send(PtModels);
                _logger.LogInformation("send 성공");
            }

        }
        //== Messenger 사용 end ==//

        //== SQL : 진료 정보 가져오기 ==//
        //검색어를 기준으로 진료 정보 가져옴
        //검색어가 없으면 그냥 전체 진료 정보를 가져옴

        private void GetProcedureData()
        {
            string sql = "SELECT * FROM MEDI_PROCEDURE WHERE PROCEDURE_NAME LIKE '%" + searchProcedureText + "%'";


            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    //DataGrid 사용 시 이전에 검색했던(조회했던) 내용이 없어지지 않고
                    //계속 남아있는 문제점 해결을 위해 추가
                    PModels = new ObservableCollection<ProcedureFilterModel>();
                    PModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("GetProcedureData() : select 실행");
                            try
                            {
                                while (reader.Read())
                                {
                                    PModels.Add(new ProcedureFilterModel()
                                    {
                                        Procedure_Id = reader.GetInt32(reader.GetOrdinal("MEDI_PROCEDURE_ID")),
                                        Procedure_Name = reader.GetString(reader.GetOrdinal("PROCEDURE_NAME")),
                                        Amount = reader.GetInt32(reader.GetOrdinal("TREATMENT_AMOUNT")),
                                    });

                                }
                            }
                            finally
                            {

                                _logger.LogInformation("처방 데이터 읽어오기 성공");
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

        private void GetTreatmentData()
        {
            string sql = "SELECT T3.TREAT_ID, T3.TREAT_DATE,T5.STAFF_NAME, T3.TREAT_DETAILS,T4.PATIENT_ID,T4.PATIENT_NAME, T4.PHONE_NUM,T4.GENDER,T1.PROCEDURE_LIST, T2.DISEASE_LIST FROM (SELECT TREAT_ID, LISTAGG(PROCEDURE_NAME, ',') WITHIN GROUP(ORDER BY TREAT_ID) PROCEDURE_LIST FROM(SELECT T.TREAT_ID, MP.PROCEDURE_NAME  FROM TREATMENT T JOIN TREAT_MEDI TM ON T.TREAT_ID = TM.TREATMENT_ID JOIN MEDI_PROCEDURE MP ON MP.MEDI_PROCEDURE_ID = TM.MEDI_PROCEDURE_ID) GROUP BY TREAT_ID) T1 LEFT join (SELECT TREAT_ID, LISTAGG(DISEASE_NAME, ',') WITHIN GROUP(ORDER BY TREAT_ID) DISEASE_LIST FROM (SELECT T.TREAT_ID, D.DISEASE_NAME  FROM TREATMENT T JOIN TREAT_DISEASE TD ON T.TREAT_ID = TD.TREATMENT_ID JOIN NAME_OF_DISEASE D ON TD.DISEASE_ID = D.DISEASE_ID) GROUP BY TREAT_ID) T2 ON T1.TREAT_ID = T2.TREAT_ID JOIN TREATMENT T3 ON T3.TREAT_ID = T2.TREAT_ID JOIN PATIENT T4 ON T3.PATIENT_ID = T4.PATIENT_ID JOIN MEDI_STAFF T5 ON T5.STAFF_ID = T3.STAFF_ID";

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
                                        PatientId = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        PatientPhoneNum = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                                        TreatDetail = reader.GetString(reader.GetOrdinal("TREAT_DETAILS")),
                                        Gender = reader.GetString(reader.GetOrdinal("GENDER")),
                                        Date = reader.GetDateTime(reader.GetOrdinal("TREAT_DATE")),
                                        StaffName = reader.GetString(reader.GetOrdinal("STAFF_NAME")),
                                        Diseases = reader.GetString(reader.GetOrdinal("DISEASE_LIST")),
                                        Procedures = reader.GetString(reader.GetOrdinal("PROCEDURE_LIST"))
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
                    _logger.LogInformation(err + "");
                }
            }
        }


        private DiseaseModel selectedDisease;
        public DiseaseModel SelectedDisease
        {
            get => selectedDisease;
            set => SetProperty(ref selectedDisease, value);
        }


        private PatientModel selectedPatient;
        public PatientModel SelectedPatient
        {
            get => selectedPatient;
            set => SetProperty(ref selectedPatient, value);
        }


        //검색어
        private string searchDiseaseText;
        public string SearchDiseaseText
        {
            get => searchDiseaseText;
            set => SetProperty(ref searchDiseaseText, value);
        }

        private ProcedureFilterModel selectedProcedure;
        public ProcedureFilterModel SelectedProcedure
        {
            get => selectedProcedure;
            set => SetProperty(ref selectedProcedure, value);
        }

        //검색어
        private string searchProcedureText;
        public string SearchProcedureText
        {
            get => searchProcedureText;
            set => SetProperty(ref searchProcedureText, value);
        }

        private string searchPatientText;
        public string SearchPatientText
        {
            get => searchPatientText;
            set => SetProperty(ref searchPatientText, value);
        }


        public void GetStafftData()
        {
            string sql = "SELECT STAFF_ID, STAFF_NAME, MEDI_SUBJECT " +
                            "FROM MEDI_STAFF ms " +
                            "WHERE \"POSITION\" = 'D' ";


            using (OracleConnection conn = new OracleConnection(strCon))
            {

                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    //DataGrid 사용 시 이전에 검색했던(조회했던) 내용이 없어지지 않고
                    //계속 남아있는 문제점 해결을 위해 추가
                    StaffModels = new ObservableCollection<MediStaffModel>();
                    StaffModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("GetStaffData() : select 실행");
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
                            finally
                            {

                                _logger.LogInformation("의사 데이터 읽어오기 성공");
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

        public void GetPatientData()
        {
            string sql = "SELECT * FROM PATIENT WHERE PATIENT_NAME LIKE '" + searchPatientText + "%'";


            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    //DataGrid 사용 시 이전에 검색했던(조회했던) 내용이 없어지지 않고
                    //계속 남아있는 문제점 해결을 위해 추가
                    PtModels = new ObservableCollection<PatientModel>();
                    PtModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("GetPatientData() : select 실행");
                            try
                            {
                                while (reader.Read())
                                {
                                    PtModels.Add(new PatientModel()
                                    {
                                        PatientId = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        ResidentRegistNum = reader.GetString(reader.GetOrdinal("RESIDENT_REGIST_NUM")).Insert(6, "-"),
                                        Gender = reader.GetString(reader.GetOrdinal("GENDER")),
                                        Name = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        PhoneNumber = reader.GetString(reader.GetOrdinal("PHONE_NUM")),
                                        Address = reader.GetString(reader.GetOrdinal("ADDRESS")),
                                    });

                                }
                            }
                            finally
                            {

                                _logger.LogInformation("환자 데이터 읽어오기 성공");
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

        public void GetDiseaseData()
        {
            string sql = "SELECT * FROM NAME_OF_DISEASE WHERE DISEASE_NAME LIKE '%" + searchDiseaseText + "%'";


            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation("DB Connection OK...");

                    //DataGrid 사용 시 이전에 검색했던(조회했던) 내용이 없어지지 않고
                    //계속 남아있는 문제점 해결을 위해 추가
                    DmModels = new ObservableCollection<DiseaseModel>();
                    DmModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            _logger.LogInformation("GetDiseaseData() : select 실행");
                            try
                            {
                                while (reader.Read())
                                {
                                    DmModels.Add(new DiseaseModel()
                                    {
                                        DiseaseId = reader.GetInt32(reader.GetOrdinal("DISEASE_ID")),
                                        DiseaseName = reader.GetString(reader.GetOrdinal("DISEASE_NAME"))
                                    });

                                }
                            }
                            finally
                            {

                                _logger.LogInformation("진단명 데이터 읽어오기 성공");
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

        private void ResetFilter()
        {
            SelectedStaff = null;
            SelectedProcedure = null;
            SelectedDisease = null;
            SelectedPatient = null;
            SelectedDateEnd = null;
            SelectedDateStart = null;
        }


        //filter logic for treatment data
        private void yourFilter(object sender, FilterEventArgs e)
        {
            var obj = e.Item as TreatmentModel;
            if (obj != null)
            {
                if (selectedPatient != null && obj.PatientId != selectedPatient.PatientId)
                {
                    e.Accepted = false;
                    return;
                }
                if (selectedDisease != null && !obj.Diseases.Contains(selectedDisease.DiseaseName))
                {

                    e.Accepted = false;
                    return;
                }
                if (selectedProcedure != null && !obj.Procedures.Contains(selectedProcedure.Procedure_Name))
                {
                    e.Accepted = false;
                    return;
                }
                if (selectedStaff != null && !(obj.StaffName == selectedStaff.StaffName))
                {
                    e.Accepted = false;
                    return;
                }
                if (selectedDateStart != null && obj.Date < selectedDateStart)
                {
                    e.Accepted = false;
                    return;
                }
                if (selectedDateEnd != null && obj.Date > selectedDateEnd)
                {
                    e.Accepted = false;
                    return;
                }

                e.Accepted = true;
            }
        }

        private void FilterTreatment()
        {
            _itemSourceList.Filter -= new FilterEventHandler(yourFilter);
            _itemSourceList.Filter += new FilterEventHandler(yourFilter);
            FilteredTreatment.Refresh();
        }


        private RelayCommand getTreatmentBtn;
        public ICommand GetTreatmentBtn => getTreatmentBtn ??= new RelayCommand(GetTreatmentData);

        private RelayCommand getDiseaseBtn;
        public ICommand GetDiseaseBtn => getDiseaseBtn ??= new RelayCommand(GetDiseaseData);

        private RelayCommand getProcedureBtn;
        public ICommand GetProcedureBtn => getProcedureBtn ??= new RelayCommand(GetProcedureData);

        private RelayCommand getPatientBtn;
        public ICommand GetPatientBtn => getPatientBtn ??= new RelayCommand(GetPatientData);

        private RelayCommand filterSearchBtn;
        public ICommand FilterSearchBtn => filterSearchBtn ??= new RelayCommand(FilterTreatment);

        private RelayCommand filterResetBtn;
        public ICommand FilterResetBtn => filterResetBtn ??= new RelayCommand(ResetFilter);


        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }


        private DateTime? selectedDateStart;
        public DateTime? SelectedDateStart
        {
            get => selectedDateStart;
            set => SetProperty(ref selectedDateStart, value);
        }

        private DateTime? selectedDateEnd;
        public DateTime? SelectedDateEnd
        {
            get => selectedDateEnd;
            set => SetProperty(ref selectedDateEnd, value);
        }

        private TreatmentModel selectedTreatment;
        public TreatmentModel SelectedTreatment
        {
            get => selectedTreatment;
            set => SetProperty(ref selectedTreatment, value);
        }

        private MediStaffModel selectedStaff;
        public MediStaffModel SelectedStaff
        {
            get => selectedStaff;
            set => SetProperty(ref selectedStaff, value);
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
