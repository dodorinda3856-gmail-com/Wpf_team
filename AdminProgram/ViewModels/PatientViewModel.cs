using AdminProgram.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.ViewModels
{
    public class PatientViewModel : Notifier
    {
        PatientModel? patient = null;

        public PatientViewModel()
        {
            patient = new PatientModel();
            AddPatient();
        }

        ObservableCollection<PatientModel> _sampleDatas = null;
        public ObservableCollection<PatientModel> SampleDatas
        {
            get
            {
                if (_sampleDatas == null)
                    _sampleDatas = new ObservableCollection<PatientModel>();

                return _sampleDatas;
            }
            set
            {
                _sampleDatas = value;
            }
        }
        public void AddPatient()
        {
            PatientModel patientData = new PatientModel();

            patientData.Name = "나현정";
            patientData.Age = 23;
            patientData.Score = 100;
            patientData.Grade = "A";

            SampleDatas.Add(patientData);
        }
    }
}
