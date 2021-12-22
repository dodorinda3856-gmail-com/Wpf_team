using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AdminProgram.Models
{
    public partial class TMModel : ObservableObject
    {
        private int patientNumber; //환자 번호
        public int PatientNumber
        {
            get => patientNumber; 
            set => SetProperty(ref patientNumber, value); 
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
    }
}
