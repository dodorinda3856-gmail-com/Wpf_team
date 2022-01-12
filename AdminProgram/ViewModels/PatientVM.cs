using Microsoft.Toolkit.Mvvm.ComponentModel;
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

namespace AdminProgram.ViewModels
{
    public class PatientVM : ObservableRecipient
    {
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";


        private int Calculate_age(DateTime date)
        {
            int now = 2022;
            string str_tmp = date.ToString("yyyy");
            int age = Convert.ToInt32(str_tmp);
            age = now - age + 1;

            return age;
        }



        private ObservableCollection<PMModel> return_patient;
        public ObservableCollection<PMModel> Return_patients
        {
            get { return return_patient; }
            set { SetProperty(ref return_patient, value); }
        }



        public PatientVM()
        {
            
            Return_patients = new ObservableCollection<PMModel>();
            Return_patients.CollectionChanged += ContentCollectionChanged;

            GetPatientList();
        }



        public void GetPatientList()
        {
            string? sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
                       "where PATIENT_STATUS_VAL = 'T' and " +
                       "PATIENT_ID like '%%' and " +
                       "PATIENT_NAME LIKE '%%' and " +
                       "DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
                       "PHONE_NUM LIKE '%%'" +
                       " order by PATIENT_ID";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();
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
                                    Return_patients.Add(new PMModel() //.Add()를 해야지 데이터의 변화를 감지할 수 있음
                                    {
                                        Patient_ID = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
                                        Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")),
                                        Address = reader.GetString(reader.GetOrdinal("Address")),
                                        Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
                                        Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
                                        Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
                                        Gender = reader.GetString(reader.GetOrdinal("Gender")),
                                        Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                                        Age = Calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
                                    });
                                }
                            }
                            catch (InvalidCastException e)
                            {
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }


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
        private void ProductOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) { }



    }
}
