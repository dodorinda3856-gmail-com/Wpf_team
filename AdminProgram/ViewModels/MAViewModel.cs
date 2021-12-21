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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace AdminProgram.ViewModels
{
    public class MAViewModel : ObservableRecipient
    {
        // viewmodel에서는 model의 값을 가져와서 view에 뿌리기 전에 전처리 하는 곳
        // messenger의 send와 receive도 하는 곳
        // viewmodel은 messenger를 사용하는 통신에 직접적인 영향을 미침

        private readonly ILogger _logger;
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

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

        //
        private void PerformReservationUpdateBtn()
        {
            string sql = "SELECT p.PATIENT_NAME , r.SYMPTOM FROM RESERVATION r JOIN PATIENT p ON r.PATIENT_ID = p.PATIENT_ID";
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
                    
                    MessageBox.Show("DB Connection OK...");
                    //_logger.LogInformation("DB Connection OK...");

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            //_logger.LogInformation("select 실행");
                            try
                            {
                                //List<MAModel> datas = new List<MAModel>();

                                while (reader.Read())
                                {
                                    MAModels.Add(new MAModel() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        PatientName = reader.GetString(reader.GetOrdinal("PATIENT_NAME")),
                                        Symptom = reader.GetString(reader.GetOrdinal("SYMPTOM"))
                                        //StaffName = reader.GetString(reader.GetOrdinal("STAFF_NAME"))
                                        //TreatStatusVal=reader.GetString(reader.GetOrdinal("TREAT_STATUS_VAL"))
                                    });
                                }
                                //reservationGrid.ItemsSource = datas;
                            }
                            finally
                            {
                                //_logger.LogInformation("데이터 읽어오기 성공");
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        private RelayCommand reservationUpdateBtn;
        public ICommand ReservationUpdateBtn => reservationUpdateBtn ??= new RelayCommand(PerformReservationUpdateBtn);

    }
}
