using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.ViewModels
{
    public class PMModel: ObservableObject
    {
        private int         patient_ID = 0;                     //환자id
        private string?     resident_Regist_Num = null;         //주민번호
        private string?     address = null;                     //주소
        private string?     patient_Name = null;                //이름
        private string?     phone_Num = null;                   //핸드폰번호
        private DateTime    regist_Date;                        //방문날짜
        private string?     gender = null;                      //성별
        private DateTime    dob;                                //생년월일
        private int         age = 0;                            //나이

        public int Patient_ID 
        {
            get => patient_ID;
            set => SetProperty(ref patient_ID, value);
        }

        public string? Resident_Regist_Num
        {
            get => resident_Regist_Num;
            set => SetProperty(ref resident_Regist_Num, value);
        }

        public string? Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        public string? Patient_Name
        {
            get => patient_Name;
            set => SetProperty(ref patient_Name, value);
        }

        public string? Phone_Num
        {
            get => phone_Num;
            set => SetProperty(ref phone_Num, value);
        }

        public DateTime Regist_Date
        {
            get => regist_Date;
            set => SetProperty(ref regist_Date, value);
        }

        public string? Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        public int Age
        {
            get => age;
            set => SetProperty(ref age, value);
        }

        public DateTime Dob
        {
            get => dob;
            set => SetProperty(ref dob, value);
        }

    }
}
