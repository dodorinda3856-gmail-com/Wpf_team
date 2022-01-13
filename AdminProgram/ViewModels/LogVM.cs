using AdminProgram.Models;
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
    public class LogVM : ObservableRecipient
    {
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";

        //log list
        private ObservableCollection<LogModel> lModels;
        public ObservableCollection<LogModel> LModels
        {
            get { return lModels; }
            set { SetProperty(ref lModels, value); }
        }

        public LogVM()
        {
            LModels = new ObservableCollection<LogModel>();
            LModels.CollectionChanged += ContentCollectionChanged;

            GetLogList();
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= ProductOnPropertyChanged;
                }
            }
            else
            {
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += ProductOnPropertyChanged;
                }
            }
        }

        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var lModels = sender as LogModel;
            if (lModels != null)
            {
                WeakReferenceMessenger.Default.Send(LModels);
            }
        }

        private void GetLogList()
        {
            LogRecord.LogWrite("[GetLogList() log list 읽어오기 시작]");

            string sql = 
                "SELECT USER_ID, LOG_LEVEL, ACCESS_PATH, LOG_MESSAGE, USER_IP, LOG_DATE " +
                "FROM LOG " +
                "ORDER BY USER_ID, LOG_DATE ";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    //데이터가 누적되던 문제 해결
                    LModels = new ObservableCollection<LogModel>();
                    LModels.CollectionChanged += ContentCollectionChanged;

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;
                        LogRecord.LogWrite("[GetLogList() log list 읽어오기 SQL QUERY] " + sql);

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    LModels.Add(new LogModel() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        UserId = reader.GetString(reader.GetOrdinal("USER_ID")),
                                        LogLevel = reader.GetString(reader.GetOrdinal("LOG_LEVEL")),
                                        AccessPath = reader.GetString(reader.GetOrdinal("ACCESS_PATH")),
                                        LogMessage = reader.GetString(reader.GetOrdinal("LOG_MESSAGE")),
                                        UserIp = reader.GetString(reader.GetOrdinal("USER_IP")),
                                        LogDate = reader.GetDateTime(reader.GetOrdinal("LOG_DATE"))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            { //System.InvalidCastException '열에 널 데이터가 있습니다'를 해결하기 위해 catch문 구현
                                LogRecord.LogWrite("[GetLogList() InvalidCastException] " + e);
                            }
                            finally
                            {
                                LogRecord.LogWrite("[GetLogList() log list 읽어오기 성공]");
                                reader.Close();
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    LogRecord.LogWrite("[GetLogList() log list 읽어오기 실패]");
                }
            }
        }
        private RelayCommand loadLogList;
        public ICommand LoadLogList => loadLogList ??= new RelayCommand(GetLogList);
    }
}
