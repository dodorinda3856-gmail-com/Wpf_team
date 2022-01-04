using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
    public class PatientModel: ObservableObject
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
        private DateTime birth;
        public DateTime Birth
        {
            get => birth;
            set => SetProperty(ref birth, value);
        }

    }
}
