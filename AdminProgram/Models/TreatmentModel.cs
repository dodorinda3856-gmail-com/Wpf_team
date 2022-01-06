using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

/**
 * 진료 관리 페이지 Model
 */
namespace AdminProgram.Models
{
    public partial class TreatmentModel : ObservableObject
    {
        private int patientId; //환자 번호
        public int PatientId
        {
            get => patientId;
            set => SetProperty(ref patientId, value);
        }

        private string patientName; //환자 이름
        public string PatientName
        {
            get => patientName;
            set => SetProperty(ref patientName, value);
        }

        private string patientPhoneNum; //환자 전화번호
        public string PatientPhoneNum
        {
            get => patientPhoneNum;
            set => SetProperty(ref patientPhoneNum, value);
        }

        private string gender; //환자 성별
        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        private string treatDetail; //진료 상세 내용
        public string TreatDetail
        {
            get => treatDetail;
            set => SetProperty(ref treatDetail, value);
        }
        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }
        private string staffName;
        public string StaffName
        {
            get => staffName;
            set => SetProperty(ref staffName, value);
        }

        private string diseases;
        public string Diseases
        {
            get => diseases;
            set => SetProperty(ref diseases, value);
        }


        private string procedures;
        public string Procedures
        {
            get => procedures;
            set => SetProperty(ref procedures, value);
        }
    }
}
