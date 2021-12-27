﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace AdminProgram.Models
{
    public partial class ReservationListModel : ObservableObject //속성 변경 알림을 지원해야하는 모든 객체에 대한 시작점으로 사용할 수 있음
    {
        private string? patientName; //예약한 환자 이름
        public string PatientName
        {
            get => patientName;
            set => SetProperty(ref patientName, value);
        }

        private string? symptom; //증상
        public string Symptom
        {
            get => symptom;
            set => SetProperty(ref symptom, value);
        }

        private string? staffName; //예약을 진행한 간호사(?) 이름
        public string StaffName 
        { 
            get => staffName; 
            set => SetProperty(ref staffName, value); 
        }

        private DateTime reservationDT; //진료 예약 날짜
        public DateTime ReservationDT
        {
            get => reservationDT;
            set => SetProperty(ref reservationDT, value);
        }

        private string doctor; //담당 의사
        public string Doctor
        {
            get => doctor;
            set => SetProperty(ref doctor, value);
        }

        private string treatStatusVal; //?
        public string TreatStatusVal 
        { 
            get => treatStatusVal; 
            set => SetProperty(ref treatStatusVal, value); 
        }
    }
}