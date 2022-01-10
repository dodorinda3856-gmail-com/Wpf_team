using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace AdminProgram.Models
{
    public class PatientModelTemp1 : ObservableObject
    {
        //환자 아이디
        private int patientId;
        public int PatientId
        {
            get => patientId;
            set => SetProperty(ref patientId, value);
        }

        //주민등록번호
        private string residentRegistNum;
        public string ResidentRegistNum
        {
            get => residentRegistNum;
            set => SetProperty(ref residentRegistNum, value);
        }

        //환자 이름
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        //환자 성별
        private string gender;
        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        //환자 거주지 주소
        private string address;
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        //대기 등록 시간
        private DateTime waitingRegisTime;
        public DateTime WaitingRegisTime
        {
            get => waitingRegisTime;
            set => SetProperty(ref waitingRegisTime, value);
        }

        //진료 예약 시간
        private DateTime reservationRegisTime;
        public DateTime ReservationRegisTime
        {
            get => reservationRegisTime;
            set => SetProperty(ref reservationRegisTime, value);
        }

        //요청 사항(간단한 증상 입력)
        private string symtom;
        public string Symtom
        {
            get => symtom;
            set => SetProperty(ref symtom, value);
        }

        //수납 완료 여부
        private string finVal;
        public string FinVal
        {
            get => finVal;
            set => SetProperty(ref finVal, value);
        }
    }
}
