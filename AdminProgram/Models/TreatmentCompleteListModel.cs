using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace AdminProgram.Models
{
    public class TreatmentCompleteListModel : ObservableObject
    {
        //== 공통 ==//
        //환자 이름
        private string patientName;
        public string PatientName
        {
            get => patientName;
            set => SetProperty(ref patientName, value);
        }

        //전화번호
        private string patientPhoneNum;
        public string PatientPhoneNum
        {
            get => patientPhoneNum;
            set => SetProperty(ref patientPhoneNum, value);
        }

        //방문(예약) 시간
        private DateTime time;
        public DateTime Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }
    }
}
