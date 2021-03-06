using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

/**
 * 병원에서 대기중인 환자 리스트
 */
namespace AdminProgram.Models
{
    public partial class WaitingListModel : ObservableObject
    {
        //대기 번호
        private int waitingId;
        public int WaitingId
        {
            get => waitingId;
            set => SetProperty(ref waitingId, value);
        }

        //수납(진료) 완료 여부
        //T : 수납 완료
        //F : 수납 미완료
        private string waitStatusVal;
        public string WaitStatusVal
        {
            get => waitStatusVal;
            set => SetProperty(ref waitStatusVal, value);
        }

        //환자 번호
        private int patientId;
        public int PatientId
        {
            get => patientId;
            set => SetProperty(ref patientId, value);
        }

        //환자 이름
        private string patientName;
        public string PatientName
        {
            get => patientName;
            set => SetProperty(ref patientName, value);
        }

        //환자 성별
        private string patientGender;
        public string PatientGender
        {
            get => patientGender;
            set => SetProperty(ref patientGender, value);
        }

        //환자 전화번호
        private string patientPhoneNum;
        public string PatientPhoneNum
        {
            get => patientPhoneNum;
            set => SetProperty(ref patientPhoneNum, value);
        }

        //환자 집 주소
        private string patientAddress;
        public string PatientAddress
        {
            get => patientAddress;
            set => SetProperty(ref patientAddress, value);
        }

        //대기 등록 시간
        private DateTime requestToWait;
        public DateTime RequestToWait
        {
            get => requestToWait;
            set => SetProperty(ref requestToWait, value);
        }

        //환자의 증상
        private string symptom;
        public string Symptom
        {
            get => symptom;
            set => SetProperty(ref symptom, value);
        }
    }
}
